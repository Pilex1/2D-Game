using Game.Items;
using Game.Util;
using System;
using System.Text;

namespace Game.Terrains.Logics {

    [Serializable]
    class LogicBridgeAttribs : PowerTransmitterData {

        public bool stateHorz { get; private set; }
        public bool stateVert { get; private set; }

        public LogicBridgeAttribs():base(delegate() { return RawItem.WireBridge; }) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            stateHorz = stateVert = false;
        }

        internal override void Update(int x, int y) {

            BoundedFloat bufferHorz = new BoundedFloat(0, 0, powerinL.max + powerinR.max);
            BoundedFloat bufferVert = new BoundedFloat(0, 0, powerinU.max + powerinD.max);

            powerInLCache = powerinL.val;
            powerInRCache = powerinR.val;
            powerInUCache = powerinU.val;
            powerInDCache = powerinD.val;

            BoundedFloat.MoveVals(ref powerinL, ref bufferHorz, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref bufferHorz, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref bufferVert, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref bufferVert, powerinD.val);

            EmptyInputs();

            bufferHorz -= dissipate;
            bufferVert -= dissipate;

            int neighbourHorz = base.NeighbouringLogics(x, y);
            int neighbourVert = base.NeighbouringLogics(x, y);

            if (neighbourHorz != 0) {
                float transval = bufferHorz / neighbourHorz;

                if (base.IsLogicL(x, y)) {
                    BoundedFloat.MoveVals(ref bufferHorz, ref poweroutL, transval);
                }

                if (base.IsLogicR(x, y)) {
                    BoundedFloat.MoveVals(ref bufferHorz, ref poweroutR, transval);
                }
            }
            if (neighbourVert != 0) {
                float transval = bufferVert / neighbourVert;
                if (base.IsLogicU(x, y)) {
                    BoundedFloat.MoveVals(ref bufferHorz, ref poweroutU, transval);
                }
                if (base.IsLogicD(x, y)) {
                    BoundedFloat.MoveVals(ref bufferHorz, ref poweroutD, transval);
                }
            }

            powerOutLCache = poweroutL.val;
            powerOutRCache = poweroutR.val;
            powerOutUCache = poweroutU.val;
            powerOutDCache = poweroutD.val;

            stateHorz = poweroutL > 0 || poweroutR > 0;
            stateVert = poweroutU > 0 || poweroutD > 0;

            base.UpdateAll(x, y);

            Terrain.TileAt(x, y).enumId = stateHorz ? (stateVert ? TileID.WireBridgeHorzVertOn : TileID.WireBridgeHorzOn) : (stateVert ? TileID.WireBridgeVertOn : TileID.WireBridgeOff);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Left: In {0} / Out {1}", powerInLCache, powerOutLCache));
            sb.AppendLine(string.Format("Right: In {0} / Out {1}", powerInRCache, powerOutRCache));
            sb.AppendLine(string.Format("Up: In {0} / Out {1}", powerInUCache, powerOutUCache));
            sb.AppendLine(string.Format("Down: In {0} / Out {1}", powerInDCache, powerOutDCache));
            return sb.ToString();
        }
    }
}
;