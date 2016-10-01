using Game.Terrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {
    class ORGate : PowerTransmitter, ISolid {

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new ORGate(x, y);
        }

        private ORGate(int x, int y) : base(x, y, TileID.GateOr) {
            powerL.max = powerR.max = powerU.max = powerD.max = 64;
            transLevel.max = 128;
        }

        internal override void Update() {
            throw new NotImplementedException();
        }
    }
}
