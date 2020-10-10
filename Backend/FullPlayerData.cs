using Backend.CodeExecution;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SystemDll;

namespace Backend
{
    public class FullPlayerData
    {
        public PlayerData PlayerData { get; }
        public RichTextBoxWriter ConsoleStream { get; }
        public ReadOnlyCollection<string> PlayerCode => new ReadOnlyCollection<string>(playerCode);

        private string[] playerCode;
        private AppDomain playerAppDomain;


        public FullPlayerData(PlayerData data, RichTextBoxWriter consoleStream)
        {
            PlayerData = data;
            ConsoleStream = consoleStream;
        }

        public bool TrySetNewAssembly(params string[] code)
        {
            try
            {
                if (playerAppDomain != null)
                {
                    AppDomain.Unload(playerAppDomain);
                    playerAppDomain = null;
                    GC.Collect();
                }
                var parsed = CodeCompiler.Parse(PlayerData.PlayerId, code.Select(CodeHelper.GetCodeWithDLL));
                var compiled = CodeCompiler.Compile(parsed);
                if (!compiled) return false;
                var appDomain = CodeRunner.GetExecutor(PlayerData.PlayerId);
                if (appDomain != null)
                {
                    playerCode = code;
                    playerAppDomain = appDomain;
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

        public void RunCode() => CodeRunner.RunCode(playerAppDomain, PlayerData, ConsoleStream);
    }
}
