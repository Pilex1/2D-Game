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

        private float sourceLevel = 20;

        private Switch(int x, int y) : base(x, y, TileID.SwitchOff) {
            powerL.max = powerR.max = powerU.max = powerD.max = sourceLevel;
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

            powerL.val = powerR.val = powerU.val = powerD.val = (id == TileID.SwitchOn ? sourceLevel : 0);

            PowerTransmitter l = Terrain.TileAt(x - 1, y) as PowerTransmitter,
                r = Terrain.TileAt(x + 1, y) as PowerTransmitter,
                u = Terrain.TileAt(x, y + 1) as PowerTransmitter,
                d = Terrain.TileAt(x, y - 1) as PowerTransmitter;

            if (l != null) BoundedFloat.MoveVals(ref powerL, ref l.transLevel, powerL * GameLogic.DeltaTime);
            if (r != null) BoundedFloat.MoveVals(ref powerR, ref r.transLevel, powerR * GameLogic.DeltaTime);
            if (u != null) BoundedFloat.MoveVals(ref powerU, ref u.transLevel, powerU * GameLogic.DeltaTime);
            if (d != null) BoundedFloat.MoveVals(ref powerD, ref d.transLevel, powerD * GameLogic.DeltaTime);
        }
    }
}
