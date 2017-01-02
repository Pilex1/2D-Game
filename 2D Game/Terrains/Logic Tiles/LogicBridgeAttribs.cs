using Game.Items;
using Game.Terrains.Lighting;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;
using System.Text;

namespace Game.Terrains.Logics {

    [Serializable]
    class LogicBridgeAttribs : PowerTransmitterData, IMultiLight {

        protected bool stateHorz;
        protected bool stateVert;

        public LogicBridgeAttribs() : base(delegate () { return RawItem.WireBridge; }) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            stateHorz = stateVert = false;
        }

        internal override void Update(int x, int y) {

            BoundedFloat bufferHorz = new BoundedFloat(0, 0, powerinL.max + powerinR.max);
            BoundedFloat bufferVert = new BoundedFloat(0, 0, powerinU.max + powerinD.max);

            CacheInputs();

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

                if (IsLogicL(x, y)) {
                    BoundedFloat.MoveVals(ref bufferHorz, ref poweroutL, transval);
                }

                if (IsLogicR(x, y)) {
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

            CacheOutputs();

            stateHorz = poweroutL > 0 || poweroutR > 0;
            stateVert = poweroutU > 0 || poweroutD > 0;

            int state = 0;
            if (stateHorz ^ stateVert) state = 1;
            else if (stateHorz && stateVert) state = 2;
            UpdateMultiLight(x, y, state, this);

           UpdateAll(x, y);

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

        ILight[] IMultiLight.Lights() => new ILight[] { new CLight(0, 0, Vector3.Zero), new CLight(4, 0.1f, new Vector3(1, 0.2f, 0.5f)), new CLight(4, 0.2f, new Vector3(1, 0.2f, 0.5f)) };
        int IMultiLight.State { get; set; }
    }
}
;