using System;
using Game.Items;
using OpenGL;

namespace Game.Terrains.Lightings {

    [Serializable]
    class LightAttribs : TileAttribs, ILight {

        public LightAttribs() : base(() => RawItem.Light) {
        }

        int ILight.Radius() => 8;
        Vector3 ILight.Colour() => new Vector3(1, 1, 1);
        float ILight.Strength() => 1f;
    }
}
