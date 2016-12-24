using Game.Items;
using System;

namespace Game.Terrains {

    [Serializable]
    class ExplosionAttribs : TileAttribs {

        public float radius, error;

        public ExplosionAttribs(Func<RawItem> dropItem) : base(dropItem) {
        }

        public override void OnInteract(int x, int y) {
            if (PlayerInventory.Instance.CurrentlySelectedItem().rawitem.id == ItemID.Igniter)
                Terrain.Explode(x, y, radius, error);
        }
    }

}
