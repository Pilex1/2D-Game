using OpenGL;
using System;
using System.Linq;

namespace Game.Util {
    [Serializable]
    struct Vector2i {

        public static Vector2i Zero = new Vector2i(0, 0);

        public int x, y;
        public Vector2i(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return x + ", " + y;
        }

        public static bool operator ==(Vector2i a, Vector2i b) {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2i a, Vector2i b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            Vector2i v = (Vector2i)obj;
            return v.x == x && v.y == y;
        }

        //unique hashing for x, y < 2^16 = 65536
        public override int GetHashCode() {
            return x << 16 + y;
        }

        public float AngleTo(Vector2i src, Vector2i dest) {
            return (float)Math.Atan2(dest.y - src.y, dest.x - src.x);
        }

        public static implicit operator Vector2(Vector2i val) {
            return new Vector2(val.x, val.y);
        }
    }

    class Line {
        private double m, b;

        private Line(double m, double b) {
            this.m = m;
            this.b = b;
        }

        public static Line FromGradPoint(double m, double x, double y) {
            return new Line(m, y - m * x);
        }

        public static Line FromGradYInt(double m, double y) {
            return FromGradPoint(m, 0, y);
        }

        public static Line FromPointPoint(double x1, double y1, double x2, double y2) {
            return new Line((y2 - y1) / (x2 - x1), y1 - x1 * (y2 - y1) / (x2 - x1));
        }

        public static Line FromAnglePoint(double theta, double x, double y) {
            return new Line(Math.Tan(theta), y - Math.Tan(theta) * x);
        }

        public static Line FromAngleYInt(double theta, double y) {
            return FromAnglePoint(theta, 0, y);
        }

        public static double IntersectX(Line l1, Line l2) {
            if (l1.m == l2.m) return double.NaN;
            return (l2.b - l1.b) / (l1.m - l2.m);
        }
    }

    class Segment {
        private double x1, x2;
        private Line l;

        private Segment(Line l, double x1, double x2) {
            this.l = l;
            this.x1 = x1;
            this.x2 = x2;
        }

        public static Segment FromPointPoint(double x1, double y1, double x2, double y2) {
            return new Segment(Line.FromPointPoint(x1, y1, x2, y2), x1, x2);
        }

        public static Segment FromPointAngleLength(double x1, double y1, double theta, double length) {
            return new Segment(Line.FromAnglePoint(theta, x1, y1), x1, x1 + length + Math.Cos(theta));
        }

        public static double IntersectX(Segment s1, Segment s2) {
            double intersect = Line.IntersectX(s1.l, s2.l);
            if (intersect >= s1.x1 && intersect <= s1.x2 && intersect >= s2.x1 && intersect <= s2.x2) return intersect;
            return double.NaN;
        }

        public static bool Intersect(Segment s1, Segment s2) {
            return IntersectX(s1, s2) == double.NaN ? false : true;
        }
    }

    static class MathUtil {

        public static readonly float Sqrt2 = (float)Math.Sqrt(2);

        public const float Epsilon = 0.001f;

        #region Clamp
        public static int Clamp(int x, int min, int max) => Math.Min(Math.Max(x, min), max);
        public static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);
        public static Vector2 ClampElementWise(Vector2 v, Vector2 min, Vector2 max) => new Vector2(Clamp(v.x, min.x, max.x), Clamp(v.y, min.y, max.y));
        public static Vector2 ClampElementWise(Vector2 v, float min, float max) => ClampElementWise(v, new Vector2(min, min), new Vector2(max, max));
        public static Vector3 ClampElementWise(Vector3 v, Vector3 min, Vector3 max) => new Vector3(Clamp(v.x, min.x, max.x), Clamp(v.y, min.y, max.y), Clamp(v.z, min.z, max.z));
        public static Vector3 ClampElementWise(Vector3 v, float min, float max) => ClampElementWise(v, new Vector3(min, min, min), new Vector3(max, max, max));
        public static Vector4 ClampElementWise(Vector4 v, Vector4 min, Vector4 max) => new Vector4(Clamp(v.x, min.x, max.x), Clamp(v.y, min.y, max.y), Clamp(v.z, min.z, max.z), Clamp(v.w, min.w, max.w));
        public static Vector4 ClampElementWise(Vector4 v, float min, float max) => ClampElementWise(v, new Vector4(min, min, min, min), new Vector4(max, max, max, max));
        public static Vector2 ClampMagnitude(Vector2 v, float min, float max) {
            if (v.Length < min) v *= min / v.Length;
            if (v.Length > max) v *= max / v.Length;
            return v;
        }
        public static Vector3 ClampMagnitude(Vector3 v, float min, float max) {
            if (v.Length < min) v *= min / v.Length;
            if (v.Length > max) v *= max / v.Length;
            return v;
        }
        public static Vector4 ClampMagnitude(Vector4 v, float min, float max) {
            if (v.Length < min) v *= min / v.Length;
            if (v.Length > max) v *= max / v.Length;
            return v;
        }
        #endregion Clamp

