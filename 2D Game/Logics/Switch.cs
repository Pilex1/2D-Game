using System;
using OpenGL;
using Game.Assets;
using Game.Util;
using Game.Terrains;
using System.Diagnostics;

namespace Game.Logics {

    class Switch : PowerSource, IRightInteractable, ISolid {

        private const float cooldown = 20; //time before it can be switched again
        private float cooldownTime = 0;

        private BoundedFloat src = new BoundedFloat(0, 0, 128);

        private Switch(int x, int y) : base(x, y, TileID.SwitchOff) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = src.max / 4;
        }

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new Switch(x, y);
        }

        public void Interact() {
            if (cooldownTime >= cooldown) {
                id = (id == TileID.SwitchOff ? TileID.SwitchOn : TileID.SwitchOff);
                cooldownTime = 0;
            }
        }

        internal override void Update() {

            cooldownTime += GameLogic.DeltaTime;

            src.val = (id == TileID.SwitchOn ? src.max : 0);
            BoundedFloat.MoveVals(ref src, ref poweroutL, src.val / 4);
            BoundedFloat.MoveVals(ref src, ref poweroutR, src.val / 4);
            BoundedFloat.MoveVals(ref src, ref poweroutU, src.val / 4);
            BoundedFloat.MoveVals(ref src, ref poweroutD, src.val / 4);

            PowerTransmitter l = Terrain.TileAt(x - 1, y) as PowerTransmitter,
                r = Terrain.TileAt(x + 1, y) as PowerTransmitter,
                u = Terrain.TileAt(x, y + 1) as PowerTransmitter,
                d = Terrain.TileAt(x, y - 1) as PowerTransmitter;

            if (l != null) BoundedFloat.MoveVals(ref poweroutL, ref l.powerinR, poweroutL.val);
            if (r != null) BoundedFloat.MoveVals(ref poweroutR, ref r.powerinL, poweroutR.val);
            if (u != null) BoundedFloat.MoveVals(ref poweroutU, ref u.powerinD, poweroutU.val);
            if (d != null) BoundedFloat.MoveVals(ref poweroutD, ref d.powerinU, poweroutD.val);
        }
    }
}
