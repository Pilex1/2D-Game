using System;
using OpenGL;

namespace Game {
    class Light {
        public const int MaxLightLevel = 16;

        public int x, y;
        public int LightLevel;

        public Light(int x, int y, int lightLevel) {
            this.x = x;
            this.y = y;
            LightLevel = lightLevel;
        }
    }
}
