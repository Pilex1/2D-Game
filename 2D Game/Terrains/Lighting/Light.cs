using System;
using Game.Items;

namespace Game.Terrains.Lightings {

    [Serializable]
    class LightAttribs : TileAttribs, ILight {

        public LightAttribs() : base(() => RawItem.Light) {
        }

        int ILight.intensity() => 8;
    }
}
