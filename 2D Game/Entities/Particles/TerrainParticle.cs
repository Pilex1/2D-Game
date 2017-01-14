using Pencil.Gaming.MathUtils;

namespace Game.Entities.Particles {

    abstract class TerrainParticleGenerator {

        protected Vector2i pos;

        protected TerrainParticleGenerator(Vector2i pos) {
            this.pos = pos;
        }

        public abstract void Generate();

    }


    class TerrainParticle : Particle {
        public TerrainParticle(EntityID model, Vector2 pos, Vector2 size) : base(model, pos, size) {
        }
    }
}
