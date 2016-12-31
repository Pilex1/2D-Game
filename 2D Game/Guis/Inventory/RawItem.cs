using Game.Entities;
using Game.Entities.Particles;
using Game.Terrains;
using Game.Util;
using OpenGL;
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

        public static readonly RawItem None = new RawItem(ItemID.None, new ItemAttribs_Empty("None", 0));

        #region Deco
        public static readonly RawItem Obsidian = new RawItem(ItemID.Obsidian, new ItemAttribs_Tile("Obsidian", () => Tile.Obsidian));
        public static readonly RawItem Grass = new RawItem(ItemID.Grass, new ItemAttribs_Tile("Grass", () => Tile.Grass));
        public static readonly RawItem Sand = new RawItem(ItemID.Sand, new ItemAttribs_Tile("Sand", () => Tile.Sand));
        public static readonly RawItem Dirt = new RawItem(ItemID.Dirt, new ItemAttribs_Tile("Dirt", () => Tile.Dirt));
        public static readonly RawItem Wood = new RawItem(ItemID.Wood, new ItemAttribs_Tile("Wood", () => Tile.Wood));
        public static readonly RawItem Leaf = new RawItem(ItemID.Leaf, new ItemAttribs_Tile("Leaf", () => Tile.Leaf));
        public static readonly RawItem Stone = new RawItem(ItemID.Stone, new ItemAttribs_Tile("Stone", () => Tile.Stone));
        public static readonly RawItem Tnt = new RawItem(ItemID.Tnt, new ItemAttribs_Tile("TNT", () => Tile.Tnt));
        public static readonly RawItem Sapling = new RawItem(ItemID.Sapling, new ItemAttribs_Tile("Sapling", () => Tile.Sapling));
        public static readonly RawItem Brick = new RawItem(ItemID.Brick, new ItemAttribs_Tile("Brick", () => Tile.Brick));
        public static readonly RawItem Metal1 = new RawItem(ItemID.Metal1, new ItemAttribs_Tile("Metal", () => Tile.Metal));
        public static readonly RawItem SmoothSlab = new RawItem(ItemID.SmoothSlab, new ItemAttribs_Tile("Smooth Slab", () => Tile.SmoothSlab));
        public static readonly RawItem WeatheredStone = new RawItem(ItemID.WeatheredStone, new ItemAttribs_Tile("Weathered Stone", () => Tile.WeatheredStone));
        public static readonly RawItem FutureMetal = new RawItem(ItemID.FutureMetal, new ItemAttribs_Tile("Futuristic Metal", () => Tile.FutureMetal));
        public static readonly RawItem Marble = new RawItem(ItemID.Marble, new ItemAttribs_Tile("Marble", () => Tile.Marble));
        public static readonly RawItem PlexSpecial = new RawItem(ItemID.PlexSpecial, new ItemAttribs_Tile("Plexico Special", () => Tile.PlexSpecial));
        public static readonly RawItem Nuke = new RawItem(ItemID.Nuke, new ItemAttribs_Tile("Nuke", () => Tile.Nuke));
        public static readonly RawItem Cactus = new RawItem(ItemID.Cactus, new ItemAttribs_Tile("Cactus", () => Tile.Cactus));
        public static readonly RawItem Bounce = new RawItem(ItemID.Bounce, new ItemAttribs_Tile("Bounce", () => Tile.Bounce));
        public static readonly RawItem Snow = new RawItem(ItemID.Snow, new ItemAttribs_Tile("Snow", () => Tile.Snow));
        public static readonly RawItem SnowWood = new RawItem(ItemID.SnowWood, new ItemAttribs_Tile("Snow Wood", () => Tile.SnowWood));
        public static readonly RawItem SnowLeaf = new RawItem(ItemID.SnowLeaf, new ItemAttribs_Tile("Snow Leaf", () => Tile.SnowLeaf));
        public static readonly RawItem GrassDeco = new RawItem(ItemID.GrassDeco, new ItemAttribs_Tile("Grass Plant", () => Tile.GrassPlant));
        public static readonly RawItem Accelerator = new RawItem(ItemID.Accelerator, new ItemAttribs_Tile("Accelerator", () => Tile.Accelerator));
        public static readonly RawItem Sandstone = new RawItem(ItemID.Sandstone, new ItemAttribs_Tile("Sandstone", () => Tile.Sandstone));

        #endregion

        #region Lights
        public static readonly RawItem Light = new RawItem(ItemID.Light, new ItemAttribs_Tile("Light", () => Tile.Light));

        #endregion

        #region Logic

        public static readonly RawItem Wire = new RawItem(ItemID.Wire, new ItemAttribs_Tile("Wire", Tile.Wire));
        public static readonly RawItem Switch = new RawItem(ItemID.Switch, new ItemAttribs_Tile("Switch", Tile.Switch));
        public static readonly RawItem LogicLamp = new RawItem(ItemID.LogicLamp, new ItemAttribs_Tile("Logic Lamp", Tile.LogicLamp));
        public static readonly RawItem GateAnd = new RawItem(ItemID.GateAnd, new ItemAttribs_DirectionalTile("AND Gate", Tile.GateAnd));
        public static readonly RawItem GateOr = new RawItem(ItemID.GateOr, new ItemAttribs_DirectionalTile("OR Gate", Tile.GateOr));
        public static readonly RawItem GateNot = new RawItem(ItemID.GateNot, new ItemAttribs_DirectionalTile("NOT Gate", Tile.GateNot));
        public static readonly RawItem WireBridge = new RawItem(ItemID.WireBridge, new ItemAttribs_Tile("Wire Bridge", Tile.WireBridge));
        public static readonly RawItem StickyTilePusher = new RawItem(ItemID.StickyTilePusher, new ItemAttribs_DirectionalTile("Sticky Tile Pusher", Tile.TilePusher));
        public static readonly RawItem StickyTilePuller = new RawItem(ItemID.StickyTilePuller, new ItemAttribs_DirectionalTile("Sticky Tile Puller", Tile.TilePuller));
        public static readonly RawItem SingleTilePusher = new RawItem(ItemID.SingleTilePusher, new ItemAttribs_DirectionalTile("Single Tile Pusher", Tile.SingleTilePusher));
        public static readonly RawItem TileBreaker = new RawItem(ItemID.TileBreaker, new ItemAttribs_DirectionalTile("Tile Breaker", Tile.TileBreaker));
        public static readonly RawItem EntitySpawner = new RawItem(ItemID.EntitySpawner, new ItemAttribs_Tile("Entity Spawner", Tile.EntitySpawner));
        public static readonly RawItem AutoShooter = new RawItem(ItemID.AutoShooter, new ItemAttribs_Tile("Auto Shooter", Tile.AutoShooter));
        #endregion

        public static readonly RawItem WardedTile = new RawItem(ItemID.WardedTile, new ItemAttribs_Tile("Warded Tile", () => Tile.WardedTile));

        #region Fluids
        public static readonly RawItem Water = new RawItem(ItemID.Water, new ItemAttribs_Fluids("Water", Tile.Water));
        public static readonly RawItem Lava = new RawItem(ItemID.Lava, new ItemAttribs_Fluids("Lava", Tile.Lava));
        public static readonly RawItem BounceFluid = new RawItem(ItemID.BounceFluid, new ItemAttribs_Fluids("Bounce Fluid", Tile.BounceFluid));
        #endregion

        #region Weapons
        public static readonly RawItem Igniter = new RawItem(ItemID.Igniter, new ItemAttribs_Empty("Igniter", stackSize: 1));
        public static readonly RawItem StaffPurple = new RawItem(ItemID.StaffPurple, new Item_PurpleStaff_Attribs());
        public static readonly RawItem StaffGreen = new RawItem(ItemID.StaffGreen, new Item_GreenStaff_Attribs());
        public static readonly RawItem StaffRed = new RawItem(ItemID.StaffRed, new Item_RedStaff_Attribs());
        public static readonly RawItem StaffBlue = new RawItem(ItemID.StaffBlue, new Item_BlueStaff_Attribs());
        public static readonly RawItem StaffYellow = new RawItem(ItemID.StaffYellow, new Item_YellowStaff_Attribs());
        public static readonly RawItem Firework = new RawItem(ItemID.Firework, new ItemAttribs_OverideableUse(999, "Fireworks", (Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) => {
            if (Array.Exists(EntityManager.GetEntitiesAt(position), e => e is FireworkLauncher || e is FireworkParticle)) return;
            Vector4 colour = new Vector4(MathUtil.RandVector3(Program.Rand, 0, 1), 0.3f);
            FireworkLauncher f = new FireworkLauncher(position, colour, 50);
            EntityManager.AddEntity(f);
            inv.RemoveItem(invslot.x, invslot.y);
        }));
        #endregion

    }
}
