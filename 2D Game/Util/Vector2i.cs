using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Util {

    [Serializable]
    struct Vector2i {
        public int x, y;
        public Vector2i(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return x + ", " + y;
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
    }
}
