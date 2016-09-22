using System;
using OpenGL;
using Game.Terrains;

namespace Game {
    class Shooter : Rectangle {

        private int ShootCooldown;
        private float ShootCooldownTime=0;

        private int ProjectileLife;

        public Shooter(int shootCooldown, int projectileLife) : this(Vector2.Zero, shootCooldown, projectileLife) { }

        public Shooter(Vector2 position, int shootCooldown, int projectileLife) : base(new Vector2(1, 2), position,new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 0, 1, 1) }, PolygonMode.Fill, 0) {
            ShootCooldown = shootCooldown;
            ProjectileLife = projectileLife;
        }

        public override void Update() {
            if (ShootCooldownTime >= ShootCooldown) {
                Projectile proj = new Projectile(Position, (Player.ToPlayer(Position)) / 5, new Vector4(0, 1, 0, 1), ProjectileLife,0.05f);
                proj.Position += proj.Velocity*GameLogic.DeltaTime;
                if (!Terrain.IsColliding(proj)) GameLogic.AddEntity(proj);
                ShootCooldownTime = 0;
            } else ShootCooldownTime+=GameLogic.DeltaTime;
            Update();
        }
    }
}
