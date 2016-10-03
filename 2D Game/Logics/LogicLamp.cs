using System;
using OpenGL;
using Game.Assets;
using Game.Terrains;
using System.Diagnostics;
using Game.Util;

namespace Game.Logics {

    class LogicLamp : PowerDrain, ISolid {

        private LogicLamp(int x, int y) : base(x, y, TileID.LogicLampUnlit) {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 8;
            power.max = 8;
        }

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new LogicLamp(x, y);
        }

        internal override void Update() {

            BoundedFloat.MoveVals(ref powerinL, ref power, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref power, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref power, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref power, powerinD.val);

            id = (power.val > 0) ? TileID.LogicLampLit : TileID.LogicLampUnlit;

            BoundedFloat dissipate = new BoundedFloat(0, 0, 8);
            BoundedFloat.MoveVals(ref power, ref dissipate, dissipate.max);

        }
    }
}
