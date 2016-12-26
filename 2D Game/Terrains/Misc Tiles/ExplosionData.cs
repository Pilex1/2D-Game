using Game.Items;
using Game.Util;
using System;

namespace Game.Terrains {

    [Serializable]
    class ExplosionAttribs : TileAttribs {

        public float radius, error;

        public ExplosionAttribs(Func<RawItem> dropItem) : base(dropItem) {
        }

        public override void OnInteract(int x, int y) {
            if (PlayerInventory.Instance.CurrentlySelectedItem().rawitem.id == ItemID.Igniter)
                Explode(x, y);
        }

        private void Explode(float x, float y) {
            for (float i = -radius + MathUtil.RandFloat(Program.Rand, -error, error); i <= radius + MathUtil.RandFloat(Program.Rand, -error, error); i++) {
                for (float j = -radius + MathUtil.RandFloat(Program.Rand, -error, error); j <= radius + MathUtil.RandFloat(Program.Rand, -error, error); j++) {
                    if (i * i + j * j <= radius * radius) {
                        Tile t = Terrain.BreakTile((int)(x + i), (int)(j + y));
                        ExplosionAttribs attribs = t.tileattribs as ExplosionAttribs;
                        if (attribs != null) {
                            attribs.Explode(x + i, y + j);
                        }
                    }
                }
            }
        }
    }

}
