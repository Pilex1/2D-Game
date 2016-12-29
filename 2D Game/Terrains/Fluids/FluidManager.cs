using Game.Util;
using System.Collections.Generic;
using Game.Terrains.Core;

namespace Game.Terrains.Fluids {
    class FluidManager : UpdateTileManager<FluidAttribs> {

        public static FluidManager Instance { get; private set; }
        private FluidManager() : base(1000f / 60) { }
        public static void Init() {
            Instance = new FluidManager();
        }

        public void UpdateAround(int x, int y) {
            AddUpdateAround(new Vector2i(x, y));
        }
        public void AddUpdateAround(Vector2i v) {
            AddUpdateUnactive(v);
            AddUpdateUnactive(new Vector2i(v.x, v.y - 1));
            AddUpdateUnactive(new Vector2i(v.x, v.y + 1));
            AddUpdateUnactive(new Vector2i(v.x - 1, v.y));
            AddUpdateUnactive(new Vector2i(v.x + 1, v.y));
        }
        private void AddUpdateUnactive(Vector2i v) {
            FluidAttribs f = Terrain.TileAt(v).tileattribs as FluidAttribs;
            if (f != null) {
                AddUpdate(v, f);
            }
        }


        public override void Update() {
            if (!Terrain.generating) {
                if (!cooldown.Ready()) return;
                cooldown.Reset();
            }

            var list = new List<Vector2i>(dict.Keys);
            foreach (var f in list) {
                FluidAttribs fluid;
                dict.TryGetValue(f, out fluid);
                if (fluid == null) continue; //fluid removed while updating
                fluid.Update(f.x, f.y);
            }
        }


    }
}
