using Game.Core;
using Game.Entities;
using Game.Fluids;
using Game.Interaction;
using Game.Particles;
using Game.Terrains;
using Game.Util;
using OpenGL;
using System;

namespace Game.Items {
    [Serializable]
    abstract class ItemAttribs {

        public int stackSize { get; private set; }
        public string name;

        public ItemAttribs(int stackSize, string name) {
            this.stackSize = stackSize;
            this.name = name;
        }

        public abstract void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction);
        public virtual void BreakTile(Inventory inv, Vector2i position) {
            Tile t = Terrain.BreakTile(position.x, position.y);
            switch (t.enumId) {
                case TileID.Invalid:
                case TileID.Air:
                case TileID.Bedrock:
                    break;

                case TileID.Grass: inv.AddItem(RawItem.Grass); break;
                case TileID.Sand: inv.AddItem(RawItem.Sand); break;
                case TileID.Dirt: inv.AddItem(RawItem.Dirt); break;
                case TileID.Wood: inv.AddItem(RawItem.Wood); break;
                case TileID.Leaf: inv.AddItem(RawItem.Leaf); break;
                case TileID.Stone: inv.AddItem(RawItem.Stone); break;
                case TileID.Tnt: inv.AddItem(RawItem.Tnt); break;
                case TileID.Sandstone: inv.AddItem(RawItem.Sandstone); break;
                case TileID.Sapling: inv.AddItem(RawItem.Sapling); break;
                case TileID.Brick: inv.AddItem(RawItem.Brick); break;
                case TileID.Metal1: inv.AddItem(RawItem.Metal1); break;
                case TileID.SmoothSlab: inv.AddItem(RawItem.SmoothSlab); break;
                case TileID.WeatheredStone: inv.AddItem(RawItem.WeatheredStone); break;
                case TileID.FutureMetal: inv.AddItem(RawItem.FutureMetal); break;
                case TileID.Marble: inv.AddItem(RawItem.Marble); break;
                case TileID.PlexSpecial: inv.AddItem(RawItem.PlexSpecial); break;
                case TileID.PurpleStone: inv.AddItem(RawItem.PurpleStone); break;
                case TileID.Nuke: inv.AddItem(RawItem.Nuke); break;
                case TileID.Cactus: inv.AddItem(RawItem.Cactus); break;
                case TileID.Bounce: inv.AddItem(RawItem.Bounce); break;
                case TileID.Water: break;
                case TileID.WireOn: case TileID.WireOff: inv.AddItem(RawItem.Wire); break;
                case TileID.SwitchOn: case TileID.SwitchOff: inv.AddItem(RawItem.Switch); break;
                case TileID.LogicLampUnlit: case TileID.LogicLampLit: inv.AddItem(RawItem.LogicLamp); break;
                case TileID.Snow: inv.AddItem(RawItem.Snow); break;
                case TileID.SnowWood: inv.AddItem(RawItem.SnowWood); break;
                case TileID.SnowLeaf: inv.AddItem(RawItem.SnowLeaf); break;
                case TileID.GrassDeco: inv.AddItem(RawItem.GrassDeco); break;
                case TileID.GateAnd: inv.AddItem(RawItem.GateAnd); break;
                case TileID.GateOr: inv.AddItem(RawItem.GateOr); break;
                case TileID.GateNot: inv.AddItem(RawItem.GateNot); break;
                case TileID.WireBridgeOff: case TileID.WireBridgeHorzVertOn: case TileID.WireBridgeHorzOn: case TileID.WireBridgeVertOn: inv.AddItem(RawItem.WireBridge); break;
                case TileID.TilePusherOff: case TileID.TilePusherOn: inv.AddItem(RawItem.StickyTilePusher); break;
                case TileID.TilePullerOn: case TileID.TilePullerOff: inv.AddItem(RawItem.StickyTilePuller); break;
                case TileID.Light: inv.AddItem(RawItem.Light); break;
                case TileID.Accelerator: inv.AddItem(RawItem.Accelerator); break;
                case TileID.SingleTilePusherOff: case TileID.SingleTilePusherOn: inv.AddItem(RawItem.SingleTilePusher); break;
            }

        }
    }

    [Serializable]
    class Item_No_Attribs : ItemAttribs {

        public Item_No_Attribs(string name, int stackSize) : base(stackSize, name) { }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) { }
    }

    [Serializable]
    class Item_Tile_Attribs : ItemAttribs {

        protected Tile tile;

        public Item_Tile_Attribs(string name, Tile tile) : base(999, name) {
            this.tile = tile;
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && EntityManager.GetEntitiesAt(position).Length == 0 && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile);
        }
    }

    [Serializable]
    class Item_DataTile_Attribs : ItemAttribs {

        protected Func<Tile> tile;

        public Item_DataTile_Attribs(string name, Func<Tile> tile) : base(999, name) {
            this.tile = tile;
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && EntityManager.GetEntitiesAt(position).Length == 0 && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile());
        }
    }

    [Serializable]
    class Item_DirectionalTile_Attribs : Item_DataTile_Attribs {


        public Item_DirectionalTile_Attribs(string name, Func<Tile> tile) : base(name, tile) {
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && EntityManager.GetEntitiesAt(position).Length == 0 && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile(), direction);
        }
    }

    [Serializable]
    class ItemFluidAttribs : ItemAttribs {

        private Tile tile;

        public ItemFluidAttribs(string name, Tile tile) : base(999, name) {
            this.tile = tile;
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && !(Terrain.TileAt(x, y).tileattribs is FluidAttribs) && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile);
        }

    }

    [Serializable]
    abstract class ItemStaffParticleAttribs : ItemAttribs {

        public ItemStaffParticleAttribs(string name) : base(1, name) { }

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
    class Item_RedStaff_Attribs : ItemStaffParticleAttribs {
        public Item_RedStaff_Attribs() : base("Staff of Destruction") {
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            SParc_Destroy.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class Item_PurpleStaff_Attribs : ItemStaffParticleAttribs {
        public Item_PurpleStaff_Attribs() : base("Staff of Damage") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            SParc_Damage.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class Item_GreenStaff_Attribs : ItemStaffParticleAttribs {
        public Item_GreenStaff_Attribs() : base("Staff of Speed") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            SParc_Speed.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class Item_BlueStaff_Attribs : ItemStaffParticleAttribs {
        public Item_BlueStaff_Attribs() : base("Staff of Water") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            SParc_Water.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class Item_YellowStaff_Attribs : ItemStaffParticleAttribs {
        public Item_YellowStaff_Attribs() : base("Staff of Creation") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position, Vector2 direction) {
            SParc_Place.Create(CalculatePos(), CalculateVel());
        }
    }
}
