using System.Collections.Generic;
using System.Linq;
using SystemDll;

namespace Backend
{
    public static class PlayerController
    {
        public static IList<ulong> PlayerIds => playerData.Keys.ToList();

        private static readonly Dictionary<ulong, FullPlayerData> playerData = new Dictionary<ulong, FullPlayerData>();

        public static FullPlayerData Create(ulong id, IClearableTextWriter stream)
        {
            if (playerData.ContainsKey(id)) return playerData[id];

            playerData[id] = new FullPlayerData(new PlayerData(id), stream);
            return playerData[id];
        }

        public static FullPlayerData Get(ulong id)
        {
            if (!playerData.ContainsKey(id))
            {
                return null;
            }
            return playerData[id];
        }

        public static void PerformAllPlayers()
        {
            foreach (var kv in playerData)
            {
                var streamTextWriter = kv.Value.ConsoleStream as IClearableTextWriter;
                streamTextWriter.Clear();
                var wasOpen = streamTextWriter.Open;
                streamTextWriter.Open = false;
                kv.Value.RunCode();
                streamTextWriter.Open = wasOpen;
            }

            foreach (var kv in playerData)
            {
                foreach (var robot in kv.Value.PlayerData.robots)
                {
                    var action = robot.WantedAction;
                    switch (action.Item1)
                    {
                        case RobotAction.Move:
                            var dir = (Direction)action.Item2;
                            var (x, y) = dir.ToVector();
                            robot.X += x;
                            robot.Y += y;
                            break;
                    }
                }
            }
        }
    }
}
