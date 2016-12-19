using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {
    [Serializable]
    class LightAttribs : TileAttribs {
        public int intensity { get; private set; }

        public LightAttribs(int intensity) {
            this.intensity = intensity;
        }
    }
}
