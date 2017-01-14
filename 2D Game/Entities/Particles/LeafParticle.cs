using Pencil.Gaming.MathUtils;

namespace Game.Entities.Particles {
    abstract class LeafParticle : TerrainParticle {
        protected LeafParticle(EntityID model, Vector2 pos) : base(model, pos, 0.1f * Vector2.One) {
        }
    }

    class NormalLeafParticle : LeafParticle {
        public NormalLeafParticle(Vector2 pos) : base(EntityID.LeafParticle, pos) {
            SetMaxLifeFull(5);

        }
    }

    class SnowLeafParticle : LeafParticle {
        public SnowLeafParticle(Vector2 pos) : base(EntityID.SnowLeafParticle, pos) {
        }
    }

}
