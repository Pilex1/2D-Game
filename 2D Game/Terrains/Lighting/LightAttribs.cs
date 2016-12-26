using Game.Items;
using System;

namespace Game.Terrains {
    [Serializable]
    class LightAttribs : TileAttribs {
        public int intensity { get; private set; }

        public LightAttribs(Func<RawItem> dropItem, int intensity) : base(dropItem) {
            this.intensity = intensity;
        }
    }
}
