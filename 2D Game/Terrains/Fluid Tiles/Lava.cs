using System;

namespace Game.Fluids {
    [Serializable]
    class LavaAttribs : FluidAttribs {
        public LavaAttribs() : base(8) {
        }

        protected override void FallAir(int x, int y) {
            throw new NotImplementedException();
        }

        protected override void FallFluid(int x, int y) {
            throw new NotImplementedException();
        }

        protected override void SpreadLeft(int x, int y) {
            throw new NotImplementedException();
        }

        protected override void SpreadRight(int x, int y) {
            throw new NotImplementedException();
        }
    }
}
