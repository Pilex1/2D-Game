using Game.Terrains;
using System;

namespace Game.Items {
    [Serializable]
    class RawItem {

        public ItemID id;
        public ItemAttribs attribs;

        public RawItem(ItemID id, ItemAttribs itemattribs) {
            this.id = id;
            this.attribs = itemattribs;
        }

        public override string ToString() {
            return id.ToString();
        }

        public static readonly RawItem None = new RawItem(ItemID.None, new NoAttribs("None", 0));

        #region Deco
        public static readonly RawItem PurpleStone = new RawItem(ItemID.PurpleStone, new ItemTileAttribs("Purple Stone", Tile.PurpleStone));
        public static readonly RawItem Grass = new RawItem(ItemID.Grass, new ItemTileAttribs("Grass", Tile.Grass));
        public static readonly RawItem Sand = new RawItem(ItemID.Sand, new ItemTileAttribs("Sand", Tile.Sand));
        public static readonly RawItem Dirt = new RawItem(ItemID.Dirt, new ItemTileAttribs("Dirt", Tile.Dirt));
        public static readonly RawItem Wood = new RawItem(ItemID.Wood, new ItemTileAttribs("Wood", Tile.Wood));
        public static readonly RawItem Leaf = new RawItem(ItemID.Leaf, new ItemTileAttribs("Leaf", Tile.Leaf));
        public static readonly RawItem Stone = new RawItem(ItemID.Stone, new ItemTileAttribs("Stone", Tile.Stone));
        public static readonly RawItem Tnt = new RawItem(ItemID.Tnt, new ItemTileAttribs("TNT", Tile.Tnt));
        public static readonly RawItem Sapling = new RawItem(ItemID.Sapling, new ItemTileAttribs("Sapling", Tile.Sapling));
        public static readonly RawItem Brick = new RawItem(ItemID.Brick, new ItemTileAttribs("Brick", Tile.Brick));
        public static readonly RawItem Metal1 = new RawItem(ItemID.Metal1, new ItemTileAttribs("Metal", Tile.Metal));
        public static readonly RawItem SmoothSlab = new RawItem(ItemID.SmoothSlab, new ItemTileAttribs("Smooth Slab", Tile.SmoothSlab));
        public static readonly RawItem WeatheredStone = new RawItem(ItemID.WeatheredStone, new ItemTileAttribs("Weathered Stone", Tile.WeatheredStone));
        public static readonly RawItem FutureMetal = new RawItem(ItemID.FutureMetal, new ItemTileAttribs("Futuristic Metal", Tile.FutureMetal));
        public static readonly RawItem Marble = new RawItem(ItemID.Marble, new ItemTileAttribs("Marble", Tile.Marble));
        public static readonly RawItem PlexSpecial = new RawItem(ItemID.PlexSpecial, new ItemTileAttribs("Plexico Special", Tile.PlexSpecial));
        public static readonly RawItem Nuke = new RawItem(ItemID.Nuke, new ItemTileAttribs("Nuke", Tile.Nuke));
        public static readonly RawItem Cactus = new RawItem(ItemID.Cactus, new ItemTileAttribs("Cactus", Tile.Cactus));
        public static readonly RawItem Bounce = new RawItem(ItemID.Bounce, new ItemTileAttribs("Bounce", Tile.Bounce));
        public static readonly RawItem Snow = new RawItem(ItemID.Snow, new ItemTileAttribs("Snow", Tile.Snow));
        public static readonly RawItem SnowWood = new RawItem(ItemID.SnowWood, new ItemTileAttribs("Snow Wood", Tile.SnowWood));
        public static readonly RawItem SnowLeaf = new RawItem(ItemID.SnowLeaf, new ItemTileAttribs("Snow Leaf", Tile.SnowLeaf));
        public static readonly RawItem GrassDeco = new RawItem(ItemID.GrassDeco, new ItemTileAttribs("Grass Plant", Tile.GrassPlant));
        public static readonly RawItem Accelerator = new RawItem(ItemID.Accelerator, new ItemTileAttribs("Accelerator", Tile.Accelerator));
        public static readonly RawItem Sandstone = new RawItem(ItemID.Sandstone, new ItemTileAttribs("Sandstone", Tile.Sandstone));
        public static readonly RawItem Light = new RawItem(ItemID.Light, new ItemTileAttribs("Light", Tile.Light));//todo

        #endregion

        #region Logic

        public static readonly RawItem Wire = new RawItem(ItemID.Wire, new ItemTileAttribs("Wire", Tile.Wire));
        public static readonly RawItem Switch = new RawItem(ItemID.Switch, new ItemTileAttribs("Switch", Tile.Switch));
        public static readonly RawItem LogicLamp = new RawItem(ItemID.LogicLamp, new ItemTileAttribs("Logic Lamp", Tile.LogicLamp));
        public static readonly RawItem GateAnd = new RawItem(ItemID.GateAnd, new ItemTileAttribs("AND Gate", Tile.GateAnd));
        public static readonly RawItem GateOr = new RawItem(ItemID.GateOr, new ItemTileAttribs("OR Gate", Tile.GateOr));
        public static readonly RawItem GateNot = new RawItem(ItemID.GateNot, new ItemTileAttribs("NOT Gate", Tile.GateNot));
        public static readonly RawItem WireBridge = new RawItem(ItemID.WireBridge, new ItemTileAttribs("Wire Bridge", Tile.WireBridge));
        public static readonly RawItem StickyTilePusher = new RawItem(ItemID.StickyTilePusher, new ItemTileAttribs("Sticky Tile Pusher", Tile.TilePusher));
        public static readonly RawItem StickyTilePuller = new RawItem(ItemID.StickyTilePuller, new ItemTileAttribs("Sticky Tile Puller", Tile.TilePuller));
        public static readonly RawItem SingleTilePusher = new RawItem(ItemID.SingleTilePusher, new ItemTileAttribs("Single Tile Pusher", Tile.SingleTilePusher));
        public static readonly RawItem TileBreaker = new RawItem(ItemID.TileBreaker, new ItemTileAttribs("Tile Breaker", Tile.TileBreaker));

        #endregion

        #region Fluids
        public static readonly RawItem Water = new RawItem(ItemID.Water, new ItemFluidAttribs("Water", Tile.Water));
        #endregion

        #region Weapons
        public static readonly RawItem Igniter = new RawItem(ItemID.Igniter, new NoAttribs("Igniter", stackSize: 1));
        public static readonly RawItem StaffPurple = new RawItem(ItemID.StaffPurple, new ItemPurpleStaffParticleAttribs());
        public static readonly RawItem StaffGreen = new RawItem(ItemID.StaffGreen, new ItemGreenStaffParticleAttribs());
        public static readonly RawItem StaffRed = new RawItem(ItemID.StaffRed, new ItemRedStaffParticleAttribs());
        public static readonly RawItem StaffBlue = new RawItem(ItemID.StaffBlue, new ItemBlueStaffParticleAttribs());
        public static readonly RawItem StaffYellow = new RawItem(ItemID.StaffYellow, new ItemYellowStaffParticleAttribs());
        #endregion

    }
}
