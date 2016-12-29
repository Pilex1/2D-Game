using System;
using Game.Util;
using System.Text;
using Game.Items;

namespace Game.Terrains.Logics {

    [Serializable]
    class SwitchAttribs : PowerSourceData {

        [NonSerialized]
        private CooldownTimer cooldown;

        public BoolSwitch state { get; private set; }

        private BoundedFloat src = new BoundedFloat(0, 0, 256);

        public SwitchAttribs() : base(delegate () { return RawItem.Switch; }) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = src.max;
            state = new BoolSwitch(false);
        }


        public override void OnInteract(int x, int y) {
            if (cooldown == null)
                cooldown = new CooldownTimer(20);

            if (cooldown.Ready()) {
                state.Toggle();
                cooldown.Reset();
            }
        }

        internal override void Update(int x, int y) {

            src.val = (state ? src.max : 0);

            float transval = src.val / 4;

            BoundedFloat.MoveVals(ref src, ref poweroutL, transval);
            BoundedFloat.MoveVals(ref src, ref poweroutR, transval);
            BoundedFloat.MoveVals(ref src, ref poweroutU, transval);
            BoundedFloat.MoveVals(ref src, ref poweroutD, transval);

            PowerTransmitterData l = Terrain.TileAt(x - 1, y).tileattribs as PowerTransmitterData,
                r = Terrain.TileAt(x + 1, y).tileattribs as PowerTransmitterData,
                u = Terrain.TileAt(x, y + 1).tileattribs as PowerTransmitterData,
                d = Terrain.TileAt(x, y - 1).tileattribs as PowerTransmitterData;

            if (l != null) BoundedFloat.MoveVals(ref poweroutL, ref l.powerinR, poweroutL.val);
            if (r != null) BoundedFloat.MoveVals(ref poweroutR, ref r.powerinL, poweroutR.val);
            if (u != null) BoundedFloat.MoveVals(ref poweroutU, ref u.powerinD, poweroutU.val);
            if (d != null) BoundedFloat.MoveVals(ref poweroutD, ref d.powerinU, poweroutD.val);

            Terrain.TileAt(x, y).enumId = state ? TileID.SwitchOn : TileID.SwitchOff;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Left: Out {0}", poweroutL.val));
            sb.AppendLine(string.Format("Right: Out {0}", poweroutR.val));
            sb.AppendLine(string.Format("Up: Out {0}", poweroutU.val));
            sb.AppendLine(string.Format("Down: Out {0}", poweroutD.val));
            return sb.ToString();
        }
    }
}
