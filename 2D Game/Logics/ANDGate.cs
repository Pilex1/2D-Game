using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {

    //inputs from top and bottom
    //output from right iff top > 0 && bottom > 0
    class ANDGate : PowerTransmitter, ISolid {

        private BoundedFloat bufferPower = BoundedFloat.Zero;

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new ANDGate(x, y);
        }

        private ANDGate(int x, int y) : base(x, y, TileID.GateAnd) {
            poweroutL.max = poweroutU.max = poweroutD.max = 0;
            poweroutR.max = 64;

            powerinL.max = powerinR.max = 0;
            powerinU.max = powerinD.max = 64;

            bufferPower.max = 64;
        }

        internal override void Update() {
            bool cond = powerinU.val > 0 && powerinD.val > 0;

            if (cond) {
                BoundedFloat.MoveVals(ref powerinU, ref bufferPower, powerinU.val);
                BoundedFloat.MoveVals(ref powerinD, ref bufferPower, powerinD.val);
                BoundedFloat.MoveVals(ref bufferPower, ref poweroutR, bufferPower.val);
            }

            base.UpdateR();

            BoundedFloat.MoveVals(ref bufferPower, ref dissipate, dissipate.max);
            dissipate.Empty();
        }
    }
}
