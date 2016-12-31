using Game.Terrains.Terrain_Generation;
using System;
using Game.Terrains;
using OpenGL;

namespace Game.Core.World_Serialization {

    [Serializable]
    internal class ChunkData {

        internal int location;
        internal Tile[,] tiles;
        internal Biome[] biomes;
        internal Vector3[,] lightings;

        internal ChunkData(int location, Tile[,] tiles, Biome[] biomes, Vector3[,] lightings) {
            this.location = location;
            this.tiles = tiles;
            this.biomes = biomes;
            this.lightings = lightings;
        }
    }
}
