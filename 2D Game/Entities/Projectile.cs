using System;
using OpenGL;
using System.Diagnostics;
using Game.Entities;
using Game.Assets;
using Game.Terrains;
using Game.Core;
using Game.Util;
using Game.Interaction;

namespace Game {
    class Projectile : Entity {

        private static Texture texture;

        private float Rotation = 0;
        private float RotationSpeed = 0.05f;

        private float Life;

        public Vector2 Velocity;

        private static readonly float Sqrt2 = (float)Math.Sqrt(2);

        public static Texture GetTexture() {
            if (texture == null) {
                texture = TextureUtil.CreateTexture(new Vector3[,] {
                    {new Vector3(0, 1, 0), new Vector3(0, 0, 0)},
                    {new Vector3(0, 0, 0), new Vector3(0, 1, 0)}
                });
                Gl.BindTexture(texture.TextureTarget, texture.TextureID);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                Gl.BindTexture(texture.TextureTarget, 0);
            }
            return texture;
        }

        public Projectile(Vector2 position, Vector2 velocity, int life, float rotationSpeed) : base( EntityVAO.CreateRectangle(new Vector2(1, 1)), GetTexture(), new RectangularHitbox(position, new Vector2(1, 1)),position) {
            base.data.speed = 0;
            base.data.jumppower = 0;
            Velocity = velocity;
            Life = life;
            RotationSpeed = rotationSpeed;
            base.data.CorrectCollisions = false;
        }

        public override void Update() {
            Hitbox.Position = data.Position.val;
            data.Position.val += Velocity * GameLogic.DeltaTime;
            Rotation += RotationSpeed * GameLogic.DeltaTime;
            if (Life <= 0) Entity.RemoveEntity(this);
            if (Terrain.IsColliding(this)) {
                for (int i = (int)data.Position.x; i <= (int)Math.Ceiling(data.Position.x + Hitbox.Width); i++) {
                    for (int j = (int)data.Position.y; j < (int)Math.Ceiling(data.Position.y + Hitbox.Height); j++) {
                        Terrain.BreakTile(i, j);
                    }
                }
                Entity.RemoveEntity(this);
            }
            if (Player.Intersecting(this)) {
                Console.WriteLine(Healthbar.Health);
                Player.Damage(1);
                Entity.RemoveEntity(this);
            }
            Life -= GameLogic.DeltaTime;
        }

        public override Matrix4 ModelMatrix() { return Matrix4.CreateRotationZ(Rotation) * Matrix4.CreateTranslation(new Vector3(data.Position.x, data.Position.y, 0)); }

    }
}
