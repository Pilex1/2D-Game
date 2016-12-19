using Game.Entities;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {

    [Serializable]
    class TileAttribs {
        public bool solid = true;
        public bool movable = true;
        public bool transparent = false;
        public Direction rotation = Direction.Up;

        public virtual void Interact(int x, int y) { }
        public virtual void OnTerrainIntersect(int x, int y, Direction side, Entity e) {
            switch (side) {
                case Direction.Up:
                    e.data.pos.y = y - e.hitbox.Size.y - MathUtil.Epsilon;
                    e.data.vel.y = 0;
                    break;
                case Direction.Down:
                    e.data.pos.y = y + 1;
                    e.data.vel.y = 0;
                    e.data.InAir = false;
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

        public override string ToString() {
            return "";
        }
    }
}
