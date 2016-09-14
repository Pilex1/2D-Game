using System;

namespace Game.Util {

  class MathUtil {
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
    }
}
