using System;
using Game.Util;
using System.Text;
using Game.Items;

namespace Game.Terrains.Logics {

    [Serializable]
    class WireAttribs : PowerTransmitterData {

        public bool state { get; private set; }

        public WireAttribs() : base(delegate () { return RawItem.Wire; }) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            transparent = true;
        }

        internal override void Update(int x, int y) {

            //power input
            BoundedFloat buffer = new BoundedFloat(0, 0, powerinL.max + powerinR.max + powerinU.max + powerinD.max);

            powerInLCache = powerinL.val;
            powerInRCache = powerinR.val;
            powerInUCache = powerinU.val;
            powerInDCache = powerinD.val;

            BoundedFloat.MoveVals(ref powerinL, ref buffer, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref buffer, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref buffer, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref buffer, powerinD.val);

            EmptyInputs();

            buffer -= dissipate;

            //power output
            base.EmptyOutputs();

            int neighbouring = NeighbouringLogics(x, y);
            if (neighbouring != 0) {
                float transval = buffer.val / neighbouring;

                if (base.IsLogicL(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutL, transval);

                if (base.IsLogicR(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutR, transval);

                if (base.IsLogicU(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutU, transval);

                if (base.IsLogicD(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutD, transval);
            }

            powerOutLCache = poweroutL.val;
            powerOutRCache = poweroutR.val;
            powerOutUCache = poweroutU.val;
            powerOutDCache = poweroutD.val;

            state = poweroutL > 0 || poweroutR > 0 || poweroutU > 0 || poweroutD > 0;

            base.UpdateAll(x, y);

            Terrain.TileAt(x, y).enumId = state ? TileID.WireOn : TileID.WireOff;
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
