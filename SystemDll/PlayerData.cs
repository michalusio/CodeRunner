using System;
using System.Collections.Generic;

namespace SystemDll
{
    public class PlayerData : MarshalByRefObject
    {
        public string PlayerId => PlayerIdULong.ToString("00000000");
        public ulong PlayerIdULong { get; private set; }

        public IReadOnlyList<Robot> Robots => robots.AsReadOnly();

        internal List<Robot> robots;

        internal PlayerData(ulong id)
        {
            PlayerIdULong = id;
            robots = new List<Robot> { new Robot() };
        }
    }
}