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

        public override string ToString() {
            return "";
        }
    }

    [Serializable]
    class Tile {

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

        private Tile(TileEnum enumId) : this(enumId, new TileAttribs()) { }
        private Tile(TileEnum enumId, TileAttribs tiledata) {
            this.enumId = enumId;
            this.tileattribs = tiledata;
        }

        #region Special
        public static readonly Tile Invalid = new Tile(TileEnum.Invalid, new TileAttribs { solid = false, movable = false });
        public static readonly Tile Air = new Tile(TileEnum.Air, new TileAttribs { solid = false, movable = false, transparent = true });
        public static readonly Tile Bedrock = new Tile(TileEnum.Bedrock);
        #endregion Special

        #region Normal
        public static readonly Tile Grass = new Tile(TileEnum.Grass);
        public static readonly Tile Sand = new Tile(TileEnum.Sand);
        public static readonly Tile Dirt = new Tile(TileEnum.Dirt);
        public static readonly Tile Wood = new Tile(TileEnum.Wood);
        public static readonly Tile Leaf = new Tile(TileEnum.Leaf);
        public static readonly Tile Stone = new Tile(TileEnum.Stone);
        public static readonly Tile Sandstone = new Tile(TileEnum.Sandstone);
        public static readonly Tile Sapling = new Tile(TileEnum.Sapling);
        public static readonly Tile Brick = new Tile(TileEnum.Brick);
        public static readonly Tile Metal1 = new Tile(TileEnum.Metal1);
        public static readonly Tile SmoothSlab = new Tile(TileEnum.SmoothSlab);
        public static readonly Tile WeatheredStone = new Tile(TileEnum.WeatheredStone);
        public static readonly Tile Metal2 = new Tile(TileEnum.Metal2);
        public static readonly Tile FutureMetal = new Tile(TileEnum.FutureMetal);
        public static readonly Tile SmoothSlab2 = new Tile(TileEnum.SmoothSlab2);
        public static readonly Tile Marble = new Tile(TileEnum.Marble);
        public static readonly Tile PlexSpecial = new Tile(TileEnum.PlexSpecial);
        public static readonly Tile PurpleStone = new Tile(TileEnum.PurpleStone);
        public static readonly Tile Cactus = new Tile(TileEnum.Cactus);
        public static readonly Tile Water = new Tile(TileEnum.Water);
        public static readonly Tile Snow = new Tile(TileEnum.Snow);
        public static readonly Tile SnowWood = new Tile(TileEnum.SnowWood);
        public static readonly Tile SnowLeaf = new Tile(TileEnum.SnowLeaf);
        public static readonly Tile GrassDeco = new Tile(TileEnum.GrassDeco, new TileAttribs { solid = false });
        #endregion Normal

        #region Logic
        public static Tile CreateWire() { return new Tile(TileEnum.WireOff, new WireData()); }
        public static Tile CreateSwitch() { return new Tile(TileEnum.SwitchOff, new SwitchData()); }
        public static Tile CreateLogicLamp() { return new Tile(TileEnum.LogicLampUnlit, new LogicLampData()); }
        public static Tile CreateGateAnd() { return new Tile(TileEnum.GateAnd, new AndGateData()); }
        public static Tile CreateGateOr() { return new Tile(TileEnum.GateOr, new OrGateData()); }
        public static Tile CreateGateNot() { return new Tile(TileEnum.GateNot, new NotGateData()); }
        public static Tile CreateLogicBridge() { return new Tile(TileEnum.LogicBridgeOff, new LogicBridgeData()); }
        public static Tile CreateTilePusher() { return new Tile(TileEnum.TilePusherOff, new StickyTilePusherData()); }
        public static Tile CreateTilePuller() { return new Tile(TileEnum.TilePullerOff, new StickyTilePullerData()); }
        public static Tile CreateTileBreaker() { return new Tile(TileEnum.TileBreakerOff, new TileBreakerData()); }
        #endregion Logic

        #region Misc
        public static readonly Tile Bounce = new Tile(TileEnum.Bounce, new BounceAttribs());
        public static readonly Tile Tnt = new Tile(TileEnum.Tnt, new ExplosionData { radius = 10, error = 2 });
        public static readonly Tile Nuke = new Tile(TileEnum.Nuke, new ExplosionData { radius = 50, error = 2 });
        #endregion Misc

        public static readonly Tile Light = new Tile(TileEnum.Light, new LightData(16));

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

    [Serializable]
    class LightData : TileAttribs {
        public int intensity { get; private set; }

        public LightData(int intensity) {
            this.intensity = intensity;
        }
    }



    enum TileEnum {
        Invalid = -1, Air, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt, Sandstone, Sapling, TileBreakerOn, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, WireOn, WireOff, SwitchOn, SwitchOff, LogicLampUnlit, LogicLampLit, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, GateOr, GateNot, LogicBridgeOff, LogicBridgeHorzVertOn, LogicBridgeHorzOn, LogicBridgeVertOn, TilePusherOff, TilePusherOn, TilePullerOn, TilePullerOff, TileBreakerOff, Light
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
