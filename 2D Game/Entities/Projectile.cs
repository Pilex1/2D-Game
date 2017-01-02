using System;
using Pencil.Gaming.MathUtils;
using Game.Entities;
using Game.Terrains;
using Game.Core;
using Game.Util;

namespace Game {

    [Serializable]
    class ShooterProjectile : Entity {

        public ShooterProjectile(Vector2 position, Vector2 velocity, int maxlife) : base(EntityID.ShooterProjectile, position, new Vector2(0.8f, 0.8f)) {
            data.speed = 0;
            data.jumppower = 0;
            data.life = new BoundedFloat(100, 0, 100);
            data.vel.val = velocity;
            data.grav = 0;
            data.calcTerrainCollisions = false;
            data.airResis = 1;
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            EntityManager.RemoveEntity(this);
        }

        public override void Update() {
            UpdatePosition();
            data.life -= GameTime.DeltaTime;
            data.colour.w = (float)Math.Pow(Math.Sin(Math.PI / 2 * data.life.GetFilledRatio()), 0.25);

            if (Player.Intersecting(this)) {
                Player.Instance.Damage(1);
                EntityManager.RemoveEntity(this);
            }
        }
    }
}
