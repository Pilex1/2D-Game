using System;
using OpenGL;
using Game.Assets;
using Game.Terrains;
using System.Diagnostics;
using Game.Util;

namespace Game.Logics {

    class LogicLampData : PowerDrainData {

        public bool state { get; protected set; }

        public LogicLampData() {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 16;
            power.max = 16;
            cost.max = 2;
            state = false;
        }

        internal override void Update(int x, int y) {

            BoundedFloat.MoveVals(ref powerinL, ref power, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref power, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref power, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref power, powerinD.val);

            BoundedFloat.MoveVals(ref power, ref cost, cost.max);
            cost.Empty();

            state = power.val > 0;
        }
    }
}
