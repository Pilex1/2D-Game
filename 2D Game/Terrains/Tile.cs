using Game.Fluids;
using Game.Items;
using Game.Logics;
using System;

namespace Game.Terrains {

    [Serializable]
    class Tile {

        private TileID _enumId;
        public TileID enumId {
            get { return _enumId; }
            set {
                TileID original = _enumId;
                _enumId = value;
                if (_enumId != original) Terrain.UpdateMesh = true;
            }
        }
        public TileAttribs tileattribs;

        private Tile(TileID enumId, Func<RawItem> dropItem) : this(enumId, new TileAttribs(dropItem)) { }
        private Tile(TileID enumId, TileAttribs tileattribs) {
            this.enumId = enumId;
            this.tileattribs = tileattribs;
        }

        #region Special
        public static readonly Tile Invalid = new Tile(TileID.Invalid, new TileAttribs(delegate () { return RawItem.None; }) { solid = true, movable = false });
        public static readonly Tile Air = new Tile(TileID.Air, new TileAttribs(delegate () { return RawItem.None; }) { solid = false, movable = false, transparent = true });
        public static readonly Tile Bedrock = new Tile(TileID.Bedrock, delegate () { return RawItem.None; });
        #endregion Special

        #region Normal
        public static readonly Tile Grass = new Tile(TileID.Grass, delegate () { return RawItem.Grass; });
        public static readonly Tile Sand = new Tile(TileID.Sand, delegate () { return RawItem.Sand; });
        public static readonly Tile Dirt = new Tile(TileID.Dirt, delegate () { return RawItem.Dirt; });
        public static readonly Tile Wood = new Tile(TileID.Wood, delegate () { return RawItem.Wood; });
        public static readonly Tile Leaf = new Tile(TileID.Leaf, delegate () { return RawItem.Leaf; });
        public static readonly Tile Stone = new Tile(TileID.Stone, delegate () { return RawItem.Stone; });
        public static readonly Tile Sandstone = new Tile(TileID.Sandstone, delegate () { return RawItem.Sandstone; });
        public static readonly Tile Sapling = new Tile(TileID.Sapling, delegate () { return RawItem.Sapling; });
        public static readonly Tile Brick = new Tile(TileID.Brick, delegate () { return RawItem.Brick; });
        public static readonly Tile Metal = new Tile(TileID.Metal1, delegate () { return RawItem.Metal1; });
        public static readonly Tile SmoothSlab = new Tile(TileID.SmoothSlab, delegate () { return RawItem.SmoothSlab; });
        public static readonly Tile WeatheredStone = new Tile(TileID.WeatheredStone, delegate () { return RawItem.WeatheredStone; });
        public static readonly Tile FutureMetal = new Tile(TileID.FutureMetal, delegate () { return RawItem.FutureMetal; });
        public static readonly Tile Marble = new Tile(TileID.Marble, delegate () { return RawItem.Marble; });
        public static readonly Tile PlexSpecial = new Tile(TileID.PlexSpecial, delegate () { return RawItem.PlexSpecial; });
        public static readonly Tile PurpleStone = new Tile(TileID.PurpleStone, delegate () { return RawItem.PurpleStone; });
        public static readonly Tile Cactus = new Tile(TileID.Cactus, delegate () { return RawItem.Cactus; });
        public static readonly Tile Snow = new Tile(TileID.Snow, delegate () { return RawItem.Snow; });
        public static readonly Tile SnowWood = new Tile(TileID.SnowWood, delegate () { return RawItem.SnowWood; });
        public static readonly Tile SnowLeaf = new Tile(TileID.SnowLeaf, delegate () { return RawItem.SnowLeaf; });
        public static readonly Tile GrassPlant = new Tile(TileID.GrassDeco, new TileAttribs(delegate () { return RawItem.GrassDeco; }) { solid = false });
        #endregion Normal

        #region Logic
        public static Tile Wire() { return new Tile(TileID.WireOff, new WireAttribs()); }
        public static Tile Switch() { return new Tile(TileID.SwitchOff, new SwitchAttribs()); }
        public static Tile LogicLamp() { return new Tile(TileID.LogicLampOff, new LogicLampAttribs()); }
        public static Tile GateAnd() { return new Tile(TileID.GateAnd, new AndGateAttribs()); }
        public static Tile GateOr() { return new Tile(TileID.GateOr, new OrGateAttribs()); }
        public static Tile GateNot() { return new Tile(TileID.GateNot, new NotGateAttribs()); }
        public static Tile WireBridge() { return new Tile(TileID.WireBridgeOff, new LogicBridgeAttribs()); }
        public static Tile TilePusher() { return new Tile(TileID.TilePusherOff, new StickyTilePusherAttribs()); }
        public static Tile TilePuller() { return new Tile(TileID.TilePullerOff, new StickyTilePullerAttribs()); }
        public static Tile SingleTilePusher() { return new Tile(TileID.SingleTilePusherOff, new SingleTilePusherAttribs()); }
        public static Tile TileBreaker() { return new Tile(TileID.TileBreakerOff, new TileBreakerAttribs()); }
        public static Tile EntitySpawner() { return new Tile(TileID.EntitySpawnerOff, new EntitySpawnerAttribs()); }
        public static Tile AutoShooter() { return new Tile(TileID.AutoShooterOff, new AutoShooterAttribs()); }

        #endregion Logic

        #region Misc
        public static readonly Tile Bounce = new Tile(TileID.Bounce, new BounceAttribs());
        public static readonly Tile Tnt = new Tile(TileID.Tnt, new ExplosionAttribs(delegate () { return RawItem.Tnt; }) { radius = 10, error = 2 });
        public static readonly Tile Nuke = new Tile(TileID.Nuke, new ExplosionAttribs(delegate () { return RawItem.Nuke; }) { radius = 50, error = 2 });
        public static readonly Tile WardedTile = new Tile(TileID.WardedTile, new WardedTileAttribs());
        #endregion Misc

        #region Fluids
        public static Tile Water() { return new Tile(TileID.Water, new WaterAttribs()); }
        //public static Tile Lava { get { return new Tile(TileEnum.Lava, new LavaAttribs()); } }
        #endregion

        public static Tile Accelerator = new Tile(TileID.Accelerator, delegate () { return RawItem.Accelerator; });

        public static readonly Tile Light = new Tile(TileID.Light, new LightAttribs(delegate () { return RawItem.Light; }, 16));

        public override string ToString() {
            return enumId.ToString();
        }
    }



}
