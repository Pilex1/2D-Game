using System;

namespace Game.Terrains.Fluids {
    [Serializable]
    abstract class FlowFluidAttribs : FluidAttribs {

        protected Func<Tile> fluid;

        protected FlowFluidAttribs(int maxIncrements, Func<Tile> fluid) : this(maxIncrements, maxIncrements, fluid) { }
        protected FlowFluidAttribs(int increments, int maxIncrements, Func<Tile> fluid) : base(increments, maxIncrements) {
            this.fluid = fluid;
        }

        protected override void FallAir(int x, int y) {
            Tile t = fluid();
            t.tileattribs = (TileAttribs)MemberwiseClone();
            Terrain.BreakTile(x, y);
            Terrain.SetTile(x, y - 1, t);
        }

        protected override void FallFluid(int x, int y) {
            Tile t = Terrain.TileAt(x, y - 1);
            FluidAttribs attribs = t.tileattribs as FluidAttribs;
            if (!t.tileattribs.GetType().Equals(fluid().tileattribs.GetType())) return;

            attribs.increments += increments;
            if (attribs.increments <= attribs.maxIncrements) {
                Terrain.BreakTile(x, y);
            } else {
                increments = attribs.increments - attribs.maxIncrements;
                attribs.increments = attribs.maxIncrements;
            }
            FluidManager.Instance.AddUpdate(x, y - 1, attribs);
        }

        protected override void SpreadLeft(int x, int y) {
            SpreadTo(x - 1, y);
        }

        protected override void SpreadRight(int x, int y) {
            SpreadTo(x + 1, y);
        }

        protected virtual void SpreadTo(int x, int y) {
            if (increments - 1 <= 0) return;
            Tile t = Terrain.TileAt(x, y);
            FluidAttribs attribs = t.tileattribs as FluidAttribs;
            if (t.enumId == TileID.Air) {
                Tile f = fluid();
                ((FluidAttribs)(f.tileattribs)).increments = 1;
                increments--;
                Terrain.SetTile(x, y, f);
            } else if (attribs != null && attribs.GetType().Equals(fluid().tileattribs.GetType())) {
                if (attribs.increments < increments) {
                    attribs.increments++;
                    FluidManager.Instance.UpdateAround(x, y);
                    increments--;
                }
            }
        }

        protected override void UpdateFinal(int x, int y) { }
    }
}
