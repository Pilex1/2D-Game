using Game.Assets;
using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Logics {

    class LogicComparer : IEqualityComparer<Logic> {
        //equal if tiles are the same position
        public bool Equals(Logic lx, Logic ly) {
            if (lx == null || ly == null) return false;
            return lx.x == ly.x && lx.y == ly.y;
        }

        public int GetHashCode(Logic l) {
            return (Terrain.MaxHeight + 1) * l.x + l.y;
        }
    }

    static class LogicManager {

        private static Dictionary<Vector2i, Logic> Logics = new Dictionary<Vector2i, Logic>();

        public static void AddLogic(Logic l) {
            Logics.Add(new Vector2i(l.x, l.y), l);
        }
        public static void RemoveLogic(Logic l) {
            Logics.Remove(new Vector2i(l.x, l.y));
        }

        public static void ModifyPosition(Vector2i oldloc, Vector2i newloc) {
            Logic ldict = Logics[oldloc];
            Logics.Remove(oldloc);
            ldict.SetX(newloc.x);
            ldict.SetY(newloc.y);
            Logics.Add(newloc, ldict);
        }

        public static void Update() {
            foreach (Logic l in Logics.Values) {
                l.Update();
            }
        }

    }
}
