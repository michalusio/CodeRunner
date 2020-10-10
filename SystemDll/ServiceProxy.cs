using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SystemDll
{
    internal class ServiceProxy : MarshalByRefObject
    {
        public void StartInstance(PlayerData currentPlayerData, TextWriter playerConsoleStream)
        {
            try
            {
                Logger.CurrentPlayer = currentPlayerData;
                Logger.CurrentConsole = playerConsoleStream;
                var assembly = AppDomain.CurrentDomain.Load(
                    AssemblyName.GetAssemblyName(Path.Combine("./PlayerDLLs", $"Player{currentPlayerData.PlayerId}Assembly.dll"))
                    );

                var entry = assembly
                    .GetType("Program", true, true)
                    .GetMethod("Main", BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);

                entry.Invoke(null, new object[] { currentPlayerData });
            }
            catch (Exception e)
            {
                if (!(e is ThreadAbortException))
                    Logger.Log(e.ToString());
            }
        }
    }
}
