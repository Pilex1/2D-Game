using System;
using Game.Terrains;
using Game.Particles;
using OpenGL;
using Game.Util;
using Game.Core;
using Game.Interaction;
using Game.Fluids;

namespace Game.Assets {

    [Serializable]
    enum ItemID {
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, TileBreaker, Brick, Metal1, SmoothSlab, WeatheredStone, FutureMetal, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, Wire, Switch, LogicLamp, WireBridge, GateOr, GateNot, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, StickyTilePusher, StickyTilePuller, Igniter, StaffGreen, StaffBlue, StaffRed, StaffPurple, Debugger, Light, Accelerator, Lava, SingleTilePusher, Sandstone, StaffYellow
    }


    [Serializable]
    abstract class ItemAttribs {
        public abstract void Use(Inventory inv, Vector2i invslot, Vector2 position);
        public virtual void BreakTile(Inventory inv, Vector2i position) {
            Tile t = Terrain.BreakTile(position.x, position.y);
            switch (t.enumId) {
                case TileEnum.Invalid:
                case TileEnum.Air:
                case TileEnum.Bedrock:
                    break;

                case TileEnum.Grass: inv.AddItem(RawItem.Grass); break;
                case TileEnum.Sand: inv.AddItem(RawItem.Sand); break;
                case TileEnum.Dirt: inv.AddItem(RawItem.Dirt); break;
                case TileEnum.Wood: inv.AddItem(RawItem.Wood); break;
                case TileEnum.Leaf: inv.AddItem(RawItem.Leaf); break;
                case TileEnum.Stone: inv.AddItem(RawItem.Stone); break;
                case TileEnum.Tnt: inv.AddItem(RawItem.Tnt); break;
                case TileEnum.Sandstone: inv.AddItem(RawItem.Sandstone); break;
                case TileEnum.Sapling: inv.AddItem(RawItem.Sapling); break;
                case TileEnum.Brick: inv.AddItem(RawItem.Brick); break;
                case TileEnum.Metal1: inv.AddItem(RawItem.Metal1); break;
                case TileEnum.SmoothSlab: inv.AddItem(RawItem.SmoothSlab); break;
                case TileEnum.WeatheredStone: inv.AddItem(RawItem.WeatheredStone); break;
                case TileEnum.FutureMetal: inv.AddItem(RawItem.FutureMetal); break;
                case TileEnum.Marble: inv.AddItem(RawItem.Marble); break;
                case TileEnum.PlexSpecial: inv.AddItem(RawItem.PlexSpecial); break;
                case TileEnum.PurpleStone: inv.AddItem(RawItem.PurpleStone); break;
                case TileEnum.Nuke: inv.AddItem(RawItem.Nuke); break;
                case TileEnum.Cactus: inv.AddItem(RawItem.Cactus); break;
                case TileEnum.Bounce: inv.AddItem(RawItem.Bounce); break;
                case TileEnum.Water: break;
                case TileEnum.WireOn: case TileEnum.WireOff: inv.AddItem(RawItem.Wire); break;
                case TileEnum.SwitchOn: case TileEnum.SwitchOff: inv.AddItem(RawItem.Switch); break;
                case TileEnum.LogicLampUnlit: case TileEnum.LogicLampLit: inv.AddItem(RawItem.LogicLamp); break;
                case TileEnum.Snow: inv.AddItem(RawItem.Snow); break;
                case TileEnum.SnowWood: inv.AddItem(RawItem.SnowWood); break;
                case TileEnum.SnowLeaf: inv.AddItem(RawItem.SnowLeaf); break;
                case TileEnum.GrassDeco: inv.AddItem(RawItem.GrassDeco); break;
                case TileEnum.GateAnd: inv.AddItem(RawItem.GateAnd); break;
                case TileEnum.GateOr: inv.AddItem(RawItem.GateOr); break;
                case TileEnum.GateNot: inv.AddItem(RawItem.GateNot); break;
                case TileEnum.WireBridgeOff: case TileEnum.WireBridgeHorzVertOn: case TileEnum.WireBridgeHorzOn: case TileEnum.WireBridgeVertOn: inv.AddItem(RawItem.WireBridge); break;
                case TileEnum.TilePusherOff: case TileEnum.TilePusherOn: inv.AddItem(RawItem.StickyTilePusher); break;
                case TileEnum.TilePullerOn: case TileEnum.TilePullerOff: inv.AddItem(RawItem.StickyTilePuller); break;
                case TileEnum.Light: inv.AddItem(RawItem.Light); break;
                case TileEnum.Accelerator: inv.AddItem(RawItem.Accelerator); break;
                case TileEnum.SingleTilePusherOff: case TileEnum.SingleTilePusherOn: inv.AddItem(RawItem.SingleTilePusher); break;
            }

        }
    }

