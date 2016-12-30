using Game.Terrains.Terrain_Generation;
using System;
using Game.Terrains;
using System.Collections.Generic;
using Game.Util;
using Game.Terrains.Logics;
using Game.Terrains.Fluids;
using OpenGL;

namespace Game.Core.World_Serialization {

    [Serializable]
    internal class ChunkData {

        internal int location;
        internal Tile[,] tiles;
        internal Biome[] biomes;
        internal Vector4[,] lightings;

        internal ChunkData(int location, Tile[,] tiles, Biome[] biomes, Vector4[,] lightings) {
            this.location = location;
            this.tiles = tiles;
            this.biomes = biomes;
            this.lightings = lightings;
        }
    }
}
