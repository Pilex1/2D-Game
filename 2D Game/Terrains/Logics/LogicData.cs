using Game.Items;
using Game.Terrains.Lighting;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;
using System.Text;

namespace Game.Terrains.Logics {


    [Serializable]
    abstract class LogicAttribs : TileAttribs {

        protected Power powerOut;
        protected Power powerIn;

        protected PowerCache powerOutCache;
        protected PowerCache powerInCache;

        protected LogicAttribs(Func<RawItem> dropItem) : base(dropItem) {
            powerOut = new Power();
            powerIn = new Power();

            powerOutCache = new PowerCache();
            powerInCache = new PowerCache();
        }

        private void TransferPower(int x, int y, Direction d, float amt) {

            //direction of current block with rotation taken into account
            Direction d_cur = DirectionUtil.TurnClockwise(d, rotation);

            //direction of input into neighbouring block (opposite current rotation)
            Direction d_opp_cur = DirectionUtil.TurnClockwise(d, Direction.Down);

            Vector2i v_cur = DirectionUtil.ToVector2i(d_cur);
            //tile of neighbouring block
            Tile t_other = Terrain.TileAt(new Vector2i(x, y) + v_cur);

            //direction of input into neighbouring block with its rotation taken into account
            Direction d_other = DirectionUtil.TurnClockwise(t_other.tileattribs.rotation, d_opp_cur);

            PowerTransmitter transmitter = t_other.tileattribs as PowerTransmitter;
            if (transmitter != null) powerOut.GivePower(d_cur, ref transmitter.powerIn.power[(int)d_other], amt);

            PowerDrain drain = t_other.tileattribs as PowerDrain;
            if (drain != null) powerOut.GivePower(d_cur, ref drain.powerIn.power[(int)d_other], amt);
        }

        protected void TransferPower(int x, int y, ref BoundedFloat buffer, params Direction[] sides) {
            int c = CountTransmitters(x, y, sides) + CountDrains(x, y, sides);
            if (c == 0) return;
            float amt = buffer.val / c;
            foreach (Direction d in sides) {
                powerOut.TakePower(d, ref buffer, amt);
                TransferPower(x, y, d, amt);
            }
        }

        protected void TransferPowerAll(int x, int y, ref BoundedFloat buffer) => TransferPower(x, y, ref buffer, Direction.Up, Direction.Right, Direction.Down, Direction.Left);

        private TileAttribs GetAttribs(Vector2i vi, Direction d) {
            Vector2i v = DirectionUtil.ToVector2i(d);
            return Terrain.TileAt(vi + v).tileattribs;
        }
        private TileAttribs GetAttribs(int x, int y, Direction d) => GetAttribs(new Vector2i(x, y), d);

        protected bool IsLogic(Vector2i v, Direction d) => GetAttribs(v, d) is LogicAttribs;
        protected bool IsLogic(int x, int y, Direction d) => IsLogic(new Vector2i(x, y), d);

        protected bool IsSource(Vector2i v, Direction d) => GetAttribs(v, d) is PowerSource;
        protected bool IsSource(int x, int y, Direction d) => IsSource(new Vector2i(x, y), d);

        protected bool IsTransmitter(Vector2i v, Direction d) => GetAttribs(v, d) is PowerTransmitter;
        protected bool IsTransmitter(int x, int y, Direction d) => IsTransmitter(new Vector2i(x, y), d);

        protected bool IsDrain(Vector2i v, Direction d) => GetAttribs(v, d) is PowerDrain;
        protected bool IsDrain(int x, int y, Direction d) => IsDrain(new Vector2i(x, y), d);

        private int Count(Func<Vector2i, Direction, bool> pred, Vector2i v, params Direction[] dirs) {
            int c = 0;
            foreach (Direction dir in dirs) {
                c += pred(v, dir) ? 1 : 0;
            }
            return c;
        }

        protected int CountNeighbouringLogic(Vector2i v) => CountLogics(v, Direction.Up, Direction.Right, Direction.Down, Direction.Left);
        protected int CountNeighbouringLogic(int x, int y) => CountNeighbouringLogic(new Vector2i(x, y));
        protected int CountLogics(int x, int y, params Direction[] dirs) => CountLogics(new Vector2i(x, y), dirs);
        protected int CountLogics(Vector2i v, params Direction[] dirs) => Count(IsLogic, v, dirs);

