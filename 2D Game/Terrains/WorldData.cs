using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {
    class WorldData {
        public Tile[,] terrain;
        public PlayerData playerdata;
        public Entity[] entities;

        public WorldData(Tile[,] terrain, PlayerData playerdata, Entity[] entities) {
            this.terrain = terrain;
            this.playerdata = playerdata;
            this.entities = entities;
        }
    }
}
