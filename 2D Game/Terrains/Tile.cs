using Game.Fluids;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {

    interface ISolid { }

    interface ILightEmitting { }

    interface IRightInteractable {
        void Interact();
    }

    enum TileID {
        Invalid = -1, Air, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt, Sandstone, Sapling, Crate, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, WireOn, WireOff, SwitchOn, SwitchOff, LogicLampUnlit, LogicLampLit, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, GateOr, GateNot, LogicBridgeOff, LogicBridgeHorzVertOn, LogicBridgeHorzOn, LogicBridgeVertOn
    }

    abstract class Tile {
        private TileID _id;
        public TileID id {
            get { return _id; }
            set {
                TileID prevId = _id;
                _id = value;
                if (prevId != _id) Terrain.UpdateMesh = true;
            }
        }
        public int x { get; protected set; }
        public int y { get; protected set; }
        protected Tile(int x, int y, TileID id) {
            this.id = id;
            this.x = x;
            this.y = y;
            if (x < 0 || x >= Terrain.Tiles.GetLength(0) || y < 0 || y >= Terrain.Tiles.GetLength(1)) return;
            if (this is ISolid && !(Terrain.TileAt(x, y) is ISolid)) {
                Terrain.Tiles[x, y] = this;
                Terrain.UpdateMesh = true;
            } else if (this is Fluid && Terrain.TileAt(x, y).id == TileID.Air) {
                Terrain.Tiles[x, y] = this;
                Terrain.UpdateMesh = true;
                FluidsManager.AddFluid((Fluid)this);
            } else {
                Terrain.Tiles[x, y] = this;
                Terrain.UpdateMesh = true;
            }
        }
        public override string ToString() {
            return String.Format("{0} [{1}, {2}]", GetType().ToString(), x, y);
        }
    }

    class Invalid : Tile {
        public Invalid() : base(-1, -1, TileID.Invalid) {
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
    class Snow : Tile, ISolid {
        public Snow(int x, int y) : base(x, y, TileID.Snow) {
        }
    }
    class SnowBiomeWood : Tile, ISolid {
        public SnowBiomeWood(int x, int y) : base(x, y, TileID.SnowWood) {
        }
    }
    class SnowLeaf : Tile, ISolid {
        public SnowLeaf(int x, int y) : base(x, y, TileID.SnowLeaf) {
        }
    }
    class GrassDeco : Tile {
        public GrassDeco(int x, int y) : base(x, y, TileID.GrassDeco) {
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

    abstract class Fluid : Tile {
        public virtual float Height { get; protected set; }
        public Fluid(int x, int y, TileID id, float height) : base(x, y, id) {
            Height = height;
        }
        public void Update() {
            Fall();
            Spread();
        }
        protected abstract void Fall();
        protected abstract void Spread();
    }

    class Water : Fluid {

        private float _height;
        public override float Height {
            get { return _height; }
            protected set {
                if (value > 1) {
                    _height = 1;
                    new Water(x, y + 1, value - 1);
                }
            }
        }

        private const float viscosity = 0.2f;

        public Water(int x, int y, float height = 1f) : base(x, y, TileID.Water, height) {
        }

        protected override void Fall() {
            Tile d = Terrain.TileAt(x, y - 1);

            float amt = viscosity * GameLogic.DeltaTime;
            if (d.id == TileID.Air) {

                if (Height > amt) {
                    Height -= amt;
                    new Water(x, y - 1, amt);
                } else {
                    Terrain.BreakTile(this);
                    new Water(x, y - 1, Height);
                }

            } else if (d.id == TileID.Water) {

                Water w = (Water)d;

                if (Height > amt) {
                    if (w.Height + amt <= 1) {
                        w.Height += amt;
                        Height -= amt;
                    } else {
                        w.Height = 1;
                        Height -= (1 - w.Height);
                    }
                } else {
                    if (w.Height + Height <= 1) {
                        w.Height += Height;
                        Terrain.BreakTile(this);
                    } else {
                        Height -= (1 - w.Height);
                        w.Height = 1;
                    }
                }
            }
        }

        protected override void Spread() {
            float amt = viscosity * GameLogic.DeltaTime;

            Tile l = Terrain.TileAt(x - 1, y), r = Terrain.TileAt(x + 1, y);
            if (l is Air && r is Air) {

                if (Height > amt * 3) {
                    new Water(x - 1, y, amt);
                    new Water(x + 1, y, amt);
                    Height -= amt * 2;
                }

            }
        }


        public override string ToString() {
            return String.Format("Water [{0}, {1}] Height: {2}", x, y, Height);
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
