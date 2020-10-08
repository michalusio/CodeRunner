using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using SystemDll;

namespace Backend.CodeExecution
{
    internal static class CodeRunner
    {
        private const int TICKS_IN_MILLISECOND = 10000;

        private const int MAX_MEMORY = 5 * 1024 * 1024;
        private const long MAX_PROCESSING_SPAN = 200 * TICKS_IN_MILLISECOND;

        private static readonly string MainAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string AssemblyPaths = string.Join(";", MainAssemblyLocation, Path.Combine(MainAssemblyLocation, "PlayerDLLs"));

        static CodeRunner()
        {
            AppDomain.MonitoringIsEnabled = true;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom($".\\PlayerDlls\\{args.Name}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static AppDomain GetExecutor(ulong playerId)
        {
            var playerAppDomain = AppDomain.CreateDomain($"Player {playerId}", null, new AppDomainSetup
            {
                ShadowCopyDirectories = AssemblyPaths,
                PrivateBinPath = AssemblyPaths,
                ApplicationBase = MainAssemblyLocation,
                ShadowCopyFiles = "true"
            });

            playerAppDomain.Load(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.GetName());
            playerAppDomain.Load(AssemblyName.GetAssemblyName("SystemDll.dll"));

            return playerAppDomain;
        }

        internal static void RunCode(AppDomain playerAppDomain, PlayerData currentPlayerData, IClearableTextWriter playerConsoleStream)
        {
            try
            {

                var proxy = (ServiceProxy)playerAppDomain.CreateInstanceFromAndUnwrap("SystemDll.dll", "SystemDll.ServiceProxy");
                var thread = new Thread(() =>
                {
                    proxy.StartInstance(currentPlayerData, playerConsoleStream as TextWriter);
                })
                {
                    IsBackground = true
                };
                thread.SetApartmentState(ApartmentState.STA);

                var initialTime = playerAppDomain.MonitoringTotalProcessorTime;
                long initialMemory = playerAppDomain.MonitoringTotalAllocatedMemorySize;

                thread.Start();
                while (thread.IsAlive)
                {
                    Thread.Yield();

                    if ((playerAppDomain.MonitoringTotalAllocatedMemorySize - initialMemory > MAX_MEMORY) || (playerAppDomain.MonitoringTotalProcessorTime - initialTime).Ticks > MAX_PROCESSING_SPAN)
                    {
                        thread.Abort();
                        break;
                    }
                }

                var memoryUsed = playerAppDomain.MonitoringTotalAllocatedMemorySize - initialMemory;
                var secondsUsed = (playerAppDomain.MonitoringTotalProcessorTime - initialTime).Ticks / TICKS_IN_MILLISECOND / 1000d;

                Console.WriteLine($"Ending with {FormatBytes(memoryUsed)} of memory used, in {secondsUsed:0.000} seconds");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static string FormatBytes(long bytes)
        {
            var bytesDecimal = (decimal)bytes;
            if (bytesDecimal > 1024)
            {
                bytesDecimal /= 1024;
                if (bytesDecimal > 1024)
                {
                    bytesDecimal /= 1024;
                    return bytesDecimal.ToString("F3") + "MB";
                }
                return bytesDecimal.ToString("F3") + "KB";
            }
            return bytesDecimal.ToString("F0") + 'B';
        }
    }
}
