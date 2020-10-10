using System;
using System.IO;

namespace SystemDll
{
    public static class Logger
    {
        [ThreadStatic] internal static PlayerData CurrentPlayer = null;
        [ThreadStatic] internal static TextWriter CurrentConsole = null;

        public static void Log(object text)
        {
            CurrentConsole.WriteLine($"{DateTime.Now} [{CurrentPlayer?.PlayerId.ToString("00000000") ?? "ERROR"}] - {text}");
        }
    }
}
