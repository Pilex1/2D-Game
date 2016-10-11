using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool operator==(Vector2i a, Vector2i b) {
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
}
