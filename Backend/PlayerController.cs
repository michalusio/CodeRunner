using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SystemDll;

namespace Backend
{
    public static class PlayerController
    {
        public static ImmutableList<Guid> PlayerIds => playerData.Keys.ToImmutableList();

        private static readonly Dictionary<Guid, FullPlayerData> playerData = new Dictionary<Guid, FullPlayerData>();

        public static FullPlayerData Create(Guid id, RichTextBoxWriter stream)
        {
            if (playerData.ContainsKey(id)) return playerData[id];

            playerData[id] = new FullPlayerData(new PlayerData(id), stream);
            return playerData[id];
        }

        public static FullPlayerData Get(Guid id)
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
                var streamTextWriter = kv.Value.ConsoleStream;
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
