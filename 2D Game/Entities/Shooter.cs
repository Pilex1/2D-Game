using System;
using OpenGL;
using Game.Terrains;
using Game.Core;
using Game.Entities;
using Game.Util;

namespace Game {
    class Shooter : Entity {

        private int ShootCooldown;
        private float ShootCooldownTime = 0;

        private int ProjectileLife;

        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    Texture texture = TextureUtil.CreateTexture(new Vector3[,] {
                        {new Vector3(1, 0, 0), new Vector3(1, 0, 0)},
                        {new Vector3(0, 0, 1), new Vector3(0, 0, 1)}
                    });
                    Gl.BindTexture(texture.TextureTarget, texture.TextureID);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                    Gl.BindTexture(texture.TextureTarget, 0);
                    _model = EntityModel.CreateRectangle(new Vector2(1, 2), texture);
                }
                return _model;
            }
        }

        public Shooter(Vector2 position, int shootCooldown, int projectileLife) : base(Model, new RectangularHitbox(position, new Vector2(1, 2)), position) {
            base.data.speed = 0;
            base.data.jumppower = 0;
            base.data.UseGravity = true;
            ShootCooldown = shootCooldown;
            ProjectileLife = projectileLife;
            CorrectTerrainCollision();
        }

        public override void Update() {
            base.UpdatePosition();
            if (ShootCooldownTime >= ShootCooldown) {
                Projectile proj = new Projectile(data.Position.val, Player.ToPlayer(data.Position.val) / 5, ProjectileLife, 0.01f);
                if (!Terrain.IsColliding(proj)) Entity.AddEntity(proj);
                ShootCooldownTime = 0;
            } else ShootCooldownTime += GameTime.DeltaTime;
            Hitbox.Position = data.Position.val;
        }
    }
}
