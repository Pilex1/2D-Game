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

        public NoAttribs(string name, int stackSize) : base(stackSize,  name) { }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) { }
    }

    [Serializable]
    class ItemTileAttribs : ItemAttribs {

        private Tile tile;

        public ItemTileAttribs(string name, Tile tile) : base(999, name) {
            this.tile = tile;
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            int x = (int)position.x;
            int y = (int)position.y;
            if (!Terrain.TileAt(x, y).tileattribs.solid && EntityManager.GetEntitiesAt(position).Length == 0 && inv.RemoveItem(invslot.x, invslot.y))
                Terrain.SetTile(x, y, tile);
        }
    }

    [Serializable]
    class ItemFluidAttribs : ItemAttribs {

        private Tile tile;

        public ItemFluidAttribs(string name, Tile tile) : base(999, name) {
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
    class ItemRedStaffParticleAttribs : ItemStaffParticleAttribs {
        public ItemRedStaffParticleAttribs() : base("Staff of Destruction") {
        }

        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Destroy.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemPurpleStaffParticleAttribs : ItemStaffParticleAttribs {
        public ItemPurpleStaffParticleAttribs() : base("Staff of Damage") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Damage.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemGreenStaffParticleAttribs : ItemStaffParticleAttribs {
        public ItemGreenStaffParticleAttribs() : base("Staff of Speed") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Speed.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemBlueStaffParticleAttribs : ItemStaffParticleAttribs {
        public ItemBlueStaffParticleAttribs() : base("Staff of Water") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Water.Create(CalculatePos(), CalculateVel());
        }
    }

    [Serializable]
    class ItemYellowStaffParticleAttribs : ItemStaffParticleAttribs {
        public ItemYellowStaffParticleAttribs() : base("Staff of Creation") {
        }
        public override void Use(Inventory inv, Vector2i invslot, Vector2 position) {
            SParc_Place.Create(CalculatePos(), CalculateVel());
        }
    }
}
