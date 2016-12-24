using System;

namespace Game.Fluids {
    [Serializable]
    class LavaAttribs : FluidAttribs {
        public LavaAttribs() : base(0.04f) {
        }

        public override void Update(int x, int y) {
            base.Fall(x, y);
            base.Spread(x, y);
        }
    }
}
