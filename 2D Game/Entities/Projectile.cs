using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using Game.Core;
using Game.Util;

namespace Game {

    [Serializable]
    class Projectile : Entity {

        private float rotationSpeed = 0.05f;

        private static Vector2 hitboxoffset = new Vector2((1 - MathUtil.Sqrt2) / 2, (1 - MathUtil.Sqrt2) / 2);

        public Projectile(Vector2 position, Vector2 velocity, int maxlife, float rotationSpeed) : base(EntityID.ShooterProjectile, new RectangularHitbox(position - hitboxoffset, new Vector2(MathUtil.Sqrt2, MathUtil.Sqrt2)), position) {
            data.speed = 0;
            data.jumppower = 0;
            data.life = new BoundedFloat(100, 0, 100);
            data.vel.val = velocity;
            data.useGravity = false;
            data.airResis = 1;
            this.rotationSpeed = rotationSpeed;
        }

        public override void InitTimers() {
        }

        public override void UpdateHitbox() {
            hitbox.Position = data.pos.val - hitboxoffset;
        }

        public override void Update() {
            UpdatePosition();

            data.rot += rotationSpeed * GameTime.DeltaTime;
            if (Terrain.IsColliding(this)) {
                EntityManager.RemoveEntity(this);
            }
            if (Player.Intersecting(this)) {
                Player.Instance.Damage(1);
                EntityManager.RemoveEntity(this);

            }
        }
    }
}
