using Game.Terrains;
using Game.Util;
using Game.Items;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Fluids {
    static class FluidManager {

        private static CooldownTimer cooldown = new CooldownTimer(1000f / 60);
        private static Dictionary<Vector2i, FluidAttribs> fluidDict = new Dictionary<Vector2i, FluidAttribs>();

        public static bool Contains(int x, int y) {
            return Contains(new Vector2i(x, y));
        }
        public static bool Contains(Vector2i v) {
            return fluidDict.ContainsKey(v);
        }

        public static int GetFluidCount() {
            return fluidDict.Count;
        }

        public static void RemoveUpdate(int x, int y) {
            RemoveUpdate(new Vector2i(x, y));
        }
        public static void RemoveUpdate(Vector2i v) {
            fluidDict.Remove(v);
        }

        public static void AddUpdateAround(int x, int y) {
            AddUpdateAround(new Vector2i(x, y));
        }
        public static void AddUpdateAround(Vector2i v) {
            AddUpdateUnactive(v);
            AddUpdateUnactive(new Vector2i(v.x, v.y - 1));
            AddUpdateUnactive(new Vector2i(v.x, v.y + 1));
            AddUpdateUnactive(new Vector2i(v.x - 1, v.y));
            AddUpdateUnactive(new Vector2i(v.x + 1, v.y));
        }
        private static void AddUpdateUnactive(Vector2i v) {
            FluidAttribs f = Terrain.TileAt(v).tileattribs as FluidAttribs;
            if (f != null) {
                AddUpdate(v, f);
            }
        }
        public static void AddUpdate(int x, int y, FluidAttribs f) {
            AddUpdate(new Vector2i(x, y), f);
        }
        public static void AddUpdate(Vector2i v, FluidAttribs f) {
            if (v.x < 0 || v.x >= Terrain.Tiles.GetLength(0) || v.y < 0 || v.y >= Terrain.Tiles.GetLength(1)) {
                Debug.WriteLine("Warning - attepting to add " + f + " at " + v);
                return;
            }
            fluidDict[v] = f;
        }
        public static void ClearUpdates() {
            fluidDict.Clear();
        }
        public static int UpdateCount() {
            return fluidDict.Count;
        }
        public static void Update() {
            if (!Terrain.generating) {
                if (!cooldown.Ready()) return;
                cooldown.Reset();
            }

            var list = new List<Vector2i>(fluidDict.Keys);
            foreach (var f in list) {
                FluidAttribs fluid;
                fluidDict.TryGetValue(f, out fluid);
                if (fluid == null) continue; //fluid removed while updating
                fluid.Update(f.x, f.y);
            }
        }

        public static void CleanUp() {
            fluidDict.Clear();
        }
    }
}
