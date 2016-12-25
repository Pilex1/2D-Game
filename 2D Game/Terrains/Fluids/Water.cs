using Game.Terrains;
using System;

namespace Game.Fluids {

    [Serializable]
    class WaterAttribs : FluidAttribs {

        public WaterAttribs() : base(8) { }
        protected WaterAttribs(int increments) : base(increments, 8) { }

        protected override void FallAir(int x, int y) {
            Tile t = Tile.Water();
            t.tileattribs = (WaterAttribs)MemberwiseClone();
            Terrain.SetTile(x, y - 1, t);
            Terrain.BreakTile(x, y);
        }

        protected override void FallFluid(int x, int y) {
            Tile t = Terrain.TileAt(x, y - 1);
            WaterAttribs attribs = t.tileattribs as WaterAttribs;
            if (attribs == null) return;

            attribs.increments += increments;
            if (attribs.increments <= attribs.maxIncrements) {
                Terrain.BreakTile(x, y);
            } else {
                increments = attribs.increments - attribs.maxIncrements;
                attribs.increments = attribs.maxIncrements;
            }
        }

        protected override void SpreadLeft(int x, int y) {
            SpreadTo(x - 1, y);
        }

        protected override void SpreadRight(int x, int y) {
            SpreadTo(x + 1, y);
        }

        protected void SpreadTo(int x, int y) {
            if (increments - 1 <= 0) return;
            Tile t = Terrain.TileAt(x, y);
            WaterAttribs attribs = t.tileattribs as WaterAttribs;
            if (t.enumId == TileID.Air) {
                Tile water = Tile.Water();
                ((WaterAttribs)(water.tileattribs)).increments = 1;
                increments--;
                Terrain.SetTile(x, y, water);
            } else if (attribs != null) {
                if (attribs.increments < increments) {
                    attribs.increments++;
                    increments--;
                }
            }
        }
    }
}
