using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SystemDll
{
    public class Map
    {
        internal const int ChunkScale = 4;

        private const int ChunkSizeN1 = (1 << ChunkScale) - 1;

        private readonly Dictionary<(int x, int y), MapChunk> chunks = new Dictionary<(int x, int y), MapChunk>();
        
        public MapTile this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var chunkCoords = (x >> ChunkScale, y >> ChunkScale);
                var tileCoords = (x: x & ChunkSizeN1, y: y & ChunkSizeN1);
                EnsureChunkAvailable(chunkCoords);
                return chunks[chunkCoords][tileCoords.x, tileCoords.y];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set
            {
                var chunkCoords = (x >> ChunkScale, y >> ChunkScale);
                var tileCoords = (x: x & ChunkSizeN1, y: y & ChunkSizeN1);
                EnsureChunkAvailable(chunkCoords);
                chunks[chunkCoords][tileCoords.x, tileCoords.y] = value;
            }
        }

        internal MapTile[] GetFlattenedChunk(int chunkX, int chunkY)
        {
            var chunkCoords = (chunkX, chunkY);
            EnsureChunkAvailable(chunkCoords);
            return chunks[chunkCoords].GetFlattened();
        }

        private void EnsureChunkAvailable((int, int) chunkCoords)
        {
            if (!chunks.ContainsKey(chunkCoords))
            {
                chunks.Add(chunkCoords, new MapChunk());
            }
        }
    }
}
