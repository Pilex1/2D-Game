using System;
using Game.Util;
using Game.Terrains;
using Game.Fluids;

namespace Game.Assets {

    interface ISolid { }

    interface IFluid {
        float Height { get; set; }
        void Flow();
    }

    interface IRightInteractable {
        void Interact();
    }

    enum TileID {
        Air, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt, Sandstone, Sapling, Crate, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water
    }

    abstract class Tile {
        public TileID id { get; protected set; }
        public int x { get; protected set; }
        public int y { get; protected set; }
        protected Tile(int x, int y, TileID id) {
            this.id = id;
            this.x = x;
            this.y = y;
            if (x < 0 || x >= Terrain.Tiles.GetLength(0) || y < 0 || y >= Terrain.Tiles.GetLength(1)) return;
            Terrain.Tiles[x, y] = this;
            Terrain.UpdateMesh = true;
        }
    }

    #region Decoratives
    class Air : Tile {
        public Air(int x, int y) : base(x, y, TileID.Air) {
        }
    }

    class Grass : Tile, ISolid {
        public Grass(int x, int y) : base(x, y, TileID.Grass) {
        }
    }

    class Sand : Tile, ISolid {
        public Sand(int x, int y) : base(x, y, TileID.Sand) {
        }
    }

    class Dirt : Tile, ISolid {
        public Dirt(int x, int y) : base(x, y, TileID.Dirt) {
        }
    }

    class Wood : Tile, ISolid {
        public Wood(int x, int y) : base(x, y, TileID.Wood) {
        }
    }

    class Leaf : Tile, ISolid {
        public Leaf(int x, int y) : base(x, y, TileID.Leaf) {
        }
    }
    class Stone : Tile, ISolid {
        public Stone(int x, int y) : base(x, y, TileID.Stone) {
        }
    }

    class Bedrock : Tile, ISolid {
        public Bedrock(int x, int y) : base(x, y, TileID.Bedrock) {
        }
    }
    class Sandstone : Tile, ISolid {
        public Sandstone(int x, int y) : base(x, y, TileID.Sandstone) {
        }
    }
    class Sapling : Tile {
        public Sapling(int x, int y) : base(x, y, TileID.Sapling) {
        }
    }
    class Crate : Tile, ISolid {
        public Crate(int x, int y) : base(x, y, TileID.Crate) {
        }
    }
    class Brick : Tile, ISolid {
        public Brick(int x, int y) : base(x, y, TileID.Brick) {
        }
    }
    class Metal1 : Tile, ISolid {
        public Metal1(int x, int y) : base(x, y, TileID.Metal1) {
        }
    }
    class SmoothSlab : Tile, ISolid {
        public SmoothSlab(int x, int y) : base(x, y, TileID.SmoothSlab) {
        }
    }
    class WeatheredStone : Tile, ISolid {
        public WeatheredStone(int x, int y) : base(x, y, TileID.WeatheredStone) {
        }
    }
    class Metal2 : Tile, ISolid {
        public Metal2(int x, int y) : base(x, y, TileID.Metal2) {
        }
    }
    class FutureMetal : Tile, ISolid {
        public FutureMetal(int x, int y) : base(x, y, TileID.FutureMetal) {
        }
    }
    class SmoothSlab2 : Tile, ISolid {
        public SmoothSlab2(int x, int y) : base(x, y, TileID.SmoothSlab2) {
        }
    }
    class Marble : Tile, ISolid {
        public Marble(int x, int y) : base(x, y, TileID.Marble) {
        }
    }
    class PlexSpecial : Tile, ISolid {
        public PlexSpecial(int x, int y) : base(x, y, TileID.PlexSpecial) {
        }
    }
    class PurpleStone : Tile, ISolid {
        public PurpleStone(int x, int y) : base(x, y, TileID.PurpleStone) {
        }
    }

    class Cactus : Tile, ISolid {
        public Cactus(int x, int y) : base(x, y, TileID.Cactus) {
        }
    }
    class Bounce : Tile, ISolid {
        public Bounce(int x, int y) : base(x, y, TileID.Bounce) {
        }
    }
    #endregion Decoratives

    #region Explosives

    class Tnt : Tile, ISolid, IRightInteractable {
        private const int explosionRadius = 10;
        private const int explosionError = 2;
        public Tnt(int x, int y) : base(x, y, TileID.Tnt) {
        }

        public void Interact() {
            TileInteract.Explode(x, y, explosionRadius, explosionError);
        }
    }
    class Nuke : Tile, ISolid, IRightInteractable {
        private const int explosionRadius = 50;
        private const int explosionError = 2;
        public Nuke(int x, int y) : base(x, y, TileID.Nuke) {
        }

        public void Interact() {
            TileInteract.Explode(x, y, explosionRadius, explosionError);
        }
    }
    #endregion Explosives

    #region Fluids
    class Water : Tile, IFluid {

        public float Height { get; set; }

        public Water(int x, int y, float height = 1f) : base(x, y, TileID.Water) {
            Height = height;
        }

        public void Flow() {
            Tile d = Terrain.TileAt(x, y - 1);
            if (d.id == TileID.Air) y--;
            //else if (d.id == TileID.Water) {
            //    Water w = (Water)d;
            //    if (w.Height < 2f / 3) w.Height += 1f / 3;
            //    FluidsManager.RemoveFluid(this);
            //}
            else {
                Tile l = Terrain.TileAt(x - 1, y), r = Terrain.TileAt(x + 1, y);
                if (l.id == TileID.Air && r.id == TileID.Air) {
                    float newHeight = Height / 3;
                    FluidsManager.AddFluid(new Water(x - 1, y, newHeight));
                    Height = newHeight;
                    FluidsManager.AddFluid(new Water(x + 1, y, newHeight));
                } else if (l.id == TileID.Air && r.id != TileID.Air) {
                    float newHeight = Height / 2;
                    FluidsManager.AddFluid(new Water(x - 1, y, newHeight));
                    Height = newHeight;
                } else if (l.id != TileID.Air && r.id == TileID.Air) {
                    float newHeight = Height / 2;
                    FluidsManager.AddFluid(new Water(x + 1, y, newHeight));
                    Height = newHeight;
                }
            }
        }
    }
    #endregion Fluids

    static class TileInteract {

        private static Random Rand = new Random();

        internal static void Explode(int x, int y, int radius, int error) {
            for (int i = -radius + MathUtil.RandInt(Rand, -error, error); i <= radius + MathUtil.RandInt(Rand, -error, error); i++) {
                double jStart = -Math.Sqrt(radius * radius - i * i) + MathUtil.RandInt(Rand, -error, error);
                double jEnd = Math.Sqrt(radius * radius - i * i) + MathUtil.RandInt(Rand, -error, error);
                if (jStart == double.NaN || jEnd == double.NaN) continue;
                for (int j = (int)jStart; j <= jEnd; j++) {
                    if (Terrain.BreakTile(x + i, j + y).id == TileID.Tnt) Explode(x + i, j + y, radius, error);
                }
            }
        }
    }
}
