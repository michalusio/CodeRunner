using System;
using System.Collections.Generic;

namespace SystemDll
{
    public class PlayerData : MarshalByRefObject
    {
        public Guid PlayerId { get; }

        public IReadOnlyList<Robot> Robots => robots.AsReadOnly();

        internal List<Robot> robots;

        internal PlayerData(Guid id)
        {
            PlayerId = id;
            robots = new List<Robot> { new Robot() };
        }
    }
}