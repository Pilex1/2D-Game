using Game.Fluids;
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

        private Tile(TileID enumId) : this(enumId, new TileAttribs()) { }
        private Tile(TileID enumId, TileAttribs tileattribs) {
            this.enumId = enumId;
            this.tileattribs = tileattribs;
        }

        #region Special
        public static readonly Tile Invalid = new Tile(TileID.Invalid, new TileAttribs { solid = true, movable = false });
        public static readonly Tile Air = new Tile(TileID.Air, new TileAttribs { solid = false, movable = false, transparent = true });
        public static readonly Tile Bedrock = new Tile(TileID.Bedrock);
        #endregion Special

        #region Normal
        public static readonly Tile Grass = new Tile(TileID.Grass);
        public static readonly Tile Sand = new Tile(TileID.Sand);
        public static readonly Tile Dirt = new Tile(TileID.Dirt);
        public static readonly Tile Wood = new Tile(TileID.Wood);
        public static readonly Tile Leaf = new Tile(TileID.Leaf);
        public static readonly Tile Stone = new Tile(TileID.Stone);
        public static readonly Tile Sandstone = new Tile(TileID.Sandstone);
        public static readonly Tile Sapling = new Tile(TileID.Sapling);
        public static readonly Tile Brick = new Tile(TileID.Brick);
        public static readonly Tile Metal = new Tile(TileID.Metal1);
        public static readonly Tile SmoothSlab = new Tile(TileID.SmoothSlab);
        public static readonly Tile WeatheredStone = new Tile(TileID.WeatheredStone);
        public static readonly Tile FutureMetal = new Tile(TileID.FutureMetal);
        public static readonly Tile Marble = new Tile(TileID.Marble);
        public static readonly Tile PlexSpecial = new Tile(TileID.PlexSpecial);
        public static readonly Tile PurpleStone = new Tile(TileID.PurpleStone);
        public static readonly Tile Cactus = new Tile(TileID.Cactus);
        public static readonly Tile Snow = new Tile(TileID.Snow);
        public static readonly Tile SnowWood = new Tile(TileID.SnowWood);
        public static readonly Tile SnowLeaf = new Tile(TileID.SnowLeaf);
        public static readonly Tile GrassPlant = new Tile(TileID.GrassDeco, new TileAttribs { solid = false });
        #endregion Normal

        #region Logic
        public static Tile Wire() { return new Tile(TileID.WireOff, new WireAttribs()); }
        public static Tile Switch() { return new Tile(TileID.SwitchOff, new SwitchAttribs()); }
        public static Tile LogicLamp() { return new Tile(TileID.LogicLampUnlit, new LogicLampAttribs()); }
        public static Tile GateAnd() { return new Tile(TileID.GateAnd, new AndGateAttribs()); }
        public static Tile GateOr() { return new Tile(TileID.GateOr, new OrGateAttribs()); }
        public static Tile GateNot() { return new Tile(TileID.GateNot, new NotGateAttribs()); }
        public static Tile WireBridge() { return new Tile(TileID.WireBridgeOff, new LogicBridgeAttribs()); }
        public static Tile TilePusher() { return new Tile(TileID.TilePusherOff, new StickyTilePusherAttribs()); }
        public static Tile TilePuller() { return new Tile(TileID.TilePullerOff, new StickyTilePullerAttribs()); }
        public static Tile SingleTilePusher() { return new Tile(TileID.SingleTilePusherOff, new SingleTilePusherAttribs()); }
        public static Tile TileBreaker() { return new Tile(TileID.TileBreakerOff, new TileBreakerAttribs()); }
        public static Tile EntitySpawner() { return new Tile(TileID.EntitySpawnerOff, new EntitySpawnerAttribs()); }

        #endregion Logic

        #region Misc
        public static readonly Tile Bounce = new Tile(TileID.Bounce, new BounceAttribs());
        public static readonly Tile Tnt = new Tile(TileID.Tnt, new ExplosionAttribs { radius = 10, error = 2 });
        public static readonly Tile Nuke = new Tile(TileID.Nuke, new ExplosionAttribs { radius = 50, error = 2 });
        #endregion Misc

        #region Fluids
        public static Tile Water { get { return new Tile(TileID.Water, new WaterAttribs()); } }
        //public static Tile Lava { get { return new Tile(TileEnum.Lava, new LavaAttribs()); } }
        #endregion

        public static Tile Accelerator = new Tile(TileID.Accelerator);

        public static readonly Tile Light = new Tile(TileID.Light, new LightAttribs(16));

        public override string ToString() {
            return enumId.ToString();
        }
    }



}
