using Game.Util;
using System;
using System.Text;

namespace Game.Logics {

    //inputs from top and bottom
    //output from right iff top > 0 || bottom > 0
    [Serializable]
    class OrGateAttribs : PowerTransmitterData {

        public OrGateAttribs() {
            poweroutL.max = poweroutU.max = poweroutD.max = 0;
            poweroutR.max = 64;

            powerinL.max = powerinR.max = 0;
            powerinU.max = powerinD.max = 64;
        }

        internal override void Update(int x, int y) {

            BoundedFloat bufferPower = new BoundedFloat(0, 0, powerinU.max + powerinD.max);

            base.powerInUCache = powerinU;
            base.powerInDCache = powerinD;

            bool cond = powerinU > 0 || powerinD > 0;

            BoundedFloat.MoveVals(ref powerinU, ref bufferPower, powerinU);
            BoundedFloat.MoveVals(ref powerinD, ref bufferPower, powerinD);

            base.EmptyInputs();

            base.EmptyOutputs();

            if (cond) {
                BoundedFloat.MoveVals(ref bufferPower, ref poweroutR, bufferPower);
            }

            base.powerOutRCache = poweroutR;

            base.UpdateR(x, y);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Right: Out {0}", powerOutRCache));
            sb.AppendLine(String.Format("Up: In {0}", powerInUCache));
            sb.AppendLine(String.Format("Down: In {0}", powerInDCache));
            return sb.ToString();
        }
    }
}
