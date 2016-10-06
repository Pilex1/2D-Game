using OpenGL;
using System;
using System.Linq;

namespace Game.Util {

    [Serializable]
    struct BoundedVector2 {

        private BoundedFloat bfx, bfy;

        public float x {
            get { return bfx.val; }
            set { bfx.val = value; }
        }

        public float y {
            get { return bfy.val; }
            set { bfy.val = value; }
        }

        public Vector2 val {
            get { return new Vector2(bfx.val, bfy.val); }
            set { bfx.val = value.x; bfy.val = value.y; }
        }

        public BoundedVector2(float val1, float min1, float max1, float val2, float min2, float max2) : this(new BoundedFloat(val1, min1, max1), new BoundedFloat(val2, min2, max2)) { }

        public BoundedVector2(BoundedFloat bfx, BoundedFloat bfy) {
            this.bfx = bfx;
            this.bfy = bfy;
        }
    }

    [Serializable]
    struct BoundedFloat {

        public static BoundedFloat Zero = new BoundedFloat(0, 0, 0);

        private static float Epsilon = 0.00001f;

        private float _val;
        public float val {
            get { return _val; }
            set {
                if (Math.Abs(value) < Epsilon) value = 0;
                _val = value;
            }
        }
        public float min, max;

        public BoundedFloat(float val, float min, float max) {
            if (Math.Abs(val) < Epsilon) val = 0;
            _val = val;
            this.min = min;
            this.max = max;
            if (val < min || val > max) throw new ArgumentException();
        }

        public float Space() {
            return max - val;
        }

        public float FilledRatio() {
            return val / max;
        }

        public void Fill() {
            val = max;
        }

        public void Empty() {
            val = min;
        }

        public static void MoveVals(ref BoundedFloat src, ref BoundedFloat dest, float val) {
            dest.val += val;
            src.val -= val;
            if (dest.val > dest.max) {
                src.val += (dest.val - dest.max);
                dest.val = dest.max;
            }
            if (dest.val < dest.min) {
                src.val += (dest.min - dest.val);
                dest.val = dest.min;
            }
            if (src.val > src.max) {
                dest.val += (src.val - src.max);
                src.val = src.max;
            }
            if (src.val < src.min) {
                dest.val += (src.min - src.val);
                src.val = src.min;
            }
        }


        public override string ToString() {
            return val.ToString() + " : [" + min + ", " + max + "]";
        }
    }
}
