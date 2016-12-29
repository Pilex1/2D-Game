using System;
using Game.Items;
using OpenGL;

namespace Game.Terrains.Lightings {

    [Serializable]
    class LightAttribs : TileAttribs, ILight {

        public LightAttribs() : base(() => RawItem.Light) {
        }

        int ILight.Radius() => 8;

        Vector4 ILight.Colour() => new Vector4(1, 1, 1, 1);
    }
}
