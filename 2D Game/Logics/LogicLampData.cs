using System;
using Game.Util;
using System.Text;

namespace Game.Logics {

    [Serializable]
    class LogicLampAttribs : PowerDrainData {

        public bool state { get; protected set; }

        public LogicLampAttribs() {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 16;
            cost = 2;
            state = false;
        }

        internal override void Update(int x, int y) {

            BoundedFloat buffer = new BoundedFloat(0, 0, cost);

            powerInLCache = powerinL.val;
            powerInRCache = powerinR.val;
            powerInUCache = powerinU.val;
            powerInDCache = powerinD.val;

            BoundedFloat.MoveVals(ref powerinL, ref buffer, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref buffer, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref buffer, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref buffer, powerinD.val);

            base.EmptyInputs();

            state = buffer.IsFull();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Required: {0}", cost));
            sb.AppendLine(string.Format("Left: In {0}", powerInLCache));
            sb.AppendLine(string.Format("Right: In {0}", powerInRCache));
            sb.AppendLine(string.Format("Up: In {0}", powerInUCache));
            sb.AppendLine(string.Format("Down: In {0}", powerInDCache));
            return sb.ToString();
        }
    }
}
