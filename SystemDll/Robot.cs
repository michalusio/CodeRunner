using System;

namespace SystemDll
{
    public class Robot : MarshalByRefObject
    {
        public int X { get; internal set; }
        public int Y { get; internal set; }

        internal (RobotAction, object) WantedAction;

        public void Move(Direction dir)
        {
            WantedAction = (RobotAction.Move, dir);
        }
    }

    internal enum RobotAction
    {
        None = 0,
        Move
    }
}