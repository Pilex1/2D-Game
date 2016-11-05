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
        public float rotfactor = 0;
        public void Update() {
            life.val -= GameTime.DeltaTime;
            base.rot += rotfactor * GameTime.DeltaTime;
        }
    }

    abstract class Particle : Entity {
        private Vector2 hitboxoffset;
        private static readonly float sqrt2 = (float)Math.Sqrt(2);

        public static new void Init() {
            StaffParticleGreen.Init();
            StaffParticleBlue.Init();
            StaffParticleRed.Init();
            StaffParticlePurple.Init();
        }

        public Particle(EntityID model, Vector2 pos) : base(model) {
            Vector2 size = Assets.Models.GetModel(model).size;
            hitboxoffset = new Vector2((1 - sqrt2) / 2 * size.x, (1 - sqrt2) / 2 * size.y);
            base.Hitbox = new RectangularHitbox(pos - hitboxoffset, sqrt2 * size);
            base.data = new ParticleData();
            base.data.Position.val = pos;
            base.data.UseGravity = true;
            Entity.AddEntity(this);
        }

        public override void Update() {
            base.UpdatePosition();
            base.Hitbox.Position = base.data.Position.val - hitboxoffset;
            ParticleData pdata = (ParticleData)data;
            pdata.Update();
            if (pdata.life < 0)
                Entity.RemoveEntity(this);
            if (Terrain.IsColliding(base.Hitbox))
                Entity.RemoveEntity(this);
            base.data.colour.w = base.data.life.val / base.data.life.max;
        }
    }
}
