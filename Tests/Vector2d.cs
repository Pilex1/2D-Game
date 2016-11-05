using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {

    [Serializable]
    struct Vector2d {
        public static Vector2d Zero = new Vector2d(0, 0);
        public double x, y;
        public Vector2d(double x, double y) {
            this.x = x;
            this.y = y;
        }
    }
}
