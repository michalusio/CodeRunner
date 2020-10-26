using System.Runtime.CompilerServices;

namespace SystemDll
{
    internal class MapChunk
    {
        private const int ChunkSize = 1 << Map.ChunkScale;

        private readonly MapTile[,] tiles = new MapTile[ChunkSize, ChunkSize];

        internal MapTile this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => tiles[x, y];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => tiles[x, y] = value;
        }

        public MapTile[] GetFlattened()
        {
            var flatMap = new MapTile[1 << (Map.ChunkScale << 1)];
            var index = 0;
            for(int x = 0; x < ChunkSize; x++)
            {
                for(int y = 0; y < ChunkSize; y++)
                {
                    flatMap[index] = tiles[x, y];
                    index++;
                }
            }
            return flatMap;
        }
    }
}