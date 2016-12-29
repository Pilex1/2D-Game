using Game.Items;
using Game.Util;
using System;
using System.Text;

namespace Game.Terrains.Logics {

    [Serializable]
    abstract class LogicAttribs : TileAttribs {

        protected LogicAttribs(Func<RawItem> dropItem) : base(dropItem) { }

        internal abstract void Update(int x, int y);
    }

    [Serializable]
    internal abstract class PowerSourceData : LogicAttribs {

        protected PowerSourceData(Func<RawItem> dropItem) : base(dropItem) { }

        //power available to draw from each side
        protected BoundedFloat poweroutL = BoundedFloat.Zero;
        protected BoundedFloat poweroutR = BoundedFloat.Zero;
        protected BoundedFloat poweroutU = BoundedFloat.Zero;
        protected BoundedFloat poweroutD = BoundedFloat.Zero;

    }

    [Serializable]
    internal abstract class PowerTransmitterData : LogicAttribs {

        protected PowerTransmitterData(Func<RawItem> dropItem) : base(dropItem) { }

        //power on each side
        protected BoundedFloat poweroutL = BoundedFloat.Zero;
        protected BoundedFloat poweroutR = BoundedFloat.Zero;
        protected BoundedFloat poweroutU = BoundedFloat.Zero;
        protected BoundedFloat poweroutD = BoundedFloat.Zero;

        internal BoundedFloat powerinL = BoundedFloat.Zero;
        internal BoundedFloat powerinR = BoundedFloat.Zero;
        internal BoundedFloat powerinU = BoundedFloat.Zero;

        protected float powerInLCache, powerInRCache, powerInUCache, powerInDCache;
        protected float powerOutLCache, powerOutRCache, powerOutUCache, powerOutDCache;

        internal void EmptyInputs() {
            powerinL.Empty();
            powerinR.Empty();
            powerinU.Empty();
            powerinD.Empty();
        }

        internal void EmptyOutputs() {
            poweroutL.Empty();
            poweroutR.Empty();
            poweroutU.Empty();
            poweroutD.Empty();
        }

        internal BoundedFloat powerinD = BoundedFloat.Zero;

        protected float dissipate = 1;

        protected void UpdateL(int x, int y) {
            PowerTransmitterData tl = Terrain.TileAt(x - 1, y).tileattribs as PowerTransmitterData;
            if (tl != null) BoundedFloat.MoveVals(ref poweroutL, ref tl.powerinR, poweroutL.val);

            PowerDrainData dl = Terrain.TileAt(x - 1, y).tileattribs as PowerDrainData;
            if (dl != null) BoundedFloat.MoveVals(ref poweroutL, ref dl.powerinR, poweroutL.val);
        }
        protected void UpdateR(int x, int y) {
            PowerTransmitterData tr = Terrain.TileAt(x + 1, y).tileattribs as PowerTransmitterData;
            if (tr != null) BoundedFloat.MoveVals(ref poweroutR, ref tr.powerinL, poweroutR.val);

            PowerDrainData dr = Terrain.TileAt(x + 1, y).tileattribs as PowerDrainData;
            if (dr != null) BoundedFloat.MoveVals(ref poweroutR, ref dr.powerinL, poweroutR.val);
        }
        protected void UpdateU(int x, int y) {
            PowerTransmitterData tu = Terrain.TileAt(x, y + 1).tileattribs as PowerTransmitterData;
            if (tu != null) BoundedFloat.MoveVals(ref poweroutU, ref tu.powerinD, poweroutU.val);

            PowerDrainData du = Terrain.TileAt(x, y + 1).tileattribs as PowerDrainData;
            if (du != null) BoundedFloat.MoveVals(ref poweroutU, ref du.powerinD, poweroutU.val);
        }
        protected void UpdateD(int x, int y) {
            PowerTransmitterData td = Terrain.TileAt(x, y - 1).tileattribs as PowerTransmitterData;
            if (td != null) BoundedFloat.MoveVals(ref poweroutD, ref td.powerinU, poweroutD.val);

            PowerDrainData dd = Terrain.TileAt(x, y - 1).tileattribs as PowerDrainData;
            if (dd != null) BoundedFloat.MoveVals(ref poweroutD, ref dd.powerinU, poweroutD.val);
        }
        protected void UpdateAll(int x, int y) {
            UpdateL(x, y);
            UpdateR(x, y);
            UpdateU(x, y);
            UpdateD(x, y);
        }

        protected bool IsLogicL(int x, int y) {
            if (Terrain.TileAt(x - 1, y).tileattribs as PowerTransmitterData != null) return true;
            if (Terrain.TileAt(x - 1, y).tileattribs as PowerDrainData != null) return true;
            return false;
        }

        protected bool IsLogicR(int x, int y) {
            if (Terrain.TileAt(x + 1, y).tileattribs as PowerTransmitterData != null) return true;
            if (Terrain.TileAt(x + 1, y).tileattribs as PowerDrainData != null) return true;
            return false;
        }

        protected bool IsLogicU(int x, int y) {
            if (Terrain.TileAt(x, y + 1).tileattribs as PowerTransmitterData != null) return true;
            if (Terrain.TileAt(x, y + 1).tileattribs as PowerDrainData != null) return true;
            return false;
        }

        protected bool IsLogicD(int x, int y) {
            if (Terrain.TileAt(x, y - 1).tileattribs as PowerTransmitterData != null) return true;
            if (Terrain.TileAt(x, y - 1).tileattribs as PowerDrainData != null) return true;
            return false;
        }

        protected int NeighbouringLogics(int x, int y) {
            int count = 0;

            count += IsLogicL(x, y) ? 1 : 0;
            count += IsLogicR(x, y) ? 1 : 0;
            count += IsLogicU(x, y) ? 1 : 0;
            count += IsLogicD(x, y) ? 1 : 0;

            return count;
        }
    }

    [Serializable]
    internal abstract class PowerDrainData : LogicAttribs {

        protected PowerDrainData(Func<RawItem> dropItem) : base(dropItem) { }

        //power received
        internal BoundedFloat powerinL = BoundedFloat.Zero;
        internal BoundedFloat powerinR = BoundedFloat.Zero;
        internal BoundedFloat powerinU = BoundedFloat.Zero;
        internal BoundedFloat powerinD = BoundedFloat.Zero;

        protected float cost = 0;

        protected float powerInLCache, powerInRCache, powerInUCache, powerInDCache;

        internal void EmptyInputs() {
            powerinL.Empty();
            powerinR.Empty();
            powerinU.Empty();
            powerinD.Empty();
        }

        protected void CachePowerLevels() {
            powerInLCache = powerinL.val;
            powerInRCache = powerinR.val;
            powerInUCache = powerinU.val;
            powerInDCache = powerinD.val;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Required: {0}", cost));
            if (powerinL.max > 0)
                sb.AppendLine(string.Format("Left: In {0}", powerInLCache));
            if (powerinR.max > 0)
                sb.AppendLine(string.Format("Right: In {0}", powerInRCache));
            if (powerinU.max > 0)
                sb.AppendLine(string.Format("Up: In {0}", powerInUCache));
            if (powerinD.max > 0)
                sb.AppendLine(string.Format("Down: In {0}", powerInDCache));
            return sb.ToString();
        }

    }

}
