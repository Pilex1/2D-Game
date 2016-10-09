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

        private float Rotation = 0;
        private float RotationSpeed = 0.05f;

        private float MaxLife;
        private float Life;

        public Vector2 Velocity;

        private static readonly float Sqrt2 = (float)Math.Sqrt(2);

        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    Texture texture = TextureUtil.CreateTexture(new Vector3[,] {
                    {new Vector3(0, 1, 0), new Vector3(0, 0, 0)},
                    {new Vector3(0, 0, 0), new Vector3(0, 1, 0)}
                });
                    Gl.BindTexture(texture.TextureTarget, texture.TextureID);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                    Gl.BindTexture(texture.TextureTarget, 0);
                    _model =  EntityModel.CreateRectangle(new Vector2(1, 1), texture);
                }
                return _model;
            }
        }

        private static Vector2 hitboxoffset = new Vector2((1 - Sqrt2) / 2, (1 - Sqrt2) / 2);

        public Projectile(Vector2 position, Vector2 velocity, int maxlife, float rotationSpeed) : base(Model, new RectangularHitbox(position - hitboxoffset, new Vector2(Sqrt2, Sqrt2)), position) {
            base.data.speed = 0;
            base.data.jumppower = 0;
            Velocity = velocity;
            MaxLife = maxlife;
            Life = 0;
            RotationSpeed = rotationSpeed;
            base.data.CorrectCollisions = false;
        }

        public override void Update() {
            data.Position.val += Velocity * GameTime.DeltaTime;
            Hitbox.Position = data.Position.val - hitboxoffset;
            Rotation += RotationSpeed * GameTime.DeltaTime;
            if (Life > MaxLife) Entity.RemoveEntity(this);
            if (Terrain.IsColliding(this)) {
                for (int i = (int)Hitbox.Position.x; i <= (int)Math.Ceiling(Hitbox.Position.x + Hitbox.Width); i++) {
                    for (int j = (int)Hitbox.Position.y; j < (int)Math.Ceiling(Hitbox.Position.y + Hitbox.Height); j++) {
                        Terrain.BreakTile(i, j);
                    }
                }
                Entity.RemoveEntity(this);
            }
            if (Player.Intersecting(this)) {
                Player.Damage(1);
                Entity.RemoveEntity(this);
            }
            Life += GameTime.DeltaTime;
        }
    }
}
