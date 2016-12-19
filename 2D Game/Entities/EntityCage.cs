using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Entities {
    class EntityCage : Entity {

        public Entity CapturedEntity { get; private set; }

        public EntityCage(Vector2 position) : base(EntityID.EntityCage, position) {
        }

        public override void InitTimers() {
        }

        public override void Update() {
            if (CapturedEntity != null) {
                var entities = EntityManager.GetEntitiesAt(data.pos, new Vector2(1, 2));
                if (entities.Length == 0) return;
                CapturedEntity = entities[0];
            }

            CapturedEntity.data.vel.val = Vector2.Zero;
            CapturedEntity.data.pos.val = data.pos + new Vector2(0, 1);
        }

    }
}
