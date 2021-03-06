﻿using Pencil.Gaming.MathUtils;
using System;

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

        public static implicit operator Vector2(BoundedVector2 v) {
            return new Vector2(v.x, v.y);
        }

        public override string ToString() {
            return string.Format("{0}, {1}", bfx, bfy);
        }
    }

    [Serializable]
    struct BoundedFloat {

        public static BoundedFloat Zero = new BoundedFloat(0, 0, 0);

        private float _val;
        public float val {
            get { return _val; }
            set {
                if (value < min) value = min;
                if (value > max) value = max;
                _val = value;
            }
        }

        internal bool IsFull() {
            return val == max;
        }

        public float min, max;

        public BoundedFloat(float max) : this(0, 0, max) { }

        public BoundedFloat(float val, float min, float max) {
            _val = val;
            this.min = min;
            this.max = max;
        }

        public static BoundedFloat operator +(BoundedFloat a, float b) {
            a.val += b;
            return a;
        }

        public static BoundedFloat operator -(BoundedFloat a, float b) {
            a.val -= b;
            return a;
        }
        public static BoundedFloat operator *(BoundedFloat a, float b) {
            a.val *= b;
            return a;
        }
        public static BoundedFloat operator /(BoundedFloat a, float b) {
            a.val /= b;
            return a;
        }


        public float GetSpace() {
            return max - val;
        }

        public float GetFilledRatio() {
            return val / max;
        }

        /// <summary>
        /// Fills value to maximum capacity
        /// </summary>
        public void Fill() {
            val = max;
        }

        /// <summary>
        /// EMpties value to minmum capacity
        /// </summary>
        public void Empty() {
            val = min;
        }

        /// <summary>
        /// Moves up to val from source to the destination
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="val"></param>
        public static void MoveVals(ref BoundedFloat src, ref BoundedFloat dest, float val) {
            if (val > src._val) val = src._val;
            if (dest._val + val > dest.max) val = dest.max - dest._val;

            dest._val += val;
            src._val -= val;
        }

        /// <summary>
        /// Moves as much values from the source to the destination
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void MoveVals(ref BoundedFloat src, ref BoundedFloat dest) => MoveVals(ref src, ref dest, src.val);

        public static implicit operator float(BoundedFloat f) {
            return f.val;
        }

        public override string ToString() {
            return val.ToString();
        }

        internal bool IsEmpty() {
            return val == min;
        }
    }
}
