using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {

    class LogicBridgeData : PowerTransmitterData {

        private BoundedFloat transLevelHorz = BoundedFloat.Zero;
        private BoundedFloat transLevelVert = BoundedFloat.Zero;

        public bool stateHorz { get; private set; }
        public bool stateVert { get; private set; }


        public LogicBridgeData() {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            transLevelHorz.max = transLevelVert.max = 128;
            stateHorz = stateVert = false;
        }

        internal override void Update(int x, int y) {

            BoundedFloat.MoveVals(ref powerinL, ref transLevelHorz, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref transLevelHorz, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref transLevelVert, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref transLevelVert, powerinD.val);

            BoundedFloat.MoveVals(ref transLevelHorz, ref poweroutL, transLevelHorz.val / 2);
            BoundedFloat.MoveVals(ref transLevelHorz, ref poweroutR, transLevelHorz.val / 2);
            BoundedFloat.MoveVals(ref transLevelVert, ref poweroutU, transLevelVert.val / 2);
            BoundedFloat.MoveVals(ref transLevelVert, ref poweroutD, transLevelVert.val / 2);

            base.UpdateAll(x, y);

            BoundedFloat.MoveVals(ref transLevelHorz, ref dissipate, dissipate.max);
            dissipate.Empty();
            BoundedFloat.MoveVals(ref transLevelVert, ref dissipate, dissipate.max);
            dissipate.Empty();

            stateHorz = transLevelHorz.val > 0;
            stateVert = transLevelVert.val > 0;
        }
    }
}
;