using System;
using OpenGL;
using Game.Terrains;
using Game.Core;
using Game.Entities;
using Game.Util;

namespace Game {

    [Serializable]
    class Shooter : Entity {

        private CooldownTimer shootCooldown;

        private int projlife;

        public Shooter(Vector2 position, int shootCooldown, int projlife) : base(EntityID.Shooter, position) {
            data.speed = 0;
            data.jumppower = 0;
            data.useGravity = true;
            data.life = new BoundedFloat(10, 0, 10);
            this.shootCooldown = new CooldownTimer(shootCooldown);
            this.projlife = projlife;
        }

        public override void InitTimers() {
            CooldownTimer.AddTimer(shootCooldown);
        }

        public override void UpdateHitbox() {
            hitbox.Position = data.pos.val;
        }

        public override void Update() {
            UpdatePosition();
            if (!shootCooldown.Ready())
                return;

            shootCooldown.Reset();
            Vector2 vel = Player.ToPlayer(data.pos.val);
            vel.x += MathUtil.RandFloat(Program.Rand, -0.1, 0.1);
            vel /= 5;
            Projectile proj = new Projectile(data.pos.val, vel, projlife, 0.01f);

            if (!Terrain.IsColliding(proj)) EntityManager.AddEntity(proj);
        }
    }
}
