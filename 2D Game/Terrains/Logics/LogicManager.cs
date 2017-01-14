using Game.Terrains.Core;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;

namespace Game.Terrains.Logics {

    class LogicManager : UpdateTileManager<LogicAttribs> {

        public static LogicManager Instance { get; private set; }
        private LogicManager(Dictionary<Vector2i, LogicAttribs> dict) : base(1000f / 60, dict) { }
        public static void Init(Dictionary<Vector2i, LogicAttribs> dict) {
            if (dict == null) {
                Instance = new LogicManager(new Dictionary<Vector2i, LogicAttribs>());
            } else {
                Instance = new LogicManager(dict);
            }
        }

        protected override void OnUpdate() {
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
