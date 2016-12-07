using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Fluids {

    [Serializable]
    class WaterAttribs : FluidAttribs {
        public WaterAttribs() : base(0.2f) {
            base.solid = false;
        }
    }
}
