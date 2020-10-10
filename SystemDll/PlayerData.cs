using System;
using System.Collections.Generic;

namespace SystemDll
{
    public class PlayerData : MarshalByRefObject
    {
        public ulong PlayerId { get; }

        public IReadOnlyList<Robot> Robots => robots.AsReadOnly();

        internal List<Robot> robots;

        internal PlayerData(ulong id)
        {
            PlayerId = id;
            robots = new List<Robot> { new Robot() };
        }
    }
}