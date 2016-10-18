using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {
    class WorldData {
        public TileID[,] terrain;
        public PlayerData playerdata;
        public Entity[] entities;

        public WorldData(TileID[,] terrain, PlayerData playerdata, Entity[] entities) {
            this.terrain = terrain;
            this.playerdata = playerdata;
            this.entities = entities;
        }
    }
}
