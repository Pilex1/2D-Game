using Game.Interaction;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {
    class ExplosionData : TileAttribs {

        public float radius, error;

        public object HotbarExplosion { get; private set; }

        public override void Interact(int x, int y) {
            if (Hotbar.CurrentlySelectedItem()==Assets.ItemId.Igniter)
                Explosion.Explode(x, y, radius, error);
        }
    }

    static class Explosion {

        private static Random Rand = new Random();

        public static void Explode(float x, float y, float radius, float error) {
            for (float i = -radius + MathUtil.RandFloat(Rand, -error, error); i <= radius + MathUtil.RandFloat(Rand, -error, error); i++) {
                float jStart = (float)-Math.Sqrt(radius * radius - i * i) + MathUtil.RandFloat(Rand, -error, error);
                float jEnd = (float)Math.Sqrt(radius * radius - i * i) + MathUtil.RandFloat(Rand, -error, error);
                if (jStart == float.NaN || jEnd == float.NaN) continue;
                for (int j = (int)jStart; j <= jEnd; j++) {
                    if (Terrain.BreakTile(x + i, j + y).enumId == TileEnum.Tnt) Explode(x + i, j + y, radius, error);
                }
            }
        }
    }

}
