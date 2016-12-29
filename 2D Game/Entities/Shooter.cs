using System;
using OpenGL;
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
            data.life = new BoundedFloat(50, 0, 50);
            this.shootCooldown = new CooldownTimer(shootCooldown);
            this.projlife = projlife;
        }

        public Shooter(Vector2 position) : this(position, 100, 250) { }

        public override void InitTimers() {
            CooldownTimer.AddTimer(shootCooldown);
        }

        public override void Update() {
            UpdatePosition();
            if (!shootCooldown.Ready())
                return;

            shootCooldown.Reset();
            Vector2 vel = Player.ToPlayer(data.pos.val);
            vel.x += MathUtil.RandFloat(Program.Rand, -0.1, 0.1);
            vel /= 5;
            Projectile proj = new Projectile(data.pos.val, vel, projlife);

            if (!proj.Colliding()) EntityManager.AddEntity(proj);
        }
    }
}
