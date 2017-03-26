using Game.Main.Util;
using System;

namespace Game.Terrains {

    interface ILight {
        int Radius();
        ColourHSB Colour();
    }

    [Serializable]
    class CLight : ILight {
        int radius;
        ColourHSB colour;

        public CLight(int radius, ColourHSB colour) {
            this.radius = radius;
            this.colour = colour;
        }

        public CLight(int radius, float hue, float saturation, float brightness) : this(radius, new ColourHSB(hue, saturation, brightness)) { }

        int ILight.Radius() => radius;
        ColourHSB ILight.Colour() => colour;
    }


}
