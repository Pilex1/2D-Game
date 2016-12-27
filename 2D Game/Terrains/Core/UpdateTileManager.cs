using Game.Util;
using System.Collections.Generic;

namespace Game.Terrains.Core {
    abstract class UpdateTileManager<T> {

        protected CooldownTimer cooldown;
        protected Dictionary<Vector2i, T> dict;

        protected UpdateTileManager(float cooldowntime) {
            cooldown = new CooldownTimer(cooldowntime);
            dict = new Dictionary<Vector2i, T>();
        }

        public void LoadDict(Dictionary<Vector2i, T> f) {
            dict = f;
        }
        public Dictionary<Vector2i, T> GetDict() {
            return dict;
        }

        public bool Contains(int x, int y) {
            return Contains(new Vector2i(x, y));
        }
        public bool Contains(Vector2i v) {
            return dict.ContainsKey(v);
        }
        public void RemoveUpdate(int x, int y) {
            RemoveUpdate(new Vector2i(x, y));
        }
        public void RemoveUpdate(Vector2i v) {
            dict.Remove(v);
        }
        public void AddUpdate(int x, int y, T f) {
            AddUpdate(new Vector2i(x, y), f);
        }
        public void AddUpdate(Vector2i v, T f) {
            if (v.x < 0 || v.x >= Terrain.Tiles.GetLength(0) || v.y < 0 || v.y >= Terrain.Tiles.GetLength(1)) return;
            dict[v] = f;
        }
        public void Clear() {
            dict.Clear();
        }
        public int GetCount() {
            return dict.Count;
        }

        public void CleanUp() {
            Clear();
        }

        public abstract void Update();

    }
}
