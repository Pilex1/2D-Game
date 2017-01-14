using System;
using Game.Entities;
using Game.Util;
using Game.Items;
using Game.Core;

namespace Game.Terrains {

    [Serializable]
    class WardedTileAttribs : TileAttribs {

        public WardedTileAttribs() : base(delegate () { return RawItem.WardedTile; }) { }

        public override void OnEntityCollision(int x, int y, Direction side, Entity e) {
            if (e.GetType() != typeof(Player)) return;
            base.OnEntityCollision(x, y, side, e);
        }
    }
}
