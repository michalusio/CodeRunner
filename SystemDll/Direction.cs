namespace SystemDll
{
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = -1,
        Left = 2,
        Right = -2
    }

    public static class DirectionExtensions
    {
        public static (int x, int y) ToVector(this Direction dir)
        {
            switch (dir)
            {
                default:
                    return (0, 0);
                case Direction.Up:
                    return (0, 1);
                case Direction.Down:
                    return (0, -1);
                case Direction.Left:
                    return (-1, 0);
                case Direction.Right:
                    return (1, 0);
            }
        }
    }
}