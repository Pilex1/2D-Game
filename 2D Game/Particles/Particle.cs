using Game.Core;
using Game.Entities;
using Game.Terrains;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Particles {

    class ParticleData : EntityData {
        public float curlife = 0;
        public float life = 0;
        public float rotfactor = 0;
        public void Update() {
            curlife += GameTime.DeltaTime;
            base.rot += rotfactor * GameTime.DeltaTime;
        }
    }

    abstract class Particle : Entity {
        private Vector2 hitboxoffset;
        private static readonly float sqrt2 = (float)Math.Sqrt(2);

        public static new void Init() {
            StaffParticleGreen.Init();
        }

        public Particle(EntityModel model, Vector2 pos) : base(model) {
            hitboxoffset = new Vector2((1 - sqrt2) / 2 * model.size.x, (1 - sqrt2) / 2 * model.size.y);
            base.Hitbox = new RectangularHitbox(pos - hitboxoffset, sqrt2 * model.size);

            base.data = new ParticleData();
            base.data.Position.val = pos;
        }

        public override void Update() {
            base.data.Position.x += base.data.vel.x * GameTime.DeltaTime;
            base.data.Position.y += base.data.vel.y * GameTime.DeltaTime;
            base.Hitbox.Position = base.data.Position.val - hitboxoffset;
            ParticleData pdata = (ParticleData)data;
            pdata.Update();
            if (pdata.curlife > pdata.life)
                Entity.RemoveEntity(this);
            if (Terrain.IsColliding(base.Hitbox))
                Entity.RemoveEntity(this);
            base.data.colour.w=(pdata.life-pdata.curlife)/pdata.life;
        }
    }
}
