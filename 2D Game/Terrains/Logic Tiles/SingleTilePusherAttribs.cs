using Game.Items;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Terrains.Logics {
    [Serializable]
    class SingleTilePusherAttribs : PowerDrainData {

        [NonSerialized]
        private CooldownTimer cooldown;

        private const int MaxTiles = 32;

        public bool state { get; private set; }

        public SingleTilePusherAttribs() : base(delegate () { return RawItem.SingleTilePusher; }) {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            cost = 8;
        }

        internal override void Update(int x, int y) {
            if (cooldown == null)
                cooldown = new CooldownTimer(40);

            BoundedFloat buffer = new BoundedFloat(0, 0, cost);

            CachePowerLevels();

            if (powerinL + powerinR + powerinU + powerinD >= buffer.max) {
                BoundedFloat.MoveVals(ref powerinL, ref buffer, powerinL);
                BoundedFloat.MoveVals(ref powerinR, ref buffer, powerinR);
                BoundedFloat.MoveVals(ref powerinU, ref buffer, powerinU);
                BoundedFloat.MoveVals(ref powerinD, ref buffer, powerinD);
            }

            EmptyInputs();

            if (buffer.IsFull()) {
                state = true;
                if (cooldown.Ready()) {
                    HashSet<Vector2i> done;
                    if (PushTilesHelper(x, y, rotation, out done)) {
                        var list = done.ToList();
                        list.Sort(new TilePositionComparer(rotation));
                        foreach (var v in list) {
                            //here
                            Terrain.MoveTile(v.x, v.y, rotation);
                        }
                        cooldown.Reset();
                    }
                }
            } else {
                state = false;
            }

            Terrain.TileAt(x, y).enumId = state ? TileID.SingleTilePusherOn : TileID.SingleTilePusherOff;

        }

        private bool PushTilesHelper(int x, int y, Direction d, out HashSet<Vector2i> processed) {
            processed = new HashSet<Vector2i>();
            while (true) {
                Tile t = null;
                switch (d) {
                    case Direction.Up:
                        t = Terrain.TileAt(x, ++y);
                        break;
                    case Direction.Right:
                        t = Terrain.TileAt(++x, y);
                        break;
                    case Direction.Down:
                        t = Terrain.TileAt(x, --y);
                        break;
                    case Direction.Left:
                        t = Terrain.TileAt(--x, y);
                        break;
                }

                if (t.enumId == TileID.Air) break;
                processed.Add(new Vector2i(x, y));
                if (processed.Count > MaxTiles) return false;
            }

            return true;
        }

        private class TilePositionComparer : IComparer<Vector2i> {
            private Direction dir;
            internal TilePositionComparer(Direction dir) {
                this.dir = dir;
            }
            public int Compare(Vector2i a, Vector2i b) {
                switch (dir) {
                    case Direction.Up:
                        if (a.y < b.y) return 1;
                        else if (a.y == b.y) return 0;
                        else return -1;
                    case Direction.Down:
                        if (a.y > b.y) return 1;
                        else if (a.y == b.y) return 0;
                        else return -1;
                    case Direction.Right:
                        if (a.x < b.x) return 1;
                        else if (a.x == b.x) return 0;
                        else return -1;
                    case Direction.Left:
                        if (a.x > b.x) return 1;
                        else if (a.x == b.x) return 0;
                        else return -1;
                }
                throw new ArgumentException();
            }
        }
    }
}
