using Game.Terrains.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
