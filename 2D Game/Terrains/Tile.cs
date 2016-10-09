using Game.Fluids;
using Game.Logics;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {

    [Serializable]
    class TileAttribs {
        public bool solid = true;
        public bool movable = true;
        public bool lightEmitting = false;
        public bool transparent = false;
        public Direction rotation = Direction.Up;

        public virtual void Interact(int x, int y) { }
        public virtual bool OnTerrainIntersect(int x, int y, Direction side, Entity e) {
            switch (side) {
                case Direction.Up:
                    e.data.Position.y = (int)Math.Ceiling(e.data.Position.y);
                    e.data.vel.y = 0;
                    break;
                case Direction.Down:
                    e.data.Position.y = (int)Math.Floor(e.data.Position.y);
                    e.data.vel.y = 0;
                    e.data.InAir = false;
                    break;
                case Direction.Left:
                    e.data.Position.x = (int)Math.Floor(e.data.Position.x);
                    e.data.vel.x = 0;
                    break;
                case Direction.Right:
                    e.data.Position.x = (int)Math.Ceiling(e.data.Position.x - 0.01);
                    e.data.vel.x = 0;
                    break;
            }
            return true;
        }
    }

    [Serializable]
    class TileID {

        private TileEnum _enumId;
        public TileEnum enumId {
            get { return _enumId; }
            set {
                TileEnum original = _enumId;
                _enumId = value;
                if (_enumId != original) Terrain.UpdateMesh = true;
            }
        }
        public TileAttribs tileattribs;

        private TileID(TileEnum enumId) : this(enumId, new TileAttribs()) { }
        private TileID(TileEnum enumId, TileAttribs tiledata) {
            this.enumId = enumId;
            this.tileattribs = tiledata;
        }

        #region Special
        public static readonly TileID Invalid = new TileID(TileEnum.Invalid, new TileAttribs { solid = false, movable = false });
        public static readonly TileID Air = new TileID(TileEnum.Air, new TileAttribs { solid = false, movable = false, transparent = true });
        public static readonly TileID Bedrock = new TileID(TileEnum.Bedrock);
        #endregion Special


        #region Normal
        public static readonly TileID Grass = new TileID(TileEnum.Grass);
        public static readonly TileID Sand = new TileID(TileEnum.Sand);
        public static readonly TileID Dirt = new TileID(TileEnum.Dirt);
        public static readonly TileID Wood = new TileID(TileEnum.Wood);
        public static readonly TileID Leaf = new TileID(TileEnum.Leaf);
        public static readonly TileID Stone = new TileID(TileEnum.Stone);
        public static readonly TileID Sandstone = new TileID(TileEnum.Sandstone);
        public static readonly TileID Sapling = new TileID(TileEnum.Sapling);
        public static readonly TileID Brick = new TileID(TileEnum.Brick);
        public static readonly TileID Metal1 = new TileID(TileEnum.Metal1);
        public static readonly TileID SmoothSlab = new TileID(TileEnum.SmoothSlab);
        public static readonly TileID WeatheredStone = new TileID(TileEnum.WeatheredStone);
        public static readonly TileID Metal2 = new TileID(TileEnum.Metal2);
        public static readonly TileID FutureMetal = new TileID(TileEnum.FutureMetal);
        public static readonly TileID SmoothSlab2 = new TileID(TileEnum.SmoothSlab2);
        public static readonly TileID Marble = new TileID(TileEnum.Marble);
        public static readonly TileID PlexSpecial = new TileID(TileEnum.PlexSpecial);
        public static readonly TileID PurpleStone = new TileID(TileEnum.PurpleStone);
        public static readonly TileID Cactus = new TileID(TileEnum.Cactus);
        public static readonly TileID Water = new TileID(TileEnum.Water);
        public static readonly TileID Snow = new TileID(TileEnum.Snow);
        public static readonly TileID SnowWood = new TileID(TileEnum.SnowWood);
        public static readonly TileID SnowLeaf = new TileID(TileEnum.SnowLeaf);
        public static readonly TileID GrassDeco = new TileID(TileEnum.GrassDeco, new TileAttribs { solid = false });
        #endregion Normal

        #region Logic
        public static TileID CreateWire() { return new TileID(TileEnum.WireOff, new WireData()); }
        public static TileID CreateSwitch() { return new TileID(TileEnum.SwitchOff, new SwitchData()); }
        public static TileID CreateLogicLamp() { return new TileID(TileEnum.LogicLampUnlit, new LogicLampData()); }
        public static TileID CreateGateAnd() { return new TileID(TileEnum.GateAnd, new AndGateData()); }
        public static TileID CreateGateOr() { return new TileID(TileEnum.GateOr, new OrGateData()); }
        public static TileID CreateGateNot() { return new TileID(TileEnum.GateNot, new NotGateData()); }
        public static TileID CreateLogicBridge() { return new TileID(TileEnum.LogicBridgeOff, new LogicBridgeData()); }
        public static TileID CreateTilePusher() { return new TileID(TileEnum.TilePusherOff, new StickyTilePusherData()); }
        public static TileID CreateTilePuller() { return new TileID(TileEnum.TilePullerOff, new StickyTilePullerData()); }
        public static TileID CreateTileBreaker() { return new TileID(TileEnum.TileBreakerOff, new TileBreakerData()); }
        #endregion Logic

        #region Misc
        public static readonly TileID Bounce = new TileID(TileEnum.Bounce, new BounceAttribs());
        public static readonly TileID Tnt = new TileID(TileEnum.Tnt, new ExplosionData { radius = 10, error = 2 });
        public static readonly TileID Nuke = new TileID(TileEnum.Nuke, new ExplosionData { radius = 50, error = 2 });
        #endregion Misc



        public override string ToString() {
            return enumId.ToString();
        }
    }

    [Serializable]
    class BounceAttribs : TileAttribs {

        float bouncePowerVert = -1.2f;
        float bouncePowerHorz = -1.8f;

        public override bool OnTerrainIntersect(int x, int y, Direction side, Entity e) {
            if (side == Direction.Up || side == Direction.Down) {
                e.data.vel.y *= bouncePowerVert;
            }
            if (side == Direction.Left || side == Direction.Right) {
                e.data.vel.x *= bouncePowerHorz;
            }
            return true;
        }
    }


    enum TileEnum {
        Invalid = -1, Air, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt, Sandstone, Sapling, TileBreakerOn, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, WireOn, WireOff, SwitchOn, SwitchOff, LogicLampUnlit, LogicLampLit, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, GateOr, GateNot, LogicBridgeOff, LogicBridgeHorzVertOn, LogicBridgeHorzOn, LogicBridgeVertOn, TilePusherOff, TilePusherOn, TilePullerOn, TilePullerOff, TileBreakerOff
    }

    #region Fluids

    //abstract class Fluid : Tile {
    //    public virtual float Height { get; protected set; }
    //    public Fluid(int x, int y, TileEnum id, float height) : base(x, y, id) {
    //        Height = height;
    //    }
    //    public void Update() {
    //        Fall();
    //        Spread();
    //    }
    //    protected abstract void Fall();
    //    protected abstract void Spread();
    //}

    //class Water : Fluid {

    //    private float _height;
    //    public override float Height {
    //        get { return _height; }
    //        protected set {
    //            if (value > 1) {
    //                _height = 1;
    //                new Water(x, y + 1, value - 1);
    //            }
    //        }
    //    }

    //    private const float viscosity = 0.2f;

    //    public Water(int x, int y, float height = 1f) : base(x, y, TileEnum.Water, height) {
    //    }

    //    protected override void Fall() {
    //        Tile d = Terrain.TileAt(x, y - 1);

    //        float amt = viscosity * GameLogic.DeltaTime;
    //        if (d.id == TileEnum.Air) {

    //            if (Height > amt) {
    //                Height -= amt;
    //                new Water(x, y - 1, amt);
    //            } else {
    //                Terrain.BreakTile(this);
    //                new Water(x, y - 1, Height);
    //            }

    //        } else if (d.id == TileEnum.Water) {

    //            Water w = (Water)d;

    //            if (Height > amt) {
    //                if (w.Height + amt <= 1) {
    //                    w.Height += amt;
    //                    Height -= amt;
    //                } else {
    //                    w.Height = 1;
    //                    Height -= (1 - w.Height);
    //                }
    //            } else {
    //                if (w.Height + Height <= 1) {
    //                    w.Height += Height;
    //                    Terrain.BreakTile(this);
    //                } else {
    //                    Height -= (1 - w.Height);
    //                    w.Height = 1;
    //                }
    //            }
    //        }
    //    }

    //    protected override void Spread() {
    //        float amt = viscosity * GameLogic.DeltaTime;

    //        Tile l = Terrain.TileAt(x - 1, y), r = Terrain.TileAt(x + 1, y);
    //        if (l is Air && r is Air) {

    //            if (Height > amt * 3) {
    //                new Water(x - 1, y, amt);
    //                new Water(x + 1, y, amt);
    //                Height -= amt * 2;
    //            }

    //        }
    //    }


    //    public override string ToString() {
    //        return String.Format("Water [{0}, {1}] Height: {2}", x, y, Height);
    //    }
    //}
    #endregion Fluids

}
