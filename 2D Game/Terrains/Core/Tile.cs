
using Game.Items;
using Game.Terrains.Logics;
using Game.Terrains.Fluids;
using Game.Terrains.Lightings;
using System;

namespace Game.Terrains {

    [Serializable]
    class Tile {

        #region Init
        public TileID enumId;
        public TileAttribs tileattribs;

        private Tile(TileID enumId, Func<RawItem> dropItem) : this(enumId, new TileAttribs(dropItem)) { }
        private Tile(TileID enumId, TileAttribs tileattribs) {
            this.enumId = enumId;
            this.tileattribs = tileattribs;
        }
        #endregion

        #region Special
        public static readonly Tile Invalid = new Tile(TileID.Invalid, new TileAttribs(() => RawItem.None) { solid = true, movable = false });
        public static readonly Tile Air = new Tile(TileID.Air, new TileAttribs(() => RawItem.None) { solid = false, movable = false, transparent = true });
        public static readonly Tile Bedrock = new Tile(TileID.Bedrock, () => RawItem.None);
        #endregion Special

        #region Normal
        public static readonly Tile Grass = new Tile(TileID.Grass, () => RawItem.Grass);
        public static readonly Tile Sand = new Tile(TileID.Sand, () => RawItem.Sand);
        public static readonly Tile Dirt = new Tile(TileID.Dirt, () => RawItem.Dirt);
        public static readonly Tile Wood = new Tile(TileID.Wood, () => RawItem.Wood);
        public static readonly Tile Leaf = new Tile(TileID.Leaf, () => RawItem.Leaf);
        public static readonly Tile Stone = new Tile(TileID.Stone, () => RawItem.Stone);
        public static readonly Tile Sandstone = new Tile(TileID.Sandstone, () => RawItem.Sandstone);
        public static readonly Tile Sapling = new Tile(TileID.Sapling, () => RawItem.Sapling);
        public static readonly Tile Brick = new Tile(TileID.Brick, () => RawItem.Brick);
        public static readonly Tile Metal = new Tile(TileID.Metal1, () => RawItem.Metal1);
        public static readonly Tile SmoothSlab = new Tile(TileID.SmoothSlab, () => RawItem.SmoothSlab);
        public static readonly Tile WeatheredStone = new Tile(TileID.WeatheredStone, () => RawItem.WeatheredStone);
        public static readonly Tile FutureMetal = new Tile(TileID.FutureMetal, () => RawItem.FutureMetal);
        public static readonly Tile Marble = new Tile(TileID.Marble, () => RawItem.Marble);
        public static readonly Tile PlexSpecial = new Tile(TileID.PlexSpecial, () => RawItem.PlexSpecial);
        public static readonly Tile Obsidian = new Tile(TileID.PurpleStone, () => RawItem.Obsidian);
        public static readonly Tile Cactus = new Tile(TileID.Cactus, () => RawItem.Cactus);
        public static readonly Tile Snow = new Tile(TileID.Snow, () => RawItem.Snow);
        public static readonly Tile SnowWood = new Tile(TileID.SnowWood, () => RawItem.SnowWood);
        public static readonly Tile SnowLeaf = new Tile(TileID.SnowLeaf, () => RawItem.SnowLeaf);
        public static readonly Tile GrassPlant = new Tile(TileID.GrassDeco, new TileAttribs(() => RawItem.GrassDeco) { solid = false });
        #endregion Normal

        #region Logic
        public static Tile Wire() => new Tile(TileID.WireOff, new WireAttribs());
        public static Tile Switch() => new Tile(TileID.SwitchOff, new SwitchAttribs());
        public static Tile LogicLamp() => new Tile(TileID.LogicLampOff, new LogicLampAttribs());
        public static Tile GateAnd() => new Tile(TileID.GateAnd, new AndGateAttribs());
        public static Tile GateOr() => new Tile(TileID.GateOr, new OrGateAttribs());
        public static Tile GateNot() => new Tile(TileID.GateNot, new NotGateAttribs());
        public static Tile WireBridge() => new Tile(TileID.WireBridgeOff, new LogicBridgeAttribs());
        public static Tile TilePusher() => new Tile(TileID.TilePusherOff, new StickyTilePusherAttribs());
        public static Tile TilePuller() => new Tile(TileID.TilePullerOff, new StickyTilePullerAttribs());
        public static Tile SingleTilePusher() => new Tile(TileID.SingleTilePusherOff, new SingleTilePusherAttribs());
        public static Tile TileBreaker() => new Tile(TileID.TileBreakerOff, new TileBreakerAttribs());
        public static Tile EntitySpawner() => new Tile(TileID.EntitySpawnerOff, new EntitySpawnerAttribs());
        public static Tile AutoShooter() => new Tile(TileID.AutoShooterOff, new AutoShooterAttribs());

        #endregion Logic

        #region Misc
        public static readonly Tile Bounce = new Tile(TileID.Bounce, new BounceAttribs());
        public static readonly Tile Tnt = new Tile(TileID.Tnt, new ExplosionAttribs(() => RawItem.Tnt) { radius = 10, error = 2 });
        public static readonly Tile Nuke = new Tile(TileID.Nuke, new ExplosionAttribs(() => RawItem.Nuke) { radius = 50, error = 2 });
        public static readonly Tile WardedTile = new Tile(TileID.WardedTile, new WardedTileAttribs());
        #endregion Misc

        #region Fluids
        public static Tile Water() => new Tile(TileID.Water, new WaterAttribs());
        public static Tile Lava() => new Tile(TileID.Lava, new LavaAttribs());
        public static Tile BounceFluid() => new Tile(TileID.BounceFluid, new BounceFluidAttribs());
        #endregion

        public static Tile Accelerator = new Tile(TileID.Accelerator, () => RawItem.Accelerator);

        public static readonly Tile Light = new Tile(TileID.Light, new LightAttribs());

        public override string ToString() {
            return enumId.ToString();
        }
    }



}
