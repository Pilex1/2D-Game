using System;
using OpenGL;

namespace Game.Entities {

    [Serializable]
    abstract class Hitbox {
        public abstract float Width { get; set; }
        public abstract float Height { get; set; }

        public Vector2 Position;
        public Vector2 Size;

        //protected static bool IntersectingCircleCircle(CircleHitbox h1, CircleHitbox h2) {
        //    float distanceX = h1.Position.x - h2.Position.x;
        //    float distanceY = h1.Position.y - h2.Position.y;
        //    float radiusSum = h1.Size.x + h2.Size.x;
        //    return distanceX * distanceX + distanceY * distanceY <= radiusSum * radiusSum;
        //}

        //protected static bool IntersectingRectCircle(RectangularHitbox h1, CircleHitbox h2) {
        //    float distanceX = Math.Abs(h2.Position.x - h1.Position.x);
        //    float distanceY = Math.Abs(h2.Position.y - h1.Position.y);

        //    if (distanceX > (h1.Width / 2 + h2.Size.x)) { return false; }
        //    if (distanceY > (h1.Height / 2 + h2.Size.x)) { return false; }

        //    if (distanceX <= (h1.Width / 2)) { return true; }
        //    if (distanceY <= (h1.Height / 2)) { return true; }

        //    float cornerDistance_sq = (distanceX - h1.Width / 2) * (distanceX - h1.Width / 2) + (distanceY - h1.Height / 2) * (distanceY - h1.Height / 2);

        //    return (cornerDistance_sq <= (h2.Size.x * h2.Size.x));
        //}

        protected static bool IntersectingRectRect(RectangularHitbox h1, RectangularHitbox h2) {
            float ax = h1.Position.x, ay = h1.Position.y, aw = h1.Size.x, ah = h1.Size.y;
            float bx = h2.Position.x, by = h2.Position.y, bw = h2.Size.x, bh = h2.Size.y;
            if (ax < bx + bw && ax + aw > bx && ay < by + bh && ay + ah > by) return true;
            return false;
        }

        public abstract bool Intersecting(Hitbox box);
    }

    [Serializable]
    class RectangularHitbox : Hitbox {

        public RectangularHitbox(Vector2 position, Vector2 size) {
            Position = position;
            Size = size;
        }

        public override float Width {
            get {
                return Size.x;
            }
            set { Size = new Vector2(value, Size.y); }
        }

        public override float Height {
            get {
                return Size.y;
            }
            set { Size = new Vector2(Size.x, value); }
        }

        public override bool Intersecting(Hitbox box) {
            if (box is RectangularHitbox) return IntersectingRectRect(this, box as RectangularHitbox);
            throw new Exception();
            //else return IntersectingRectCircle(this, box as CircleHitbox);
        }
    }
    //class CircleHitbox : Hitbox {

    //    public CircleHitbox(Vector2 position, float radius) {
    //        Position = position;
    //        Size = new Vector2(radius, radius);
    //    }

    //    public override float Width {
    //        get {
    //            return Size.x * 2;
    //        }
    //        set { Size = new Vector2(value, value); }
    //    }

    //    public override float Height {
    //        get {
    //            return Size.x * 2;
    //        }
    //        set { Size = new Vector2(value, value); }
    //    }

    //    public override bool Intersecting(Hitbox box) {
    //        if (box is CircleHitbox) return IntersectingCircleCircle(this, box as CircleHitbox);
    //        else return IntersectingRectCircle(box as RectangularHitbox, this);
    //    }
    //}
}
