using Game.Main.Util;

namespace Game.Terrains {

    interface ILight {
        int Radius();
        ColourHSB Colour();
    }

    class CLight:ILight {
        int radius;
        ColourHSB colour;

        public CLight(int radius, ColourHSB colour) {
            this.radius = radius;
            this.colour = colour;
        }

        int ILight.Radius() => radius;
        ColourHSB ILight.Colour() => colour;
    }
    

}
