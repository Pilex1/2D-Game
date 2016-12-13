using System;
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
