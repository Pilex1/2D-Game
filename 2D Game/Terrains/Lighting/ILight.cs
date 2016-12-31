using OpenGL;

namespace Game.Terrains {

    interface ILight {
        int Radius();
        float Strength();
        Vector3 Colour();
    }
}
