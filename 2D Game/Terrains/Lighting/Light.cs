using System;
using Game.Items;
using Game.Main.Util;

namespace Game.Terrains.Lightings {

    [Serializable]
    class LightAttribs : TileAttribs, ILight {

        protected int radius;
        protected ColourHSB colour;

        public LightAttribs(int radius, ColourHSB colour, Func<RawItem> dropItem) : base(dropItem) {
            this.radius = radius;
            this.colour = colour;
        }

        int ILight.Radius() => radius;
        ColourHSB ILight.Colour() => colour;

    }
}
