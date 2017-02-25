using Game.Core;
using Game.Main.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Entities {

    [Serializable]
    class EntityCage : Entity {

        public Entity CapturedEntity { get; private set; }

        public EntityCage(Vector2 position) : base(EntityID.EntityCage, position, new Vector2(1, 2)) {
            data.invulnerable = true;
        }

        public override void Update() {
            if (CapturedEntity == null) {
                var entities = EntityManager.GetEntitiesAt(data.pos, new Vector2(1, 2), e => !(e is EntityCage || e is Player || e is ShooterProjectile));
                if (entities.Length == 0) return;
                CapturedEntity = entities[0];
            } else {
                CapturedEntity.data.vel.val = Vector2.Zero;
                CapturedEntity.data.pos.val = data.pos + new Vector2(0, 1);
                CapturedEntity.data.colour = new ColourRGBA(204, 204, 204, 0.5f);
                CapturedEntity.data.invulnerable = true;
            }
        }

        public void Release() {
            if (CapturedEntity == null) return;
            CapturedEntity.data.invulnerable = false;
            CapturedEntity.data.colour = new ColourRGBA(255, 255, 255, 1);
        }

    }
}
