using System;

namespace Game.Fluids {

    [Serializable]
    class WaterAttribs : FluidAttribs {
        public WaterAttribs() : base(0.2f) {
            base.solid = false;
        }
    }
}