        #region Random
        public static int RandInt(Random rand, int min, int max) {
            if (min > max) return RandInt(rand, max, min);
            return rand.Next(max - min + 1) + min;
        }

        public static float RandFloat(Random rand, float min, float max) {
            if (min > max) return RandFloat(rand, max, min);
            return (float)rand.NextDouble() * (max - min) + min;
        }
        public static float RandFloat(Random rand, double min, double max) {
            return RandFloat(rand, (float)min, (float)max);
        }

        public static double RandDouble(Random rand, double min, double max) {
            if (min > max) return RandDouble(rand, max, min);
            return rand.NextDouble() * (max - min) + min;
        }
        public static Vector2 RandVector2(Random rand, Vector2 min, Vector2 max) {
            return new Vector2(RandFloat(rand, min.x, max.x), RandFloat(rand, min.y, max.y));
        }
        public static Vector2 RandVector2(Random rand, float min, float max) {
            return RandVector2(rand, new Vector2(min, min), new Vector2(max, max));
        }
        public static Vector2 RandVector2(Random rand, double min, double max) {
            return RandVector2(rand, (float)min, (float)max);
        }
        public static Vector2i RandVector2i(Random rand, Vector2i min, Vector2i max) {
            return new Vector2i(RandInt(rand, min.x, max.x), RandInt(rand, min.y, max.y));
        }
        public static Vector3 RandVector3(Random rand, Vector3 min, Vector3 max) {
            return new Vector3(RandFloat(rand, min.x, max.x), RandFloat(rand, min.y, max.y), RandFloat(rand, min.z, max.z));
        }
        public static Vector3 RandVector3(Random rand, float min, float max) {
            return RandVector3(rand, new Vector3(min, min, min), new Vector3(max, max, max));
        }
        public static Vector3 RandVector3(Random rand, double min, double max) {
            return RandVector3(rand, (float)min, (float)max);
        }
        public static bool RandBool(Random rand) {
            return rand.NextDouble() < 0.5;
        }

        #endregion Random

        #region Swap
        public static void Swap(ref int x, ref int y) {
            int tmp = x;
            x = y;
            y = tmp;
        }
        public static void Swap(ref float x, ref float y) {
            float tmp = x;
            x = y;
            y = tmp;
        }
        public static void Swap(ref double x, ref double y) {
            double tmp = x;
            x = y;
            y = tmp;
        }
        #endregion Swap

        #region Interpolation
        public static int CatmullRomCubicInterpolate(float y0, float y1, float y2, float y3, float mu) {
            float a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = (float)(-0.5 * y0 + 1.5 * y1 - 1.5 * y2 + 0.5 * y3);
            a1 = (float)(y0 - 2.5 * y1 + 2 * y2 - 0.5 * y3);
            a2 = (float)(-0.5 * y0 + 0.5 * y2);
            a3 = y1;

            return (int)(a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }
        #endregion Interpolation

        #region Modulus
        public static int Mod(int x, int modulus) {
            return ((x %= modulus) < 0) ? x + modulus : x;
        }
        public static float Mod(float x, float modulus) {
            return x - modulus * (float)Math.Floor(x / modulus);
        }
        public static double Mod(double x, double modulus) {
            return x - modulus * Math.Floor(x / modulus);
        }
        #endregion Modulus

        public static float Max(params float[] arr) {
            return arr.Max();
        }
        public static float Min(params float[] arr) {
            return arr.Min();
        }

        internal static bool Bounded(Vector2 val, Vector2 min, Vector2 max) {
            return val.x >= min.x && val.x <= max.x && val.y >= min.y && val.y <= max.y;
        }
        internal static bool Bounded(Vector2i val, Vector2i min, Vector2i max) {
            return val.x >= min.x && val.x <= max.x && val.y >= min.y && val.y <= max.y;
        }

        public static float GetAngle(Vector2 v) {
            return (float)Math.Atan2(v.y, v.x);
        }

        public static float GetAngleFrom(Vector2 src, Vector2 dest) {
            return (float)Math.Atan2(dest.y - src.y, dest.x - src.x);
        }

        public static Vector2 Vec2FromAngle(float theta) {
            return new Vector2(Math.Cos(theta), Math.Sin(theta));
        }

        public static Matrix4 ModelMatrix(Vector2 scale, float rotation, Vector2 translation) {
            return ModelMatrix(new Vector3(scale.x, scale.y, 0), new Vector3(0, 0, rotation), new Vector3(translation.x, translation.y, 0));
        }

        public static Matrix4 ModelMatrix(Vector3 scale, Vector3 rotation, Vector3 translation) {
            return Matrix4.CreateScaling(scale) * Matrix4.CreateRotationX(rotation.x) * Matrix4.CreateRotationY(rotation.y) * Matrix4.CreateRotationZ(rotation.z) * Matrix4.CreateTranslation(translation);
        }
    }
}
