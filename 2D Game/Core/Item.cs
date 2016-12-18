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
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, TileBreaker, Brick, Metal1, SmoothSlab, WeatheredStone, FutureMetal, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, Wire, Switch, LogicLamp, WireBridge, GateOr, GateNot, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, StickyTilePusher, StickyTilePuller, Igniter, StaffGreen, StaffBlue, StaffRed, StaffPurple, Debugger, Light, Accelerator, Lava, SingleTilePusher, Sandstone
    }

    [Serializable]
    struct Item {
        public ItemID id;
        public uint amt;

        public Item(ItemID id, uint amt) {
            this.id = id;
            this.amt = amt;
        }

        public override string ToString() {
            return id + ": " + amt;
        }
    }

    static class ItemInteract {

        public static void Interact(ItemID item, int x, int y) {
            Vector2 v = Input.NDCMouse;
            var p = PlayerInventory.Instance;
            switch (item) {
                #region Deco
                case ItemID.PurpleStone: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.PurpleStone)) Terrain.SetTile(x, y, Tile.PurpleStone); break;
                case ItemID.Grass: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Grass)) Terrain.SetTile(x, y, Tile.Grass); break;
                case ItemID.Sand: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Sand)) Terrain.SetTile(x, y, Tile.Sand); break;
                case ItemID.Dirt: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Dirt)) Terrain.SetTile(x, y, Tile.Dirt); break;
                case ItemID.Wood: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Wood)) Terrain.SetTile(x, y, Tile.Wood); break;
                case ItemID.Leaf: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Leaf)) Terrain.SetTile(x, y, Tile.Leaf); break;
                case ItemID.Stone: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Stone)) Terrain.SetTile(x, y, Tile.Stone); break;
                case ItemID.Tnt: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Tnt)) Terrain.SetTile(x, y, Tile.Tnt); break;
                case ItemID.Sapling: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Sapling)) Terrain.SetTile(x, y, Tile.Sapling); break;
                case ItemID.Brick: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Brick)) Terrain.SetTile(x, y, Tile.Brick); break;
                case ItemID.Metal1: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Metal1)) Terrain.SetTile(x, y, Tile.Metal1); break;
                case ItemID.SmoothSlab: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.SmoothSlab)) Terrain.SetTile(x, y, Tile.SmoothSlab); break;
                case ItemID.WeatheredStone: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.WeatheredStone)) Terrain.SetTile(x, y, Tile.WeatheredStone); break;
                case ItemID.FutureMetal: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.FutureMetal)) Terrain.SetTile(x, y, Tile.FutureMetal); break;
                case ItemID.Marble: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Marble)) Terrain.SetTile(x, y, Tile.Marble); break;
                case ItemID.PlexSpecial: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.PlexSpecial)) Terrain.SetTile(x, y, Tile.PlexSpecial); break;
                case ItemID.Nuke: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Nuke)) Terrain.SetTile(x, y, Tile.Nuke); break;
                case ItemID.Cactus: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Cactus)) Terrain.SetTile(x, y, Tile.Cactus); break;
                case ItemID.Bounce: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Bounce)) Terrain.SetTile(x, y, Tile.Bounce); break;
                case ItemID.Snow: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Snow)) Terrain.SetTile(x, y, Tile.Snow); break;
                case ItemID.SnowWood: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.SnowWood)) Terrain.SetTile(x, y, Tile.SnowWood); break;
                case ItemID.SnowLeaf: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.SnowLeaf)) Terrain.SetTile(x, y, Tile.SnowLeaf); break;
                case ItemID.GrassDeco: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.GrassDeco)) Terrain.SetTile(x, y, Tile.GrassDeco); break;
                #endregion

                #region Logic
                case ItemID.Wire: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Wire)) Terrain.SetTile(x, y, Tile.Wire, v); break;
                case ItemID.Switch: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.Switch)) Terrain.SetTile(x, y, Tile.Switch, v); break;
                case ItemID.LogicLamp: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.LogicLamp)) Terrain.SetTile(x, y, Tile.LogicLamp, v); break;
                case ItemID.GateAnd: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.GateAnd)) Terrain.SetTile(x, y, Tile.GateAnd, v); break;
                case ItemID.GateOr: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.GateOr)) Terrain.SetTile(x, y, Tile.GateOr, v); break;
                case ItemID.GateNot: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.GateNot)) Terrain.SetTile(x, y, Tile.GateNot, v); break;
                case ItemID.WireBridge: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.WireBridge)) Terrain.SetTile(x, y, Tile.WireBridge, v); break;
                case ItemID.StickyTilePusher: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.StickyTilePusher)) Terrain.SetTile(x, y, Tile.TilePusher, v); break;
                case ItemID.StickyTilePuller: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.StickyTilePuller)) Terrain.SetTile(x, y, Tile.TilePuller, v); break;
                case ItemID.SingleTilePusher: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.SingleTilePusher)) Terrain.SetTile(x, y, Tile.SingleTilePusher, v); break;
                case ItemID.TileBreaker: if (!Terrain.TileAt(x, y).tileattribs.solid && p.RemoveItem(ItemID.TileBreaker)) Terrain.SetTile(x, y, Tile.TileBreaker, v); break;
                #endregion

                case ItemID.Water: if (!Terrain.TileAt(x, y).tileattribs.solid && !(Terrain.TileAt(x, y).tileattribs is FluidAttribs) && p.RemoveItem(ItemID.Water)) Terrain.SetTile(x, y, Tile.Water); break;
                //case Item.Lava: Terrain.SetTile(x, y, Tile.Lava); break;

                case ItemID.Accelerator: Terrain.SetTile(x, y, Tile.Accelerator); break;

                #region Weapons
                case ItemID.StaffPurple: {
                        Vector2 pos = Player.Instance.data.pos.val;
                        pos += new Vector2(Player.Instance.hitbox.Size.x / 2, Player.Instance.hitbox.Size.y / 2);
                        pos += MathUtil.RandVector2(Program.Rand, new Vector2(-1, -1), new Vector2(1, 1));
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        SParc_Damage.Create(pos, vel);
                    }
                    break;
                case ItemID.StaffRed: {
                        Vector2 pos = Player.Instance.data.pos.val;
                        pos += new Vector2(Player.Instance.hitbox.Size.x / 2, Player.Instance.hitbox.Size.y / 2);
                        pos += MathUtil.RandVector2(Program.Rand, -1, 1);
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        SParc_Destroy.Create(pos, vel);
                    }
                    break;
                case ItemID.StaffGreen: {
                        Vector2 pos = Player.Instance.data.pos.val;
                        pos += new Vector2(Player.Instance.hitbox.Size.x / 2, Player.Instance.hitbox.Size.y / 2);
                        pos += MathUtil.RandVector2(Program.Rand, -1, 1);
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        SParc_Speed.Create(pos, vel);
                    }
                    break;
                case ItemID.StaffBlue: {
                        Vector2 pos = Player.Instance.data.pos.val;
                        pos += new Vector2(Player.Instance.hitbox.Size.x / 2, Player.Instance.hitbox.Size.y / 2);
                        pos += MathUtil.RandVector2(Program.Rand, -1, 1);
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        SParc_Water.Create(pos, vel);
                    }
                    break;
                    #endregion
            }
        }
    }
}
