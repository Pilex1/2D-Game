using Game.Terrains.Terrain_Generation;
using System;
using Game.Terrains;
using Pencil.Gaming.MathUtils;

namespace Game.Core.world_Serialization {

    [Serializable]
    internal class ChunkData {
        internal int location;
        internal Tile[,] tiles;
        internal Biome[] biomes;
    }
}
