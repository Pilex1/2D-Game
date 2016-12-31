using Game.Core;
using Game.Entities;
using OpenGL;
using System;

namespace Game.Entities.Particles {

    [Serializable]
    abstract class Particle : Entity {

        public static void Init() {
            SParc_Damage.Init();
            SParc_Destroy.Init();
            SParc_Speed.Init();
            SParc_Water.Init();
            SParc_Place.Init();
        }

        public float deltaRot;

        public Particle(EntityID model, Vector2 pos) : base(model, pos) {
            Vector2 size = Assets.Models.GetModel(model).size;
            data.calcTerrainCollisions = false;
            data.pos.val = pos;
            EntityManager.AddEntity(this);
        }

        public override void Update() {
            UpdatePosition();
            DamageNatural(GameTime.DeltaTime);
            data.rot += deltaRot;
        }

        protected void RemoveIfColliding() {
            if (Colliding()) EntityManager.RemoveEntity(this);
        }
    }
}