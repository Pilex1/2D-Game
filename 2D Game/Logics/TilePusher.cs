using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logics {
    class TilePusher : PowerDrain, ISolid {

        private const int MaxTiles = 32;

        public static void Create(int x, int y) {
            if (Terrain.TileAt(x, y).id == TileID.Air) new TilePusher(x, y);
        }

        private TilePusher(int x, int y) : base(x, y, TileID.TilePusherOff) {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            power.max = 64;
        }

        internal override void Update() {

            BoundedFloat.MoveVals(ref powerinL, ref power, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref power, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref power, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref power, powerinD.val);

            BoundedFloat dissipate = new BoundedFloat(0, 0, 1);
            id = power.val > 0 ? TileID.TilePusherOn : TileID.TilePusherOff;
            if (power.val == power.max) {
                HashSet<Vector2i> done = new HashSet<Vector2i>();
                try {
                    PushTilesHelper(x, y, x + 1, y, done);
                    var list = done.ToList();
                    list.Sort(new TilePositionComparer());
                    foreach (var v in list) {
                        Terrain.MoveTile(v.x, v.y, Direction.Right);
                    }
                    dissipate.max = 32;
                } catch (ArgumentException) { }
            }
            BoundedFloat.MoveVals(ref power, ref dissipate, dissipate.max);
        }

        private void Push(int srcx, int srcy, Tile tile, HashSet<Vector2i> done) {
            if (tile.x <= srcx) return;
            if (!(tile is IUnmovable) && !done.Contains(tile.Pos())) {
                done.Add(tile.Pos());
                PushTilesHelper(srcx, srcy, tile.x, tile.y, done);
                if (done.Count > MaxTiles)
                    throw new ArgumentException("Attempting to move more than " + MaxTiles + " Tiles");
            }
        }

        private void PushTilesHelper(int srcx, int srcy, int x, int y, HashSet<Vector2i> done) {
            Tile u = Terrain.TileAt(x, y + 1), d = Terrain.TileAt(x, y - 1), l = Terrain.TileAt(x - 1, y), r = Terrain.TileAt(x + 1, y);
            try {
                Push(srcx, srcy, l, done);
                Push(srcx, srcy, r, done);
                Push(srcx, srcy, u, done);
                Push(srcx, srcy, d, done);
            } catch (ArgumentException e) {
                throw e;
            }
        }

        private class TilePositionComparer : IComparer<Vector2i> {
            public int Compare(Vector2i x, Vector2i y) {
                if (x.y < y.y) return 1;
                else if (x.y == y.y) return 0;
                else return -1;
            }
        }
    }
}
