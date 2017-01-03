using Pencil.Gaming.MathUtils;

namespace Game.Terrains {

    interface ILight {
        int Radius();
        float Strength();
        Vector3 Colour();
    }

    sealed class CLight : ILight {
        private int radius;
        private float strength;
        private Vector3 colour;

        public CLight(int radius, float strength, Vector3 colour) {
            this.radius = radius;
            this.strength = strength;
            this.colour = colour;
        }

        public int Radius() => radius;

        public float Strength() => strength;

        public Vector3 Colour() => colour;
    }

}
