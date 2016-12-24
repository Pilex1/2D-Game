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

        public static readonly RawItem None = new RawItem(ItemID.None, new Item_No_Attribs("None", 0));

        #region Deco
        public static readonly RawItem PurpleStone = new RawItem(ItemID.PurpleStone, new Item_Tile_Attribs("Purple Stone", Tile.PurpleStone));
        public static readonly RawItem Grass = new RawItem(ItemID.Grass, new Item_Tile_Attribs("Grass", Tile.Grass));
        public static readonly RawItem Sand = new RawItem(ItemID.Sand, new Item_Tile_Attribs("Sand", Tile.Sand));
        public static readonly RawItem Dirt = new RawItem(ItemID.Dirt, new Item_Tile_Attribs("Dirt", Tile.Dirt));
        public static readonly RawItem Wood = new RawItem(ItemID.Wood, new Item_Tile_Attribs("Wood", Tile.Wood));
        public static readonly RawItem Leaf = new RawItem(ItemID.Leaf, new Item_Tile_Attribs("Leaf", Tile.Leaf));
        public static readonly RawItem Stone = new RawItem(ItemID.Stone, new Item_Tile_Attribs("Stone", Tile.Stone));
        public static readonly RawItem Tnt = new RawItem(ItemID.Tnt, new Item_Tile_Attribs("TNT", Tile.Tnt));
        public static readonly RawItem Sapling = new RawItem(ItemID.Sapling, new Item_Tile_Attribs("Sapling", Tile.Sapling));
        public static readonly RawItem Brick = new RawItem(ItemID.Brick, new Item_Tile_Attribs("Brick", Tile.Brick));
        public static readonly RawItem Metal1 = new RawItem(ItemID.Metal1, new Item_Tile_Attribs("Metal", Tile.Metal));
        public static readonly RawItem SmoothSlab = new RawItem(ItemID.SmoothSlab, new Item_Tile_Attribs("Smooth Slab", Tile.SmoothSlab));
        public static readonly RawItem WeatheredStone = new RawItem(ItemID.WeatheredStone, new Item_Tile_Attribs("Weathered Stone", Tile.WeatheredStone));
        public static readonly RawItem FutureMetal = new RawItem(ItemID.FutureMetal, new Item_Tile_Attribs("Futuristic Metal", Tile.FutureMetal));
        public static readonly RawItem Marble = new RawItem(ItemID.Marble, new Item_Tile_Attribs("Marble", Tile.Marble));
        public static readonly RawItem PlexSpecial = new RawItem(ItemID.PlexSpecial, new Item_Tile_Attribs("Plexico Special", Tile.PlexSpecial));
        public static readonly RawItem Nuke = new RawItem(ItemID.Nuke, new Item_Tile_Attribs("Nuke", Tile.Nuke));
        public static readonly RawItem Cactus = new RawItem(ItemID.Cactus, new Item_Tile_Attribs("Cactus", Tile.Cactus));
        public static readonly RawItem Bounce = new RawItem(ItemID.Bounce, new Item_Tile_Attribs("Bounce", Tile.Bounce));
        public static readonly RawItem Snow = new RawItem(ItemID.Snow, new Item_Tile_Attribs("Snow", Tile.Snow));
        public static readonly RawItem SnowWood = new RawItem(ItemID.SnowWood, new Item_Tile_Attribs("Snow Wood", Tile.SnowWood));
        public static readonly RawItem SnowLeaf = new RawItem(ItemID.SnowLeaf, new Item_Tile_Attribs("Snow Leaf", Tile.SnowLeaf));
        public static readonly RawItem GrassDeco = new RawItem(ItemID.GrassDeco, new Item_Tile_Attribs("Grass Plant", Tile.GrassPlant));
        public static readonly RawItem Accelerator = new RawItem(ItemID.Accelerator, new Item_Tile_Attribs("Accelerator", Tile.Accelerator));
        public static readonly RawItem Sandstone = new RawItem(ItemID.Sandstone, new Item_Tile_Attribs("Sandstone", Tile.Sandstone));
        public static readonly RawItem Light = new RawItem(ItemID.Light, new Item_Tile_Attribs("Light", Tile.Light));//todo

        #endregion

        #region Logic

        public static readonly RawItem Wire = new RawItem(ItemID.Wire, new Item_DataTile_Attribs("Wire", Tile.Wire));
        public static readonly RawItem Switch = new RawItem(ItemID.Switch, new Item_DataTile_Attribs("Switch", Tile.Switch));
        public static readonly RawItem LogicLamp = new RawItem(ItemID.LogicLamp, new Item_DataTile_Attribs("Logic Lamp", Tile.LogicLamp));
        public static readonly RawItem GateAnd = new RawItem(ItemID.GateAnd, new Item_DirectionalTile_Attribs("AND Gate", Tile.GateAnd));
        public static readonly RawItem GateOr = new RawItem(ItemID.GateOr, new Item_DirectionalTile_Attribs("OR Gate", Tile.GateOr));
        public static readonly RawItem GateNot = new RawItem(ItemID.GateNot, new Item_DirectionalTile_Attribs("NOT Gate", Tile.GateNot));
        public static readonly RawItem WireBridge = new RawItem(ItemID.WireBridge, new Item_DataTile_Attribs("Wire Bridge", Tile.WireBridge));
        public static readonly RawItem StickyTilePusher = new RawItem(ItemID.StickyTilePusher, new Item_DirectionalTile_Attribs("Sticky Tile Pusher", Tile.TilePusher));
        public static readonly RawItem StickyTilePuller = new RawItem(ItemID.StickyTilePuller, new Item_DirectionalTile_Attribs("Sticky Tile Puller", Tile.TilePuller));
        public static readonly RawItem SingleTilePusher = new RawItem(ItemID.SingleTilePusher, new Item_DirectionalTile_Attribs("Single Tile Pusher", Tile.SingleTilePusher));
        public static readonly RawItem TileBreaker = new RawItem(ItemID.TileBreaker, new Item_DirectionalTile_Attribs("Tile Breaker", Tile.TileBreaker));
        public static readonly RawItem EntitySpawner = new RawItem(ItemID.EntitySpawner, new Item_DataTile_Attribs("Entity Spawner", Tile.EntitySpawner));
        public static readonly RawItem AutoShooter = new RawItem(ItemID.AutoShooter, new Item_DataTile_Attribs("Auto Shooter", Tile.AutoShooter));
        #endregion

        public static readonly RawItem WardedTile = new RawItem(ItemID.WardedTile, new Item_Tile_Attribs("Warded Tile", Tile.WardedTile));

        #region Fluids
        public static readonly RawItem Water = new RawItem(ItemID.Water, new Item_Fluid_Attribs("Water", Tile.Water));
        #endregion

        #region Weapons
        public static readonly RawItem Igniter = new RawItem(ItemID.Igniter, new Item_No_Attribs("Igniter", stackSize: 1));
        public static readonly RawItem StaffPurple = new RawItem(ItemID.StaffPurple, new Item_PurpleStaff_Attribs());
        public static readonly RawItem StaffGreen = new RawItem(ItemID.StaffGreen, new Item_GreenStaff_Attribs());
        public static readonly RawItem StaffRed = new RawItem(ItemID.StaffRed, new Item_RedStaff_Attribs());
        public static readonly RawItem StaffBlue = new RawItem(ItemID.StaffBlue, new Item_BlueStaff_Attribs());
        public static readonly RawItem StaffYellow = new RawItem(ItemID.StaffYellow, new Item_YellowStaff_Attribs());
        #endregion

    }
}
