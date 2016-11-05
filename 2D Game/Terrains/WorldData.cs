using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {

    [Serializable]
    class WorldData {
        public Tile[,] terrain;
        public Biome[] terrainbiomes;
        public PlayerData playerdata;
        public Entity[] entities;

        public WorldData(Tile[,] terrain, Biome[] terrainbiomes, PlayerData playerdata, Entity[] entities) {
            this.terrain = terrain;
            this.playerdata = playerdata;
            this.entities = entities;
            this.terrainbiomes = terrainbiomes;
        }
    }
}
