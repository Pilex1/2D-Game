using Game.Terrains.Core;
using Game.Util;
using System.Collections.Generic;

namespace Game.Terrains.Logics {

    class LogicManager : UpdateTileManager<LogicAttribs> {

        public static LogicManager Instance { get; private set; }
        public LogicManager() : base(1000f / 60) { }
        public static void Init() {
            Instance = new LogicManager();
        }

        public override void Update() {
            if (!cooldown.Ready()) return;
            cooldown.Reset();

            var list = new List<Vector2i>(dict.Keys);
            foreach (Vector2i v in list) {
                LogicAttribs logic;
                dict.TryGetValue(v, out logic);
                if (logic == null) continue;
                logic.Update(v.x, v.y);
            }
        }
    }
}
