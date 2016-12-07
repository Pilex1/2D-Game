using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Entities;

namespace Game.Core.World_Serialization {

    [Serializable]
    public class EntitiesData {

        internal PlayerData player;
        internal Entity[] entities;

        internal EntitiesData(PlayerData player, Entity[] entities) {
            this.player = player;
            this.entities = entities;
        }
    }
}
