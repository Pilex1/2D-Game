using Game.Terrains;
using Game.Terrains.Fluid_Tiles;
using System;

namespace Game.Fluids {

    [Serializable]
    class WaterAttribs : FlowFluidAttribs {

        public WaterAttribs() : this(8) { }
        protected WaterAttribs(int increments) : base(increments, 8, Tile.Water) { }


    }
}
