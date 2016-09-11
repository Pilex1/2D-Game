using System;

namespace Game.Util {

    [Serializable]
    public struct Vector2i : IEquatable<Vector2i> {
        public int x, y;

        #region Static Constructors
        public static Vector2i Identity {
            get { return new Vector2i(1, 1); }
        }

        public static Vector2i Zero {
            get { return new Vector2i(0, 0); }
        }
        #endregion

        #region Operators
        public static Vector2i operator +(Vector2i v1, Vector2i v2) {
            return new Vector2i(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2i operator +(Vector2i v, int s) {
            return new Vector2i(v.x + s, v.y + s);
        }

        public static Vector2i operator +(int s, Vector2i v) {
            return new Vector2i(v.x + s, v.y + s);
        }

        public static Vector2i operator -(Vector2i v1, Vector2i v2) {
            return new Vector2i(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2i operator -(Vector2i v, int s) {
            return new Vector2i(v.x - s, v.y - s);
        }

        public static Vector2i operator -(int s, Vector2i v) {
            return new Vector2i(s - v.x, s - v.y);
        }

        public static Vector2i operator -(Vector2i v) {
            return new Vector2i(-v.x, -v.y);
        }

        public static Vector2i operator *(Vector2i v1, Vector2i v2) {
            return new Vector2i(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2i operator *(int s, Vector2i v) {
            return new Vector2i(v.x * s, v.y * s);
        }

        public static Vector2i operator *(Vector2i v, int s) {
            return new Vector2i(v.x * s, v.y * s);
        }

        public static Vector2i operator /(Vector2i v1, Vector2i v2) {
            return new Vector2i(v1.x / v2.x, v1.y / v2.y);
        }

        public static Vector2i operator /(int s, Vector2i v) {
            return new Vector2i(s / v.x, s / v.y);
        }

        public static Vector2i operator /(Vector2i v, int s) {
            return new Vector2i(v.x / s, v.y / s);
        }

        public static bool operator ==(Vector2i v1, Vector2i v2) {
            return (v1.x == v2.x && v1.y == v2.y);
        }

        public static bool operator !=(Vector2i v1, Vector2i v2) {
            return (v1.x != v2.x || v1.y != v2.y);
        }
        #endregion

        #region Constructors
        public Vector2i(int x, int y) {
            this.x = x; this.y = y;
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj) {
            if (!(obj is Vector2i)) return false;

            return this.Equals((Vector2i)obj);
        }

        public bool Equals(Vector2i other) {
            return this == other;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return "{" + x + ", " + y + "}";
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get the length of the Vector2 structure.
        /// </summary>
        public float Length {
            get { return (float)Math.Sqrt(x * x + y * y); }
        }

        /// <summary>
        /// Get the squared length of the Vector2 structure.
        /// </summary>
        public float SquaredLength {
            get { return x * x + y * y; }
        }

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public Vector2i PerpendicularRight {
            get { return new Vector2i(y, -x); }
        }

        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public Vector2i PerpendicularLeft {
            get { return new Vector2i(-y, x); }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Performs the Vector2 scalar dot product.
        /// </summary>
        /// <param name="v1">The left Vector2.</param>
        /// <param name="v2">The right Vector2.</param>
        /// <returns>v1.x * v2.x + v1.y * v2.y</returns>
        public static float Dot(Vector2i v1, Vector2i v2) {
            return v1.x * v2.x + v1.y * v2.y;
        }

        /// <summary>
        /// Performs the Vector2 scalar dot product.
        /// </summary>
        /// <param name="v">Second dot product term</param>
        /// <returns>Vector2.Dot(this, v)</returns>
        public float Dot(Vector2i v) {
            return this.Dot(v);
        }
        
        /// <summary>
        /// Store the minimum values of x, and y between the two vectors.
        /// </summary>
        /// <param name="v">Vector to check against</param>
        public void TakeMin(Vector2i v) {
            if (v.x < x) x = v.x;
            if (v.y < y) y = v.y;
        }

        /// <summary>
        /// Store the maximum values of x, and y between the two vectors.
        /// </summary>
        /// <param name="v">Vector to check against</param>
        public void TakeMax(Vector2i v) {
            if (v.x > x) x = v.x;
            if (v.y > y) y = v.y;
        }
        #endregion
    }
}
