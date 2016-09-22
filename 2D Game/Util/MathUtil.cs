using System;

namespace Game.Util {

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

    class MathUtil {
        #region Clamp
        public static void Clamp(ref int x, int min, int max) {
            if (x < min) x = min;
            if (x > max) x = max;
        }
        public static void ClampMin(ref int x, int min) {
            Clamp(ref x, min, int.MaxValue);
        }
        public static void ClampMax(ref int x, int max) {
            Clamp(ref x, int.MinValue, max);
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

        public static double RandDouble(Random rand, double min, double max) {
            if (min > max) return RandDouble(rand, max, min);
            return rand.NextDouble() * (max - min) + min;
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
    }
}
