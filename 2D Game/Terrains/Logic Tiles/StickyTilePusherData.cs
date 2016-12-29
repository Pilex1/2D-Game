using Game.Items;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Terrains.Logics {
    [Serializable]
    class StickyTilePusherAttribs : PowerDrainData {

        [NonSerialized]
        private CooldownTimer cooldown;

        private const int MaxTiles = 32;

        public bool state { get; private set; }

        public StickyTilePusherAttribs() : base(delegate () { return RawItem.StickyTilePusher; }) {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            cooldown = new CooldownTimer(40);
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
                    if (PushTilesHelper(x, y, out done)) {
                        var list = done.ToList();
                        list.Sort(new TilePositionComparer());
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

            Terrain.TileAt(x, y).enumId = state ? TileID.TilePusherOn : TileID.TilePusherOff;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Required: {0}", cost));
            sb.AppendLine(string.Format("Left: In {0}", powerInLCache));
            sb.AppendLine(string.Format("Right: In {0}", powerInRCache));
            sb.AppendLine(string.Format("Up: In {0}", powerInUCache));
            sb.AppendLine(string.Format("Down: In {0}", powerInDCache));
            return sb.ToString();
        }

        //returns false if the amount of pushable tiles exceeds the max limit, else returns true
        private bool PushTilesHelper(int x, int y, out HashSet<Vector2i> processed) {

            processed = new HashSet<Vector2i>();
            var processing = new HashSet<Vector2i>();

            processing.Add(new Vector2i(x, y));

            while (processing.Count > 0) {
                if (processed.Count > MaxTiles)
                    return false;

                var list = new List<Vector2i>(processing);
                foreach (var v in list) {

                    processing.Remove(v);

                    Vector2i lv = new Vector2i(v.x - 1, v.y);
                    Tile l = Terrain.TileAt(lv);
                    if (v.x - 1 > x && l.enumId != TileID.Air && !processed.Contains(lv)) {
                        processed.Add(lv);
                        Vector2i[] arr = Neighbours(lv);
                        processing.Add(lv);
                        foreach (var a in arr)
                            if (Terrain.TileAt(a).enumId != TileID.Air)
                                processing.Add(a);
                    } else {
                        processing.Remove(lv);
                    }

                    Vector2i rv = new Vector2i(v.x + 1, v.y);
                    Tile r = Terrain.TileAt(rv);
                    if (v.x + 1 > x && r.enumId != TileID.Air && !processed.Contains(rv)) {
                        processed.Add(rv);
                        Vector2i[] arr = Neighbours(rv);
                        processing.Add(rv);
                        foreach (var a in arr)
                            if (Terrain.TileAt(a).enumId != TileID.Air)
                                processing.Add(a);
                    } else {
                        processing.Remove(rv);
                    }

                    Vector2i uv = new Vector2i(v.x, v.y + 1);
                    Tile u = Terrain.TileAt(uv);
                    if (v.x > x && u.enumId != TileID.Air && !processed.Contains(uv)) {
                        processed.Add(uv);
                        Vector2i[] arr = Neighbours(uv);
                        processing.Add(uv);
                        foreach (var a in arr)
                            if (Terrain.TileAt(a).enumId != TileID.Air)
                                processing.Add(a);
                    } else {
                        processing.Remove(uv);
                    }

                    Vector2i dv = new Vector2i(v.x, v.y - 1);
                    Tile d = Terrain.TileAt(dv);
                    if (v.x > x && d.enumId != TileID.Air && !processed.Contains(dv)) {
                        processed.Add(dv);
                        Vector2i[] arr = Neighbours(dv);
                        processing.Add(dv);
                        foreach (var a in arr)
                            if (Terrain.TileAt(a).enumId != TileID.Air)
                                processing.Add(a);
                    } else {
                        processing.Remove(dv);
                    }

                }
            }

            return true;
        }

        private Vector2i[] Neighbours(Vector2i v) {
            int x = v.x, y = v.y;
            return new Vector2i[] {
              new Vector2i(x-1,y),
              new Vector2i(x+1,y),
              new Vector2i(x, y+1),
              new Vector2i(x, y-1)
            };
        }

        private class TilePositionComparer : IComparer<Vector2i> {
            public int Compare(Vector2i x, Vector2i y) {
                if (x.x < y.x) return 1;
                else if (x.x == y.x) return 0;
                else return -1;
            }
        }
    }
}