        protected int CountNeighbouringSources(Vector2i v) => CountSources(v, Direction.Up, Direction.Right, Direction.Down, Direction.Left);
        protected int CountNeighbouringSources(int x, int y) => CountNeighbouringSources(new Vector2i(x, y));
        protected int CountSources(int x, int y, params Direction[] dirs) => CountSources(new Vector2i(x, y), dirs);
        protected int CountSources(Vector2i v, params Direction[] dirs) => Count(IsSource, v, dirs);

        protected int CountNeighbouringTransmitters(Vector2i v) => CountTransmitters(v, Direction.Up, Direction.Right, Direction.Down, Direction.Left);
        protected int CountNeighbouringTransmitters(int x, int y) => CountNeighbouringTransmitters(new Vector2i(x, y));
        protected int CountTransmitters(int x, int y, params Direction[] dirs) => CountTransmitters(new Vector2i(x, y), dirs);
        protected int CountTransmitters(Vector2i v, params Direction[] dirs) => Count(IsTransmitter, v, dirs);

        protected int CountNeighbouringDrains(Vector2i v) => CountDrains(v, Direction.Up, Direction.Right, Direction.Down, Direction.Left);
        protected int CountNeighbouringDrains(int x, int y) => CountNeighbouringDrains(new Vector2i(x, y));
        protected int CountDrains(int x, int y, params Direction[] dirs) => CountDrains(new Vector2i(x, y), dirs);
        protected int CountDrains(Vector2i v, params Direction[] dirs) => Count(IsDrain, v, dirs);

        protected virtual void EmptyInputs() => powerIn.Empty();
        protected virtual void EmptyOutputs() => powerOut.Empty();

        protected virtual void CacheInputs() => powerInCache.CachePower(powerIn);
        protected virtual void CacheOutputs() => powerOutCache.CachePower(powerOut);

        protected void UpdateMultiLight(int x, int y, int newState, IMultiLight light) {
            if (newState == light.State) return;
            LightingManager.RemoveLight(x, y, light.Lights()[light.State]);
            light.State = newState;
            LightingManager.AddLight(x, y, light.Lights()[light.State]);
        }

        internal void Update(int x, int y) {
            powerOut.dir = powerIn.dir = rotation;
            UpdateMechanics(x, y);
        }

        protected abstract void UpdateMechanics(int x, int y);

        public abstract override string ToString();
    }


    [Serializable]
    internal abstract class PowerSource : LogicAttribs {

        protected PowerSource(Func<RawItem> dropItem) : base(dropItem) {
            powerIn.SetPowerAll(BoundedFloat.Zero);
        }

        protected sealed override void EmptyInputs() { }
        protected sealed override void CacheInputs() { }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Left: Out {0}", powerOutCache.GetPower(Direction.Left)));
            sb.AppendLine(string.Format("Right: Out {0}", powerOutCache.GetPower(Direction.Right)));
            sb.AppendLine(string.Format("Up: Out {0}", powerOutCache.GetPower(Direction.Up)));
            sb.AppendLine(string.Format("Down: Out {0}", powerOutCache.GetPower(Direction.Down)));
            return sb.ToString();
        }

    }

    [Serializable]
    internal abstract class PowerTransmitter : LogicAttribs {

        protected float dissipate = 1;

        protected PowerTransmitter(Func<RawItem> dropItem) : base(dropItem) {

        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Left: In {0} / Out {1}", powerInCache.GetPower(Direction.Left), powerOutCache.GetPower(Direction.Left)));
            sb.AppendLine(string.Format("Right: In {0} / Out {1}", powerInCache.GetPower(Direction.Right), powerOutCache.GetPower(Direction.Right)));
            sb.AppendLine(string.Format("Up: In {0} / Out {1}", powerInCache.GetPower(Direction.Up), powerOutCache.GetPower(Direction.Up)));
            sb.AppendLine(string.Format("Down: In {0} / Out {1}", powerInCache.GetPower(Direction.Down), powerOutCache.GetPower(Direction.Down)));
            return sb.ToString();
        }
    }

    [Serializable]
    internal abstract class PowerDrain : LogicAttribs {

        protected float cost = 0;

        protected PowerDrain(Func<RawItem> dropItem) : base(dropItem) {
            powerOut.SetPowerAll(BoundedFloat.Zero);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Required: {0}", cost));
            sb.AppendLine(string.Format("Left: In {0}", powerInCache.GetPower(Direction.Left)));
            sb.AppendLine(string.Format("Right: In {0}", powerInCache.GetPower(Direction.Right)));
            sb.AppendLine(string.Format("Up: In {0}", powerInCache.GetPower(Direction.Up)));
            sb.AppendLine(string.Format("Down: In {0}", powerInCache.GetPower(Direction.Down)));
            return sb.ToString();
        }

    }

}
