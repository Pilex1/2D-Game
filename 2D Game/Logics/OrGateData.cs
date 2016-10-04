using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {

    //inputs from top and bottom
    //output from right iff top > 0 || bottom > 0
    [Serializable]
    class OrGateData : PowerTransmitterData {

        private BoundedFloat bufferPower = BoundedFloat.Zero;

        public OrGateData() {
            poweroutL.max = poweroutU.max = poweroutD.max = 0;
            poweroutR.max = 64;

            powerinL.max = powerinR.max = 0;
            powerinU.max = powerinD.max = 64;

            bufferPower.max = 64;
        }

        internal override void Update(int x, int y) {
            BoundedFloat.MoveVals(ref powerinU, ref dissipate, dissipate.max / 2);
            dissipate.Empty();
            BoundedFloat.MoveVals(ref powerinD, ref dissipate, dissipate.max / 2);
            dissipate.Empty();

            bool cond = powerinU.val > 0 || powerinD.val > 0;
            if (cond) {
                BoundedFloat.MoveVals(ref powerinU, ref bufferPower, powerinU.val);
                BoundedFloat.MoveVals(ref powerinD, ref bufferPower, powerinD.val);
                BoundedFloat.MoveVals(ref bufferPower, ref poweroutR, bufferPower.val);
            }
            base.UpdateR(x, y);
        }
    }
}
