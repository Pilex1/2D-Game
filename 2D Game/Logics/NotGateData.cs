using Game.Terrains;
using Game.Util;
using System;

namespace Game.Logics {
    //inputs from the left
    //output from right iff left == 0
    [Serializable]
    class NotGateData : PowerTransmitterData {

        private BoundedFloat bufferPower = BoundedFloat.Zero;

        private BoundedFloat src = new BoundedFloat(0, 0, 128);

        public NotGateData() {
            poweroutL.max = poweroutU.max = poweroutD.max = 0;
            poweroutR.max = src.max;

            powerinL.max = src.max;
            powerinU.max = powerinD.max = powerinR.max = 0;

            bufferPower.max = src.max;
        }

        internal override void Update(int x, int y) {
            BoundedFloat.MoveVals(ref src, ref poweroutR, src.val);

            base.UpdateR(x, y);

            BoundedFloat.MoveVals(ref bufferPower, ref dissipate, dissipate.max);
            dissipate.Empty();
            BoundedFloat.MoveVals(ref powerinL, ref dissipate, dissipate.max);
            dissipate.Empty();


            src.val = (powerinL.val == 0 ? src.max : 0);
        }
    }
}
