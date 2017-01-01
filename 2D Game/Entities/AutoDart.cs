using Game.Core;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;
using Game.Terrains;

namespace Game.Entities {

    [Serializable]
    class AutoDart : Entity {

        public AutoDart(Vector2 pos, Vector2 vel) : base(EntityID.AutoDart, pos) {
            data.vel.val = vel;
            data.airResis = 0.999f;
            data.grav = 0f;
            data.life = new BoundedFloat(150, 0, 150);
            data.calcTerrainCollisions = false;
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            EntityManager.RemoveEntity(this);
        }

        public override void Update() {
            UpdatePosition();

            DamageNatural(GameTime.DeltaTime);
            var entities = GetEntityCollisions();
            foreach (var e in entities) {
                if (e is AutoDart) continue;
                e.Damage(1);
                DamageNatural(5);
                if (IsDead()) break;
            }
        }
    }
}
