using System;

namespace Game.Util {

    struct BoundedFloat {

        public static BoundedFloat Zero = new BoundedFloat(0, 0, 0);

        public static float Epsilon = 0.00001f;

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
    }
}
