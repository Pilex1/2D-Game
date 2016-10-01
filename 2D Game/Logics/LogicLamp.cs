using System;
using OpenGL;
using Game.Assets;
using Game.Terrains;
using System.Diagnostics;
using Game.Util;

namespace Game.Logics {
    class LogicLamp : PowerDrain, ISolid {

        private LogicLamp(int x, int y) : base(x, y, TileID.LogicLampUnlit) {
            power.max = 2;
        }

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new LogicLamp(x, y);
        }

        internal override void Update() {
            BoundedFloat dissipate = new BoundedFloat(0, 0, 1);
            BoundedFloat.MoveVals(ref power, ref dissipate, dissipate.max);
            id = (power.val > 0) ? TileID.LogicLampLit : TileID.LogicLampUnlit;
        }
    }
}
