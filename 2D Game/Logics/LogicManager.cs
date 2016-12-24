using Game.Terrains;
using Game.Util;
using System.Collections.Generic;

namespace Game.Logics {

    static class LogicManager {

        private static CooldownTimer cooldown = new CooldownTimer(1000f / 60);

        public static void Update() {
            if (!cooldown.Ready()) return;
            cooldown.Reset();

            var logicDict = Terrain.LogicDict;
            var list = new List<Vector2i>(logicDict.Keys);
            foreach (Vector2i v in list) {
                LogicAttribs logic;
                logicDict.TryGetValue(v, out logic);
                if (logic == null) continue;
                logic.Update(v.x, v.y);
            }
        }
    }
}
