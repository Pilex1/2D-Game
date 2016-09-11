using System;
using OpenGL;
using System.Diagnostics;
using Game.Entities;

namespace Game {
    class Projectile : Rectangle {

        private float Rotation = 0;
        private float RotationSpeed = 0.05f;

        private int Life;

        public Vector2 Velocity;

        private static readonly float Sqrt2 = (float)Math.Sqrt(2);

        public Projectile(Vector2 position, Vector2 velocity, Vector4 colour, int life) : base(new Vector2(1, 1), position, new Vector4[] { colour, colour, colour, colour }, new RectangularHitbox(position-new Vector2((1-Sqrt2)/2, (1 - Sqrt2) / 2), new Vector2(Sqrt2, Sqrt2))) {
            Velocity = velocity;
            Life = life;
            CorrectCollisions = false;
        }

        public override void Update() {
            Position += Velocity;
            Rotation += RotationSpeed;
            if (Life == 0) GameLogic.RemoveEntity(this);
            if (Terrain.IsColliding(this)) {
                Terrain.SetTile((int)Position.x,(int)Position.y, Tile.Air);
                GameLogic.RemoveEntity(this);
            }
            if (Player.Intersecting(this)) {
                Player.Damage(1);
                GameLogic.RemoveEntity(this);
            }
            Life--;
        }

        public override Matrix4 ModelMatrix() => Matrix4.CreateRotationZ(Rotation) * Matrix4.CreateTranslation(new Vector3(Position.x, Position.y, 0));
    }
}
