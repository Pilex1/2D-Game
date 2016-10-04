using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Util {

    [Serializable]
    enum Direction {
        Up, Right, Down, Left
    }
    static class DirectionHelper {
        public static Direction TurnClockwise(Direction dir, Direction amt) {
            int res = (int)dir + (int)amt;
            res %= 4;
            return (Direction)res;
        }
        public static Direction TurnAntiClockwise(Direction dir, Direction amt) {
            int res = (int)dir - (int)amt;
            res = MathUtil.Mod(res, 4);
            return (Direction)res;
        }
    }
}
