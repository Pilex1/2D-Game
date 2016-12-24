using Game.Terrains;
using Game.Util;
using Game.Items;
using System.Collections.Generic;

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

                if (fluid.height <= 0.0001f) {
                    fluid.Destroy(f.x, f.y, PlayerInventory.Instance);
                }
            }
        }
    }
}
