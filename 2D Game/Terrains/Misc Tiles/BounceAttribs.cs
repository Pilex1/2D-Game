using Game.Entities;
using Game.Util;
using System;
using Game.Items;

namespace Game.Terrains {


    [Serializable]
    class BounceAttribs : TileAttribs {

        private float bouncePowerVert = 1.2f;
       private float bouncePowerHorz = 1.8f;

        public BounceAttribs() : base(delegate() { return RawItem.Bounce; }) { }

        public override void OnEntityCollision(int x, int y, Direction side, Entity e) {
            if (side == Direction.Up || side == Direction.Down) {
                e.ReboundY();
                e.data.vel.y *= bouncePowerVert;
            }
            if (side == Direction.Left || side == Direction.Right) {
                e.ReboundX();
                e.data.vel.x *= bouncePowerHorz;
            }
        }
    }
}
