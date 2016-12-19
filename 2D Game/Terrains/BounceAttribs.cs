using Game.Entities;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {


    [Serializable]
    class BounceAttribs : TileAttribs {

        float bouncePowerVert = -1.2f;
        float bouncePowerHorz = -1.8f;

        public override void OnTerrainIntersect(int x, int y, Direction side, Entity e) {
            if (side == Direction.Up || side == Direction.Down) {
                e.data.vel.y *= bouncePowerVert;
            }
            if (side == Direction.Left || side == Direction.Right) {
                e.data.vel.x *= bouncePowerHorz;
            }
        }
    }
}
