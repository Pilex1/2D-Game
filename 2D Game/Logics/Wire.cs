using System;
using Game.Assets;
using OpenGL;
using Game.Terrains;
using Game.Util;

namespace Game.Logics {
    class Wire : PowerTransmitter, ISolid {

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new Wire(x, y);
        }

        private Wire(int x, int y) : base(x, y, TileID.WireOff) {
            powerL.max = powerR.max = powerU.max = powerD.max = 64;
            transLevel.max = 128;
        }

        internal override void Update() {

            BoundedFloat dissipate = new BoundedFloat(0, 0, 1);
            BoundedFloat.MoveVals(ref transLevel, ref dissipate, dissipate.max);

            id = (transLevel.val > 0 ? TileID.WireOn : TileID.WireOff);
            //calculate power output for each side
            float transval = transLevel.val / 4;
            BoundedFloat.MoveVals(ref transLevel, ref powerL, transval);
            BoundedFloat.MoveVals(ref transLevel, ref powerR, transval);
            BoundedFloat.MoveVals(ref transLevel, ref powerU, transval);
            BoundedFloat.MoveVals(ref transLevel, ref powerD, transval);

            UpdateTransmitters();
            UpdateDrains();
        }

        private void UpdateTransmitters() {
            PowerTransmitter l = Terrain.TileAt(x - 1, y) as PowerTransmitter,
                r = Terrain.TileAt(x + 1, y) as PowerTransmitter,
                u = Terrain.TileAt(x, y + 1) as PowerTransmitter,
                d = Terrain.TileAt(x, y - 1) as PowerTransmitter;

            if (l != null) BoundedFloat.MoveVals(ref powerL, ref l.transLevel, powerL);
            if (r != null) BoundedFloat.MoveVals(ref powerR, ref r.transLevel, powerR);
            if (u != null) BoundedFloat.MoveVals(ref powerU, ref u.transLevel, powerU);
            if (d != null) BoundedFloat.MoveVals(ref powerD, ref d.transLevel, powerD);
        }

        private void UpdateDrains() {
            PowerDrain l = Terrain.TileAt(x - 1, y) as PowerDrain,
                r = Terrain.TileAt(x + 1, y) as PowerDrain,
                u = Terrain.TileAt(x, y + 1) as PowerDrain,
                d = Terrain.TileAt(x, y - 1) as PowerDrain;

            if (l != null) BoundedFloat.MoveVals(ref powerL, ref l.power, powerL);
            if (r != null) BoundedFloat.MoveVals(ref powerR, ref r.power, powerR);
            if (u != null) BoundedFloat.MoveVals(ref powerU, ref u.power, powerU);
            if (d != null) BoundedFloat.MoveVals(ref powerD, ref d.power, powerD);
        }
    }
}
