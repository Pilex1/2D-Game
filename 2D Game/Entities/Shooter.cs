using System;
using OpenGL;
using Game.Terrains;
using Game.Core;
using Game.Entities;
using Game.Util;

namespace Game {
    class Shooter : Entity {

        private static Texture texture;

        private int ShootCooldown;
        private float ShootCooldownTime = 0;

        private int ProjectileLife;

        private static Texture GetTexture() {
            if (texture == null) {
                texture = TextureUtil.CreateTexture(new Vector3[,] {
                    {new Vector3(1, 0, 0), new Vector3(1,0,0)},
                    {new Vector3(0, 0, 1), new Vector3(0, 0, 1)}
                });
                Gl.BindTexture(texture.TextureTarget, texture.TextureID);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            }
            return texture;
        }

        public Shooter(Vector2 position, int shootCooldown, int projectileLife) : base(EntityVAO.CreateRectangle(new Vector2(1, 2)), GetTexture(), new RectangularHitbox(position, new Vector2(1, 2)),position) {
            base.data.speed = 0;
            base.data.jumppower = 0;
            ShootCooldown = shootCooldown;
            ProjectileLife = projectileLife;
        }

        public override void Update() {
            if (ShootCooldownTime >= ShootCooldown) {
                Projectile proj = new Projectile(data.Position.val, (Player.ToPlayer(data.Position.val)) / 5, ProjectileLife, 0.05f);
                proj.data.Position.val += proj.Velocity * GameLogic.DeltaTime;
                if (Terrain.IsColliding(proj)) Entity.RemoveEntity(proj);
                ShootCooldownTime = 0;
            } else ShootCooldownTime += GameLogic.DeltaTime;
            Update();
        }
    }
}
