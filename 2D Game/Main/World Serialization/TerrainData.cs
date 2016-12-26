using Game.Terrains.Gen;
using System;
using Game.Terrains;

namespace Game.Core.World_Serialization {

    [Serializable]
    public class TerrainData {
        internal Tile[,] terrain;
        internal Biome[] terrainbiomes;

        internal TerrainData(Tile[,] terrain, Biome[] terrainbiomes) {
            this.terrain = terrain;
            this.terrainbiomes = terrainbiomes;
        }
    }
}
