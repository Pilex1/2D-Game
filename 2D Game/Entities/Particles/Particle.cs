using Game.Core;
using Game.Entities;
using Game.Terrains;
using OpenGL;
using System;

namespace Game.Particles {

    [Serializable]
    class ParticleData : EntityData {
        public float rotfactor = 0;
        public void Update() {
            life.val -= GameTime.DeltaTime;
            rot += rotfactor * GameTime.DeltaTime;
        }
    }

    [Serializable]
    abstract class Particle : Entity {

        public static void Init() {
            SParc_Damage.Init();
            SParc_Destroy.Init();
            SParc_Speed.Init();
            SParc_Water.Init();
            SParc_Place.Init();
        }

        public Particle(EntityID model, Vector2 pos) : base(model, pos) {
            Vector2 size = Assets.Models.GetModel(model).size;
            data = new ParticleData();
            data.pos.val = pos;
            EntityManager.AddEntity(this);
        }

        public override void Update() {
            UpdatePosition();
            ((ParticleData)data).Update();
            data.colour.w = data.life.val / data.life.max;
        }

        protected void RemoveIfColliding() {
            if (Colliding()) EntityManager.RemoveEntity(this);
        }
    }
}