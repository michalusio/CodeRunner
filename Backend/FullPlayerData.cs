using Backend.CodeExecution;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SystemDll;

namespace Backend
{
    public class FullPlayerData
    {
        public readonly PlayerData PlayerData;
        public readonly IClearableTextWriter ConsoleStream;
        private string[] playerCode;

        public ReadOnlyCollection<string> PlayerCode => new ReadOnlyCollection<string>(playerCode);

        internal AppDomain PlayerAppDomain { get; private set; }


        public FullPlayerData(PlayerData data, IClearableTextWriter consoleStream)
        {
            PlayerData = data;
            ConsoleStream = consoleStream;
        }

        public bool TrySetNewAssembly(params string[] code)
        {
            try
            {
                if (PlayerAppDomain != null)
                {
                    AppDomain.Unload(PlayerAppDomain);
                    PlayerAppDomain = null;
                    GC.Collect();
                }
                var parsed = CodeCompiler.Parse(PlayerData.PlayerIdULong, code.Select(CodeHelper.GetCodeWithDLL).ToArray());
                var compiled = CodeCompiler.Compile(parsed);
                if (!compiled) return false;
                var appDomain = CodeRunner.GetExecutor(PlayerData.PlayerIdULong);
                if (appDomain != null)
                {
                    playerCode = code;
                    PlayerAppDomain = appDomain;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while compiling:");
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public void RunCode() => CodeRunner.RunCode(PlayerAppDomain, PlayerData, ConsoleStream);
    }
}
