using Game.Assets;
using Game.Terrains;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Logics {

    class LogicComparer<T> : IEqualityComparer<T> {
        public bool Equals(T x, T y) {
            Logic lx = x as Logic, ly = y as Logic;
            if (lx == null || ly == null) return false;
            return lx.x == ly.x && lx.y == ly.y;
        }

        public int GetHashCode(T obj) {
            Logic l = obj as Logic;
            if (l == null) return -1;
            return (Terrain.MaxHeight + 1) * l.x + l.y;
        }
    }

    static class LogicManager {
        private static HashSet<Logic> Logics = new HashSet<Logic>(new LogicComparer<Logic>());

        public static void AddLogic(Logic l) {
            Logics.Add(l);
        }
        public static void RemoveLogic(Logic l) {
            Logics.Remove(l);
        }

        public static void Update() {
            foreach (Logic l in Logics) {
                l.Update();
            }
        }

    }
}
