using System;
using Game.Assets;
using OpenGL;
using Game.Terrains;
using Game.Util;

namespace Game.Logics {

    class WireData : PowerTransmitterData {

        private BoundedFloat transLevel = BoundedFloat.Zero;

        public bool state { get; private set; }

        public WireData() {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            transLevel.max = 128;
        }

        internal override void Update(int x, int y) {

            BoundedFloat.MoveVals(ref powerinL, ref transLevel, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref transLevel, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref transLevel, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref transLevel, powerinD.val);

            //calculate power output for each side
            BoundedFloat.MoveVals(ref transLevel, ref poweroutL, transLevel.val / 4);
            BoundedFloat.MoveVals(ref transLevel, ref poweroutR, transLevel.val / 4);
            BoundedFloat.MoveVals(ref transLevel, ref poweroutU, transLevel.val / 4);
            BoundedFloat.MoveVals(ref transLevel, ref poweroutD, transLevel.val / 4);

            base.UpdateAll(x, y);

            BoundedFloat.MoveVals(ref transLevel, ref dissipate, dissipate.max);
            dissipate.Empty();

            state = transLevel.val > 0;
        }
    }
}
