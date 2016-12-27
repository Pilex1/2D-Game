using Game.Terrains;
using Game.Terrains.Fluid_Tiles;
using System;

namespace Game.Fluids {
    [Serializable]
    class LavaAttribs : FlowFluidAttribs {
        public LavaAttribs() : this(8) { }
        protected LavaAttribs(int increments) : base(increments, 8, Tile.Lava) { }
    }
}
