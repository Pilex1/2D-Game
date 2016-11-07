using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Fluids {

    [Serializable]
    class FluidAttribs : TileAttribs {

        public float height { get { return _height; } }
        internal BoundedFloat _height;
        public float viscosity { get; private set; }

        protected FluidAttribs(float viscosity) {
            base.transparent = true;
            _height = new BoundedFloat(1, 0, 1);
            this.viscosity = viscosity;
        }

        //flow downwrds, flowing outwards
        public virtual void Update(int x, int y) {
            TileEnum below = Terrain.TileAt(x, y - 1).enumId;
            if (below == TileEnum.Air || below == TileEnum.Water) {
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
                if (Terrain.TileAt(x, y - 1).enumId == TileEnum.Air) {
                    Tile newWater = Tile.Water;
                    FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
                    fluid._height.Empty();
                    BoundedFloat.MoveVals(ref _height, ref fluid._height, viscosity);
                    Terrain.SetTile(x, y - 1, newWater);
                }
            }
        }

        //protected virtual void Spread(int x, int y) {
        //    if (_height == 0) return;

        //    FluidAttribs l = Terrain.TileAt(x - 1, y).tileattribs as FluidAttribs;
        //    FluidAttribs r = Terrain.TileAt(x + 1, y).tileattribs as FluidAttribs;

        //    float lspace;
        //    float rspace;
        //    if (l == null) {
        //        if (Terrain.TileAt(x - 1, y).enumId == TileEnum.Air) {
        //            Tile newWater = Tile.Water;
        //            FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
        //            fluid._height.Empty();
        //            Terrain.SetTile(x - 1, y, newWater);
        //            l = fluid;
        //            lspace = 1;
        //        }
        //        else {
        //            lspace = 0;
        //        }
        //    }
        //    else {
        //        lspace = 1 - l._height;
        //    }
        //    if (r == null) {
        //        if (Terrain.TileAt(x + 1, y).enumId == TileEnum.Air) {
        //            Tile newWater = Tile.Water;
        //            FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
        //            fluid._height.Empty();
        //            Terrain.SetTile(x + 1, y, newWater);
        //            r = fluid;
        //            rspace = 1;
        //        }
        //        else {
        //            rspace = 0;
        //        }
        //    }
        //    else {
        //        rspace = 1 - r._height;
        //    }

        //    //if left and right are both full or if they are both non air tiles
        //    if (lspace + rspace == 0) return;

        //    if (_height > viscosity) {
        //        if (l != null)
        //            BoundedFloat.MoveVals(ref _height, ref l._height, viscosity * lspace / (lspace + rspace));
        //        if (r != null)
        //            BoundedFloat.MoveVals(ref _height, ref r._height, viscosity * rspace / (lspace + rspace));
        //    } else {
        //        if (l != null)
        //            BoundedFloat.MoveVals(ref _height, ref l._height, _height * lspace / _height);
        //        if (r != null)
        //            BoundedFloat.MoveVals(ref _height, ref r._height, _height * rspace / _height);
        //    }
        //}

        protected virtual void Spread(int x, int y) {
            if (_height == 0) return;

            FluidAttribs l = Terrain.TileAt(x - 1, y).tileattribs as FluidAttribs;
            FluidAttribs r = Terrain.TileAt(x + 1, y).tileattribs as FluidAttribs;

            if (l == null && Terrain.TileAt(x - 1, y).enumId == TileEnum.Air) {
                Tile newWater = Tile.Water;
                FluidAttribs fluid = ((FluidAttribs)newWater.tileattribs);
                fluid._height.Empty();
                Terrain.SetTile(x - 1, y, newWater);
                l = fluid;
            }
            if (r == null && Terrain.TileAt(x + 1, y).enumId == TileEnum.Air) {
                Tile newWater = Tile.Water;
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
                BoundedFloat.MoveVals(ref _height, ref l._height, amt);
            }
            if (r != null && r.height < _height) {
                BoundedFloat.MoveVals(ref _height, ref r._height, amt);
            }
        }

        public override string ToString() {
            return "Amount: " + _height.val;
        }
    }

    [Serializable]
    class WaterAttribs : FluidAttribs {
        public WaterAttribs()
            : base(0.2f) {

        }
    }

}
