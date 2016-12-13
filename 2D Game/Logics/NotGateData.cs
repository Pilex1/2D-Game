using Game.Util;
using System;
using System.Text;

namespace Game.Logics {

    //inputs from the left
    //output from right iff left == 0
    [Serializable]
    class NotGateAttribs : PowerTransmitterData {

        private BoundedFloat src = new BoundedFloat(0, 0, 256);

        public NotGateAttribs() {
            poweroutL.max = poweroutU.max = poweroutD.max = 0;
            poweroutR.max = src.max;

            powerinL.max = src.max;
            powerinU.max = powerinD.max = powerinR.max = 0;
        }

        internal override void Update(int x, int y) {

            base.powerInLCache = powerinL;

            base.EmptyOutputs();

            if (powerinL > 0) {
                poweroutR.val = 0;
            } else {
                poweroutR.val = src.max;
            }

            base.EmptyInputs();

            base.powerOutRCache = poweroutR;

            base.UpdateR(x, y);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Left: In {0}", powerInLCache));
            sb.AppendLine(String.Format("Right: Out {0}", powerOutRCache));
            return sb.ToString();
        }
    }
}
