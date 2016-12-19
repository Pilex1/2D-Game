using System;
using Game.Entities;
using Game.Items;

namespace Game.Core.World_Serialization {

    [Serializable]
    public class EntitiesData {

        internal EntityData player;
        internal Item[,] playerItems;
        internal Entity[] entities;

        internal EntitiesData(EntityData player, Item[,] playerItems, Entity[] entities) {
            this.player = player;
            this.playerItems = playerItems;
            this.entities = entities;
        }
    }
}
