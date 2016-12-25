using Game.Terrains;
using Game.Util;
using Game.Items;
using System.Collections.Generic;
using System.Diagnostics;

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
                if (fluid == null) {
                    Debug.WriteLine("Warning: fluid null at " + f);
                    continue;
                }
                fluid.Update(f.x, f.y);
            }
        }
    }
}
