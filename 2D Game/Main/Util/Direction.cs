﻿using Pencil.Gaming.MathUtils;
using System;

namespace Game.Util {

    [Serializable]
    enum Direction {
        Up, Right, Down, Left
    }

    static class DirectionUtil {
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

        public static Direction FromVector2(Vector2 v) {
            float angle = MathUtil.GetAngle(v);
            if (angle >= Math.PI / 4 && angle <= Math.PI * 3 / 4)
                return Direction.Up;
            if (angle >= Math.PI * 3 / 4 || angle <= -Math.PI * 3 / 4)
                return Direction.Left;
            if (angle >= -Math.PI * 3 / 4 && angle <= -Math.PI / 4)
                return Direction.Down;
            if (angle <= Math.PI / 4 && angle >= -Math.PI / 4)
                return Direction.Right;
            throw new ArgumentException();
        }

        public static Vector2i ToVector2i(Direction d) {
            switch (d) {
                case Direction.Up:
                    return new Vector2i(0, 1);
                case Direction.Right:
                    return new Vector2i(1, 0);
                case Direction.Down:
                    return new Vector2i(0, -1);
                case Direction.Left:
                    return new Vector2i(-1, 0);
            }
            throw new ArgumentException();
        }
    }
}
