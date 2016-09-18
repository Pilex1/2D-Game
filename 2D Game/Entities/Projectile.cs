using System;
using OpenGL;
using System.Diagnostics;
using Game.Entities;
using Game.Assets;
using Game.Terrains;

namespace Game {
    class Projectile : Rectangle {

        private float Rotation = 0;
        private float RotationSpeed = 0.05f;

        private float Life;

        public Vector2 Velocity;

        private static readonly float Sqrt2 = (float)Math.Sqrt(2);

        public Projectile(Vector2 position, Vector2 velocity, Vector4 colour, int life, float rotationSpeed) : base(new Vector2(1, 1), position, new Vector4[] { colour, colour, colour, colour },PolygonMode.Fill, new RectangularHitbox(position - new Vector2((1 - Sqrt2) / 2, (1 - Sqrt2) / 2), new Vector2(Sqrt2, Sqrt2)), 0) {
            Velocity = velocity;
            Life = life;
            RotationSpeed = rotationSpeed;
            CorrectCollisions = false;
        }

        public override void Update() {
            Hitbox.Position = Position;
            Position += Velocity*GameLogic.DeltaTime;
            Rotation += RotationSpeed*GameLogic.DeltaTime;
            if (Life <= 0) GameLogic.RemoveEntity(this);
            if (Terrain.IsColliding(this)) {
                for (int i = (int)Position.x; i <= (int)Math.Ceiling(Position.x + Hitbox.Width); i++) {
                    for (int j = (int)Position.y; j < (int)Math.Ceiling(Position.y + Hitbox.Height); j++) {
                        Terrain.BreakTile(i, j);
                    }
                }
                GameLogic.RemoveEntity(this);
            }
            if (Player.Intersecting(this)) {
                Player.Damage(1);
                GameLogic.RemoveEntity(this);
            }
            Life-=GameLogic.DeltaTime;
        }

        public override Matrix4 ModelMatrix() {return Matrix4.CreateRotationZ(Rotation) * Matrix4.CreateTranslation(new Vector3(Position.x, Position.y, 0));}
    }
}
