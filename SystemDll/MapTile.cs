namespace SystemDll
{
    public struct MapTile
    {
        public MapTileType Type { get; internal set; }
    }

    public enum MapTileType
    {
        Empty,
        Wall
    }
}