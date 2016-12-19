using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {
    class EntitySpawnerAttribs : PowerDrainData {

        internal bool state;

        public EntitySpawnerAttribs() {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 16;
            cost = 2;
            state = false;
        }

        internal override void Update(int x, int y) {

        }
    }
}
