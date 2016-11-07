using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Fluids {
    static class FluidManager {
        private static CooldownTimer cooldown = new CooldownTimer(1000f / 60);

        public static void Update() {
            if (!cooldown.Ready()) return;
            cooldown.Reset();

            var fluidDict = Terrain.FluidDict;
            var list = new List<Vector2i>(fluidDict.Keys);
            foreach (var f in list) {
                FluidAttribs fluid;
                fluidDict.TryGetValue(f, out fluid);
                if (fluid == null) continue;
                fluid.Update(f.x, f.y);

                if (fluid.height <= 0)
                    Terrain.BreakTile(f);
            }
        }
    }
}
