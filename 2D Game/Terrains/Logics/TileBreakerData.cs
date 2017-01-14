using Game.Items;
using System;

namespace Game.Terrains.Logics {
    [Serializable]
    class TileBreakerAttribs : PowerDrain {

        public TileBreakerAttribs() : base(delegate () { return RawItem.TileBreaker; }) {
        }

        protected override void UpdateMechanics(int x, int y) {

        }
    }
}
