using Game.Items;
using Game.Terrains;
using Game.Util;
using System;

namespace Game.Fluids {

    [Serializable]
    class FluidAttribs : TileAttribs {

        public float height { get { return _height; } }
        internal BoundedFloat _height;
        public float viscosity { get; private set; }

        protected FluidAttribs(float viscosity) : base(delegate() { return RawItem.None; }) {
            base.transparent = true;
            _height = new BoundedFloat(1, 0, 1);
            this.viscosity = viscosity;
        }

        public override void OnDestroy(int x, int y, Inventory inv) {
            Terrain.FluidDict.Remove(new Vector2i(x, y));
        }

        //flow downwrds, flowing outwards
        public virtual void Update(int x, int y) {

            //TileEnum below = Terrain.TileAt(x, y - 1).enumId;
            //if (below == TileEnum.Air) {
            //    Fall(x, y);
            //}else {
            //    Spread(x, y);
            //}

            TileID below = Terrain.TileAt(x, y - 1).enumId;
            FluidAttribs fluid = Terrain.TileAt(x, y - 1).tileattribs as FluidAttribs;
            if (below == TileID.Air || (fluid != null && fluid._height < 1 && below == TileID.Water)) {
                Fall(x, y);
            } else {
                Spread(x, y);
            }
        }

        protected virtual void Fall(int x, int y) {
            if (_height == 0) return;

            FluidAttribs d = Terrain.TileAt(x, y - 1).tileattribs as FluidAttribs;
            if (d != null) {
                BoundedFloat.MoveVals(ref _height, ref d._height, viscosity);
            } else {
                if (Terrain.TileAt(x, y - 1).enumId == TileID.Air) {
                    Tile newWater = Tile.Water();
                    FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
                    fluid._height.Empty();
                    BoundedFloat.MoveVals(ref _height, ref fluid._height, viscosity);
                    Terrain.SetTile(x, y - 1, newWater);
                }
            }
        }

        protected virtual void Spread(int x, int y) {
            if (_height == 0) return;

            FluidAttribs l = Terrain.TileAt(x - 1, y).tileattribs as FluidAttribs;
            FluidAttribs r = Terrain.TileAt(x + 1, y).tileattribs as FluidAttribs;

            if (l == null && Terrain.TileAt(x - 1, y).enumId == TileID.Air) {
                Tile newWater = Tile.Water();
                FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
                fluid._height.Empty();
                Terrain.SetTile(x - 1, y, newWater);
                l = fluid;
            }
            if (r == null && Terrain.TileAt(x + 1, y).enumId == TileID.Air) {
                Tile newWater = Tile.Water();
                FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
                fluid._height.Empty();
                Terrain.SetTile(x + 1, y, newWater);
                r = fluid;
            }

            if (_height < viscosity) return;

            int count = 0;
            if (l != null && l._height < _height) {
                count++;
            }
            if (r != null && r._height < _height) {
                count++;
            }
            if (count == 0) return;

            float amt = viscosity / count;
            if (l != null && l.height < _height) {
                BoundedFloat.MoveVals(ref _height, ref l._height, l._height + amt > _height ? _height - l._height : amt);
            }
            if (r != null && r.height < _height) {
                BoundedFloat.MoveVals(ref _height, ref r._height, r._height + amt > _height ? _height - r._height : amt);
            }
        }

        public override string ToString() {
            return "Amount: " + _height.val;
        }
    }


}