    [Serializable]
    class NoAttribs : ItemAttribs {
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) { }
    }

    [Serializable]
    class ItemTileAttribs : ItemAttribs {

        private Tile tile;

        public ItemTileAttribs(Tile tile) {
            this.tile = tile;
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile);
        }
    }

    [Serializable]
    class ItemFluidAttribs : ItemAttribs {

        private Tile tile;

        public ItemFluidAttribs(Tile tile) {
            this.tile = tile;
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && !(Terrain.TileAt(x, y).tileattribs is FluidAttribs) && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile);
        }

    }

    [Serializable]
    abstract class ItemStaffParticleAttribs : ItemAttribs {

        protected Vector2 CalculatePos() {
            Vector2 pos = Player.Instance.data.pos.val;
            pos += new Vector2(Player.Instance.hitbox.Size.x / 2, Player.Instance.hitbox.Size.y / 2);
            pos += MathUtil.RandVector2(Program.Rand, new Vector2(-1, -1), new Vector2(1, 1));
            return pos;
        }

        protected Vector2 CalculateVel() {
            Vector2 vel = Input.RayCast().Normalize();
            Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
            vel += playervel;
            return vel;
        }

    }

    [Serializable]
    class ItemRedStaffParticleAttribs : ItemStaffParticleAttribs {
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Destroy.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemPurpleStaffParticleAttribs : ItemStaffParticleAttribs {
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Damage.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemGreenStaffParticleAttribs : ItemStaffParticleAttribs {
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Speed.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemBlueStaffParticleAttribs : ItemStaffParticleAttribs {
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Water.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemYellowStaffParticleAttribs : ItemStaffParticleAttribs {
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Place.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class RawItem {

        public ItemID id;
        public ItemAttribs attribs;

        public RawItem(ItemID id, ItemAttribs itemattribs) {
            this.id = id;
            this.attribs = itemattribs;
        }
        public RawItem(ItemID id) : this(id, new NoAttribs()) { }

        public override string ToString() {
            return id.ToString();
        }

        public static readonly RawItem None = new RawItem(ItemID.None, new NoAttribs());

        #region Deco
        public static readonly RawItem PurpleStone = new RawItem(ItemID.PurpleStone, new ItemTileAttribs(Tile.PurpleStone));
        public static readonly RawItem Grass = new RawItem(ItemID.Grass, new ItemTileAttribs(Tile.Grass));
        public static readonly RawItem Sand = new RawItem(ItemID.Sand, new ItemTileAttribs(Tile.Sand));
        public static readonly RawItem Dirt = new RawItem(ItemID.Dirt, new ItemTileAttribs(Tile.Dirt));
        public static readonly RawItem Wood = new RawItem(ItemID.Wood, new ItemTileAttribs(Tile.Wood));
        public static readonly RawItem Leaf = new RawItem(ItemID.Leaf, new ItemTileAttribs(Tile.Leaf));
        public static readonly RawItem Stone = new RawItem(ItemID.Stone, new ItemTileAttribs(Tile.Stone));
        public static readonly RawItem Tnt = new RawItem(ItemID.Tnt, new ItemTileAttribs(Tile.Tnt));
        public static readonly RawItem Sapling = new RawItem(ItemID.Sapling, new ItemTileAttribs(Tile.Sapling));
        public static readonly RawItem Brick = new RawItem(ItemID.Brick, new ItemTileAttribs(Tile.Brick));
        public static readonly RawItem Metal1 = new RawItem(ItemID.Metal1, new ItemTileAttribs(Tile.Metal1));
        public static readonly RawItem SmoothSlab = new RawItem(ItemID.SmoothSlab, new ItemTileAttribs(Tile.SmoothSlab));
        public static readonly RawItem WeatheredStone = new RawItem(ItemID.WeatheredStone, new ItemTileAttribs(Tile.WeatheredStone));
        public static readonly RawItem FutureMetal = new RawItem(ItemID.FutureMetal, new ItemTileAttribs(Tile.FutureMetal));
        public static readonly RawItem Marble = new RawItem(ItemID.Marble, new ItemTileAttribs(Tile.Marble));
        public static readonly RawItem PlexSpecial = new RawItem(ItemID.PlexSpecial, new ItemTileAttribs(Tile.PlexSpecial));
        public static readonly RawItem Nuke = new RawItem(ItemID.Nuke, new ItemTileAttribs(Tile.Nuke));
        public static readonly RawItem Cactus = new RawItem(ItemID.Cactus, new ItemTileAttribs(Tile.Cactus));
        public static readonly RawItem Bounce = new RawItem(ItemID.Bounce, new ItemTileAttribs(Tile.Bounce));
        public static readonly RawItem Snow = new RawItem(ItemID.Snow, new ItemTileAttribs(Tile.Snow));
        public static readonly RawItem SnowWood = new RawItem(ItemID.SnowWood, new ItemTileAttribs(Tile.SnowWood));
        public static readonly RawItem SnowLeaf = new RawItem(ItemID.SnowLeaf, new ItemTileAttribs(Tile.SnowLeaf));
        public static readonly RawItem GrassDeco = new RawItem(ItemID.GrassDeco, new ItemTileAttribs(Tile.GrassDeco));
        public static readonly RawItem Accelerator = new RawItem(ItemID.Grass, new ItemTileAttribs(Tile.Accelerator));
        public static readonly RawItem Sandstone = new RawItem(ItemID.Sandstone, new ItemTileAttribs(Tile.Sandstone));
        public static readonly RawItem Light = new RawItem(ItemID.Light);

        #endregion

        #region Logic

        public static readonly RawItem Wire = new RawItem(ItemID.Wire, new ItemTileAttribs(Tile.Wire));
        public static readonly RawItem Switch = new RawItem(ItemID.Switch, new ItemTileAttribs(Tile.Switch));
        public static readonly RawItem LogicLamp = new RawItem(ItemID.LogicLamp, new ItemTileAttribs(Tile.LogicLamp));
        public static readonly RawItem GateAnd = new RawItem(ItemID.GateAnd, new ItemTileAttribs(Tile.GateAnd));
        public static readonly RawItem GateOr = new RawItem(ItemID.GateOr, new ItemTileAttribs(Tile.GateOr));
        public static readonly RawItem GateNot = new RawItem(ItemID.GateNot, new ItemTileAttribs(Tile.GateNot));
        public static readonly RawItem WireBridge = new RawItem(ItemID.WireBridge, new ItemTileAttribs(Tile.WireBridge));
        public static readonly RawItem StickyTilePusher = new RawItem(ItemID.StickyTilePusher, new ItemTileAttribs(Tile.TilePusher));
        public static readonly RawItem StickyTilePuller = new RawItem(ItemID.StickyTilePuller, new ItemTileAttribs(Tile.TilePuller));
        public static readonly RawItem SingleTilePusher = new RawItem(ItemID.SingleTilePusher, new ItemTileAttribs(Tile.SingleTilePusher));
        public static readonly RawItem TileBreaker = new RawItem(ItemID.TileBreaker, new ItemTileAttribs(Tile.TileBreaker));

        #endregion

        #region Fluids
        public static readonly RawItem Water = new RawItem(ItemID.Water, new ItemFluidAttribs(Tile.Water));
        #endregion

        #region Weapons
        public static readonly RawItem Igniter = new RawItem(ItemID.Igniter, new NoAttribs());
        public static readonly RawItem StaffPurple = new RawItem(ItemID.StaffPurple, new ItemPurpleStaffParticleAttribs());
        public static readonly RawItem StaffGreen = new RawItem(ItemID.StaffGreen, new ItemGreenStaffParticleAttribs());
        public static readonly RawItem StaffRed = new RawItem(ItemID.StaffRed, new ItemRedStaffParticleAttribs());
        public static readonly RawItem StaffBlue = new RawItem(ItemID.StaffBlue, new ItemBlueStaffParticleAttribs());
        public static readonly RawItem StaffYellow = new RawItem(ItemID.StaffYellow, new ItemYellowStaffParticleAttribs());
        #endregion

    }

    [Serializable]
    class Item {

        public RawItem rawitem;
        public uint amt;

        public Item(RawItem item, uint amt) {
            this.rawitem = item;
            this.amt = amt;
        }

        public override string ToString() {
            return rawitem.ToString() + ": " + amt;
        }
    }

}
