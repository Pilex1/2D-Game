using Game.Interaction;
using System;

namespace Game.Terrains {

    [Serializable]
    class ExplosionData : TileAttribs {

        public float radius, error;

        public object HotbarExplosion { get; private set; }

        public override void Interact(int x, int y) {
            if (PlayerInventory.Instance.CurrentlySelectedItem() == Assets.ItemID.Igniter)
                Terrain.Explode(x, y, radius, error);
        }
    }

}
