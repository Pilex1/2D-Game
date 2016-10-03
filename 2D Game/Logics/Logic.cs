using Game.Assets;
using Game.Terrains;
using Game.Util;
using System;

namespace Game.Logics {

    abstract class Logic : Tile {

        protected Logic(int x, int y, TileID id) : base(x, y, id) {

            LogicManager.AddLogic(this);
        }

        public void SetX(int x) => this.x = x;
        public void SetY(int y) => this.y = y;

        internal abstract void Update();

    }

    internal abstract class PowerSource : Logic {

        //power available to draw from each side
        protected BoundedFloat poweroutL = BoundedFloat.Zero;
        protected BoundedFloat poweroutR = BoundedFloat.Zero;
        protected BoundedFloat poweroutU = BoundedFloat.Zero;
        protected BoundedFloat poweroutD = BoundedFloat.Zero;

        public PowerSource(int x, int y, TileID id) : base(x, y, id) {
        }
    }

    internal abstract class PowerTransmitter : Logic {

        //power on each side
        protected BoundedFloat poweroutL = BoundedFloat.Zero;
        protected BoundedFloat poweroutR = BoundedFloat.Zero;
        protected BoundedFloat poweroutU = BoundedFloat.Zero;
        protected BoundedFloat poweroutD = BoundedFloat.Zero;

        internal BoundedFloat powerinL = BoundedFloat.Zero;
        internal BoundedFloat powerinR = BoundedFloat.Zero;
        internal BoundedFloat powerinU = BoundedFloat.Zero;
        internal BoundedFloat powerinD = BoundedFloat.Zero;

        protected BoundedFloat dissipate = new BoundedFloat(0, 0, 1);

        protected PowerTransmitter(int x, int y, TileID id) : base(x, y, id) {
        }

        protected void UpdateL() {
            PowerTransmitter tl = Terrain.TileAt(x - 1, y) as PowerTransmitter;
            if (tl != null) BoundedFloat.MoveVals(ref poweroutL, ref tl.powerinR, poweroutL.val);

            PowerDrain dl = Terrain.TileAt(x - 1, y) as PowerDrain;
            if (dl != null) BoundedFloat.MoveVals(ref poweroutL, ref dl.powerinR, poweroutL.val);
        }
        protected void UpdateR() {
            PowerTransmitter tr = Terrain.TileAt(x + 1, y) as PowerTransmitter;
            if (tr != null) BoundedFloat.MoveVals(ref poweroutR, ref tr.powerinL, poweroutR.val);

            PowerDrain dr = Terrain.TileAt(x + 1, y) as PowerDrain;
            if (dr != null) BoundedFloat.MoveVals(ref poweroutR, ref dr.powerinL, poweroutR.val);
        }
        protected void UpdateU() {
            PowerTransmitter tu = Terrain.TileAt(x, y + 1) as PowerTransmitter;
            if (tu != null) BoundedFloat.MoveVals(ref poweroutU, ref tu.powerinD, poweroutU.val);

            PowerDrain du = Terrain.TileAt(x, y + 1) as PowerDrain;
            if (du != null) BoundedFloat.MoveVals(ref poweroutU, ref du.powerinD, poweroutU.val);
        }
        protected void UpdateD() {
            PowerTransmitter td = Terrain.TileAt(x, y - 1) as PowerTransmitter;
            if (td != null) BoundedFloat.MoveVals(ref poweroutD, ref td.powerinU, poweroutD.val);

            PowerDrain dd = Terrain.TileAt(x, y - 1) as PowerDrain;
            if (dd != null) BoundedFloat.MoveVals(ref poweroutD, ref dd.powerinU, poweroutD.val);
        }
        protected void UpdateAll() {
            UpdateL();
            UpdateR();
            UpdateU();
            UpdateD();
        }

    }

    internal abstract class PowerDrain : Logic {


        protected BoundedFloat power = BoundedFloat.Zero;

        //power received
        internal BoundedFloat powerinL = BoundedFloat.Zero;
        internal BoundedFloat powerinR = BoundedFloat.Zero;
        internal BoundedFloat powerinU = BoundedFloat.Zero;
        internal BoundedFloat powerinD = BoundedFloat.Zero;

        public PowerDrain(int x, int y, TileID id) : base(x, y, id) {

        }
    }
}
