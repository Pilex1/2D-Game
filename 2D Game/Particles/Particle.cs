using Game.Core;
using Game.Entities;
using Game.Terrains;
using Game.Util;
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

        private Vector2 hitboxoffset;

        public static void Init() {
            SParc_Damage.Init();
            SParc_Destroy.Init();
            SParc_Speed.Init();
            SParc_Water.Init();
        }

        public Particle(EntityID model, Vector2 pos) : base(model, pos) {
            Vector2 size = Assets.Models.GetModel(model).size;
            hitboxoffset = MathUtil.Sqrt2_Offset * size;
            hitbox = new RectangularHitbox(pos - hitboxoffset, MathUtil.Sqrt2 * size);
            data = new ParticleData();
            data.pos.val = pos;
            data.useGravity = true;
            EntityManager.AddEntity(this);
        }

        public override void UpdateHitbox() {
            hitbox.Position = data.pos.val - hitboxoffset;
        }

        public override void Update() {
            UpdatePosition();
            ((ParticleData)data).Update();
            data.colour.w = data.life.val / data.life.max;
        }

        protected void RemoveIfColliding() {
            if (Terrain.IsColliding(this)) EntityManager.RemoveEntity(this);
        }
    }
}
