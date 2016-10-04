using System;
using OpenGL;
using Game.Assets;
using Game.Util;
using Game.Terrains;
using System.Diagnostics;

namespace Game.Logics {

    [Serializable]
    class SwitchData : PowerSourceData {

        [NonSerialized]
        private CooldownTimer cooldown;

        public BoolSwitch state { get; private set; }

        private BoundedFloat src = new BoundedFloat(0, 0, 256);

        public SwitchData() {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = src.max;
            state = false;
            cooldown = new CooldownTimer(20);
        }

        public override void Interact(int x, int y) {
            if (cooldown.Ready()) {
                state.Toggle();
                cooldown.Reset();
            }
        }

        internal override void Update(int x, int y) {

            src.val = (state ? src.max : 0);
            BoundedFloat.MoveVals(ref src, ref poweroutL, src.val);
            src.val = (state ? src.max : 0);
            BoundedFloat.MoveVals(ref src, ref poweroutR, src.val);
            src.val = (state ? src.max : 0);
            BoundedFloat.MoveVals(ref src, ref poweroutU, src.val);
            src.val = (state ? src.max : 0);
            BoundedFloat.MoveVals(ref src, ref poweroutD, src.val);

            PowerTransmitterData l = Terrain.TileAt(x - 1, y).tileattribs as PowerTransmitterData,
                r = Terrain.TileAt(x + 1, y).tileattribs as PowerTransmitterData,
                u = Terrain.TileAt(x, y + 1).tileattribs as PowerTransmitterData,
                d = Terrain.TileAt(x, y - 1).tileattribs as PowerTransmitterData;

            if (l != null) BoundedFloat.MoveVals(ref poweroutL, ref l.powerinR, poweroutL.val);
            if (r != null) BoundedFloat.MoveVals(ref poweroutR, ref r.powerinL, poweroutR.val);
            if (u != null) BoundedFloat.MoveVals(ref poweroutU, ref u.powerinD, poweroutU.val);
            if (d != null) BoundedFloat.MoveVals(ref poweroutD, ref d.powerinU, poweroutD.val);
        }
    }
}
