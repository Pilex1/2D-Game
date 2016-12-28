using Game.Entities;
using Game.Items;
using Game.Util;
using System;

namespace Game.Terrains {

    [Serializable]
    class TileAttribs {

        public Func<RawItem> dropItem;
        public bool solid = true;
        public bool movable = true;
        public bool transparent = false;
        public Direction rotation = Direction.Up;

        public TileAttribs(Func<RawItem> dropItem) {
            this.dropItem = dropItem;
        }

        public virtual void OnInteract(int x, int y) { }
        public virtual void OnEntityCollision(int x, int y, Direction side, Entity e) {
            if (!solid) return;

            switch (side) {
                case Direction.Up:
                    e.data.pos.y = y - e.hitbox.Size.y - MathUtil.Epsilon;
                    e.data.vel.y = 0;
                    break;
                case Direction.Down:
                    e.data.pos.y = y + 1;
                    e.data.vel.y = 0;
                    e.data.mvtState = MovementState.Ground;
                    break;
                case Direction.Left:
                    e.data.pos.x = x + 1;
                    e.data.vel.x = 0;
                    break;
                case Direction.Right:
                    e.data.pos.x = x - e.hitbox.Size.x - MathUtil.Epsilon;
                    e.data.vel.x = 0;
                    break;
            }
        }

        public void Destroy(int x, int y, Inventory inv) {
            Terrain.BreakTile(x, y);
            if (inv != null) {
                OnDestroy(x, y, inv);
            }
        }
        protected virtual void OnDestroy(int x, int y, Inventory inv) {
            inv.AddItem(new Item(dropItem()));
        }

        public override string ToString() {
            return "";
        }
    }
}
