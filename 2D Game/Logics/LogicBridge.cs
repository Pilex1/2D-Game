using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {

    class LogicBridge : PowerTransmitter, ISolid {

        private BoundedFloat transLevelHorz = BoundedFloat.Zero;
        private BoundedFloat transLevelVert = BoundedFloat.Zero;

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new LogicBridge(x, y);
        }

        private LogicBridge(int x, int y) : base(x, y, TileID.LogicBridgeOff) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            transLevelHorz.max = transLevelVert.max = 128;
        }

        internal override void Update() {

            BoundedFloat.MoveVals(ref powerinL, ref transLevelHorz, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref transLevelHorz, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref transLevelVert, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref transLevelVert, powerinD.val);

            bool h = transLevelHorz.val > 0, v = transLevelVert.val > 0;
            id = h && v ? TileID.LogicBridgeHorzVertOn : h && !v ? TileID.LogicBridgeHorzOn : !h && v ? TileID.LogicBridgeVertOn : TileID.LogicBridgeOff;

            BoundedFloat.MoveVals(ref transLevelHorz, ref poweroutL, transLevelHorz.val / 2);
            BoundedFloat.MoveVals(ref transLevelHorz, ref poweroutR, transLevelHorz.val / 2);
            BoundedFloat.MoveVals(ref transLevelVert, ref poweroutU, transLevelVert.val / 2);
            BoundedFloat.MoveVals(ref transLevelVert, ref poweroutD, transLevelVert.val / 2);

            base.UpdateAll();

            BoundedFloat.MoveVals(ref transLevelHorz, ref dissipate, dissipate.max);
            dissipate.Empty();
            BoundedFloat.MoveVals(ref transLevelVert, ref dissipate, dissipate.max);
            dissipate.Empty();
        }
    }
}
;