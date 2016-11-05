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

        private float RotationSpeed = 0.05f;

        private static readonly float Sqrt2 = (float)Math.Sqrt(2);

        private static Vector2 hitboxoffset = new Vector2((1 - Sqrt2) / 2, (1 - Sqrt2) / 2);

        public Projectile(Vector2 position, Vector2 velocity, int maxlife, float rotationSpeed) : base(EntityID.ShooterProjectile, new RectangularHitbox(position - hitboxoffset, new Vector2(Sqrt2, Sqrt2)), position) {
            base.data.speed = 0;
            base.data.jumppower = 0;
            base.data.life = new BoundedFloat(5, 0, 5);
            base.data.vel.val = velocity;
            base.data.UseGravity = false;
            base.data.AirResis = 1;
            RotationSpeed = rotationSpeed;
            base.data.CorrectCollisions = false;
        }

        public override void Update() {
            base.UpdatePosition();
            Hitbox.Position = data.Position.val - hitboxoffset;
            base.data.rot += RotationSpeed * GameTime.DeltaTime;
            if (Terrain.IsColliding(this)) {
                for (int i = (int)Hitbox.Position.x; i <= (int)Math.Ceiling(Hitbox.Position.x + Hitbox.Width); i++) {
                    for (int j = (int)Hitbox.Position.y; j < (int)Math.Ceiling(Hitbox.Position.y + Hitbox.Height); j++) {
                        // Terrain.BreakTile(i, j);
                    }
                }
                Entity.RemoveEntity(this);
            }
            if (Player.Intersecting(this)) {
                Player.Instance.Damage(1);
                Entity.RemoveEntity(this);
            }
        }
    }
}
