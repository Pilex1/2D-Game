using System;
using Game.Assets;
using OpenGL;
using Game.Terrains;
using Game.Util;

namespace Game.Logics {

    class Wire : PowerTransmitter {

        private BoundedFloat transLevel = BoundedFloat.Zero;

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new Wire(x, y);
        }

        private Wire(int x, int y) : base(x, y, TileID.WireOff) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            transLevel.max = 128;
        }

        internal override void Update() {

            BoundedFloat.MoveVals(ref powerinL, ref transLevel, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref transLevel, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref transLevel, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref transLevel, powerinD.val);

            id = (transLevel.val > 0 ? TileID.WireOn : TileID.WireOff);

            //calculate power output for each side
            BoundedFloat.MoveVals(ref transLevel, ref poweroutL, transLevel.val / 4);
            BoundedFloat.MoveVals(ref transLevel, ref poweroutR, transLevel.val / 4);
            BoundedFloat.MoveVals(ref transLevel, ref poweroutU, transLevel.val / 4);
            BoundedFloat.MoveVals(ref transLevel, ref poweroutD, transLevel.val / 4);

            base.UpdateAll();

            BoundedFloat.MoveVals(ref transLevel, ref dissipate, dissipate.max);
            dissipate.Empty();
        }
    }
}
