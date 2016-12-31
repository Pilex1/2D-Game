using System;

namespace Game.Terrains.Fluids {

    [Serializable]
    class WaterAttribs : FlowFluidAttribs {

        public WaterAttribs(int increments = 8) : base(increments, 8, Tile.Water) {
            mvtFactor = 0.03f;
        }


    }
}
