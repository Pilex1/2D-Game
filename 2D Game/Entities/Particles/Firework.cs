using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Terrains;
using Game.Util;

namespace Game.Entities.Particles {

    [Serializable]
    class FireworkParticle : Particle, ILight {

        public FireworkParticle(Vector2 pos, Vector2 vel, float life, Vector4 colour) : base(EntityID.WhiteFill, pos) {
            data.airResis = 0.995f;
            data.grav = 0.01f;
            data.vel.val = vel;
            data.colour = colour;
            data.life = new BoundedFloat(life, 0, life);
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            EntityManager.RemoveEntity(this);
        }

        Vector3 ILight.Colour() => data.colour.Xyz;
        int ILight.Radius() => 8;
        float ILight.Strength() => 0.25f;
    }

    [Serializable]
    class FireworkLauncher : Particle, ILight {

        private int freq;

        public FireworkLauncher(Vector2 pos, Vector4 colour, int freq) : base(EntityID.WhiteFill, pos) {
            data.vel.val = new Vector2(0, 0.5);
            data.grav = 0;
            data.airResis = 1;
            data.life = new BoundedFloat(35, 0, 35);
            data.colour = colour;
            this.freq = freq;
        }

        private void Explode() {

            for (int i = 0; i < freq; i++) {
                Vector2 vel = MathUtil.Vec2FromAngle(MathUtil.RandFloat(Program.Rand, 0, 2 * Math.PI)) / 2;
                vel *= MathUtil.RandFloat(Program.Rand, 0.8, 1.2);
                FireworkParticle particle = new FireworkParticle(data.pos, vel, 40, data.colour);
            }

            EntityManager.RemoveEntity(this);
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            Explode();
        }

        public override void OnDeath() {
            Explode();
        }

        Vector3 ILight.Colour() => data.colour.Xyz;
        int ILight.Radius() => 8;
        float ILight.Strength() => 0.25f;

    }
}
