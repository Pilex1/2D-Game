using Game.Terrains;
using Game.Util;
using System;

namespace Game.Logics {
    //inputs from the left
    //output from right iff left == 0
    class NOTGate : PowerTransmitter, ISolid {

        private BoundedFloat bufferPower = BoundedFloat.Zero;

        private BoundedFloat src = new BoundedFloat(0, 0, 128);

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new NOTGate(x, y);
        }

        private NOTGate(int x, int y) : base(x, y, TileID.GateNot) {
            poweroutL.max = poweroutU.max = poweroutD.max = 0;
            poweroutR.max = src.max;

            powerinL.max = src.max;
            powerinU.max = powerinD.max = powerinR.max = 0;

            bufferPower.max = src.max;
        }

        internal override void Update() {

            src.val = (powerinL.val == 0 ? src.max : 0);
            BoundedFloat.MoveVals(ref src, ref poweroutR, src.val);

            base.UpdateR();

            BoundedFloat.MoveVals(ref bufferPower, ref dissipate, dissipate.max);
            dissipate.Empty();
            BoundedFloat.MoveVals(ref powerinL, ref dissipate, dissipate.max);
            dissipate.Empty();
        }
    }
}
