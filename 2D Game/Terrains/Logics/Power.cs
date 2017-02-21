using Game.Util;
using System;
using System.Linq;

namespace Game.Terrains.Logics {
    [Serializable]
    internal class Power {
        internal BoundedFloat[] power;
        internal Direction dir;

        public Power() : this(Direction.Up) { }
        public Power(Direction dir) {
            power = new BoundedFloat[4];
            this.dir = dir;
        }

        public BoundedFloat GetPower(Direction dir) => power[(int)DirectionUtil.TurnClockwise(this.dir, dir)];
        public void SetPower(Direction dir, BoundedFloat b) => power[(int)DirectionUtil.TurnClockwise(this.dir, dir)] = b;
        public void SetPower(Direction dir, float f) => power[(int)DirectionUtil.TurnClockwise(this.dir, dir)].val = f;
        public void SetPowerAll(BoundedFloat b) {
            for (int i = 0; i < 4; i++) {
                power[i] = b;
            }
        }
        public void Empty() {
            foreach (BoundedFloat b in power) {
                b.Empty();
            }
        }
        /// <summary>
        /// Gives the buffer power from all sides
        /// </summary>
        /// <param name="b"></param>
        public void GivePowerAll(ref BoundedFloat b) {
            for (int i = 0; i < 4; i++) {
                GivePower((Direction)i, ref b);
            }
        }
        /// <summary>
        /// Gives the buffer power from the given side
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="b"></param>
        public void GivePower(Direction dir, ref BoundedFloat b) => GivePower(dir, ref b, power[(int)DirectionUtil.TurnClockwise(this.dir, dir)].val);

        /// <summary>
        /// Gives the buffer power from the given side up to the given amount
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="b"></param>
        /// <param name="amt"></param>
        public void GivePower(Direction dir, ref BoundedFloat b, float amt) {
            BoundedFloat.MoveVals(ref power[(int)DirectionUtil.TurnClockwise(this.dir, dir)], ref b, amt);
        }

        /// <summary>
        /// Takes the given amount of power from the buffer and puts it into the given side
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="b"></param>
        /// <param name="amt"></param>
        public void TakePower(Direction dir, ref BoundedFloat b, float amt) {
            BoundedFloat.MoveVals(ref b, ref power[(int)DirectionUtil.TurnClockwise(this.dir, dir)], amt);
        }

        /// <summary>
        /// Takes power from the buffer and puts it into the given side
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="b"></param>
        public void TakePower(Direction dir, ref BoundedFloat b) => TakePower(dir, ref b, b.val);

        /// <summary>
        /// Takes power from the buffer and puts it into every side
        /// </summary>
        /// <param name="b"></param>
        public void TakePowerAll(ref BoundedFloat b) {
            for (int i = 0; i < 4; i++) {
                TakePower((Direction)i, ref b);
            }
        }

        public float TotalPower() => power.Sum(p => p.val);
    }

    [Serializable]
    internal class PowerCache {
        internal float[] powerCache;
        public PowerCache() {
            powerCache = new float[4];
        }
        public void CachePower(Power p) {
            for (int i = 0; i < 4; i++) {
                powerCache[(int)DirectionUtil.TurnClockwise((Direction)i, p.dir)] = p.power[i];
            }
        }
        public float GetPower(Direction dir) => powerCache[(int)dir];
        public void SetPower(Direction dir, BoundedFloat b) => powerCache[(int)dir] = b;
    }
}
