using Game.Items;
using Game.Terrains;
using Game.Util;
using System;
using System.Text;

namespace Game.Fluids {

    [Serializable]
    abstract class FluidAttribs : TileAttribs {

        protected int maxIncrements;
        protected int increments;

        protected FluidAttribs(int increments, int maxIncrements) : base(delegate () { return RawItem.None; }) {
            this.increments = increments;
            this.maxIncrements = maxIncrements;

            transparent = true;
            solid = false;
        }
        protected FluidAttribs(int maxIncrements) : this(maxIncrements, maxIncrements) { }

        //flow downwrds, flowing outwards
        public virtual void Update(int x, int y) {

            TileID below = Terrain.TileAt(x, y - 1).enumId;
            FluidAttribs fluid = Terrain.TileAt(x, y - 1).tileattribs as FluidAttribs;

            if (below == TileID.Air) {
                FallAir(x, y);
            } else if (fluid != null && fluid.increments < fluid.maxIncrements) {
                FallFluid(x, y);
            } else {
                //bias to going left if there is not enough fluid to cover both left and right
                SpreadLeft(x, y);
                SpreadRight(x, y);
            }
        }

        protected abstract void FallAir(int x, int y);
        protected abstract void FallFluid(int x, int y);
        protected abstract void SpreadLeft(int x, int y);
        protected abstract void SpreadRight(int x, int y);

        public float GetHeight() {
            return (float)increments / maxIncrements;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Amount: " + increments + " / " + maxIncrements);
            return sb.ToString();
        }

    }

    //[Serializable]
    //class _FluidAttribs : TileAttribs {

    //    protected BoundedFloat height_bf;
    //    public float height { get { return height_bf; } }
    //    public float viscosity { get; private set; }

    //    protected _FluidAttribs(float viscosity) : base(delegate () { return RawItem.None; }) {
    //        base.transparent = true;
    //        height_bf = new BoundedFloat(1, 0, 1);
    //        this.viscosity = viscosity;
    //    }

    //    protected override void OnDestroy(int x, int y, Inventory inv) {
    //        Terrain.FluidDict.Remove(new Vector2i(x, y));
    //    }

    //    //flow downwrds, flowing outwards
    //    public virtual void Update(int x, int y) {

    //        TileID below = Terrain.TileAt(x, y - 1).enumId;
    //        _FluidAttribs fluid = Terrain.TileAt(x, y - 1).tileattribs as _FluidAttribs;
    //        if (below == TileID.Air || (fluid != null && fluid.height_bf < 1 && below == TileID.Water)) {
    //            Fall(x, y);
    //        } else {
    //            Spread(x, y);
    //        }
    //    }

    //    protected virtual void Fall(int x, int y) {
    //        if (height_bf == 0) return;

    //        _FluidAttribs d = Terrain.TileAt(x, y - 1).tileattribs as _FluidAttribs;
    //        if (d != null) {
    //            BoundedFloat.MoveVals(ref height_bf, ref d.height_bf, viscosity);
    //        } else {
    //            if (Terrain.TileAt(x, y - 1).enumId == TileID.Air) {
    //                Tile newWater = Tile.Water();
    //                _FluidAttribs fluid = ((_FluidAttribs)newWater.tileattribs);
    //                fluid.height_bf.Empty();
    //                BoundedFloat.MoveVals(ref height_bf, ref fluid.height_bf, viscosity);
    //                Terrain.SetTile(x, y - 1, newWater);
    //            }
    //        }
    //    }

    //    protected virtual void Spread(int x, int y) {
    //        if (height_bf == 0) return;

    //        _FluidAttribs l = Terrain.TileAt(x - 1, y).tileattribs as _FluidAttribs;
    //        _FluidAttribs r = Terrain.TileAt(x + 1, y).tileattribs as _FluidAttribs;

    //        if (l == null && Terrain.TileAt(x - 1, y).enumId == TileID.Air) {
    //            Tile newWater = Tile.Water();
    //            _FluidAttribs fluid = ((_FluidAttribs)newWater.tileattribs);
    //            fluid.height_bf.Empty();
    //            Terrain.SetTile(x - 1, y, newWater);
    //            l = fluid;
    //        }
    //        if (r == null && Terrain.TileAt(x + 1, y).enumId == TileID.Air) {
    //            Tile newWater = Tile.Water();
    //            _FluidAttribs fluid = ((_FluidAttribs)newWater.tileattribs);
    //            fluid.height_bf.Empty();
    //            Terrain.SetTile(x + 1, y, newWater);
    //            r = fluid;
    //        }

    //        if (height_bf < viscosity) return;

    //        int count = 0;
    //        if (l != null && l.height_bf < height_bf) {
    //            count++;
    //        }
    //        if (r != null && r.height_bf < height_bf) {
    //            count++;
    //        }
    //        if (count == 0) return;

    //        float amt = viscosity / count;
    //        if (l != null && l.height < height_bf) {
    //            BoundedFloat.MoveVals(ref height_bf, ref l.height_bf, l.height_bf + amt > height_bf ? height_bf - l.height_bf : amt);
    //        }
    //        if (r != null && r.height < height_bf) {
    //            BoundedFloat.MoveVals(ref height_bf, ref r.height_bf, r.height_bf + amt > height_bf ? height_bf - r.height_bf : amt);
    //        }
    //    }

    //    public override string ToString() {
    //        return "Amount: " + height_bf.val;
    //    }
    //}


}
