using System;
using Game.Terrains;
using Game.Fluids;
using Game.Logics;
using Game.Particles;
using OpenGL;
using Game.Util;
using Game.Core;

namespace Game.Assets {

    [Serializable]
    enum Item {
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, TileBreaker, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, Wire, Switch, LogicLamp, LogicBridge, GateOr, GateNot, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, StickyTilePusher, StickyTilePuller, Igniter, StaffGreen, StaffBlue, StaffRed, StaffPurple
    }

    static class ItemInteract {

        public static void Interact(Item item, int x, int y) {
            switch (item) {
                case Item.PurpleStone: Terrain.SetTile(x, y, TileID.PurpleStone); break;
                case Item.Grass: Terrain.SetTile(x, y, TileID.Grass); break;
                case Item.Sand: Terrain.SetTile(x, y, TileID.Sand); break;
                case Item.Dirt: Terrain.SetTile(x, y, TileID.Dirt); break;
                case Item.Wood: Terrain.SetTile(x, y, TileID.Wood); break;
                case Item.Leaf: Terrain.SetTile(x, y, TileID.Leaf); break;
                case Item.Stone: Terrain.SetTile(x, y, TileID.Stone); break;
                case Item.Tnt: Terrain.SetTile(x, y, TileID.Tnt); break;
                case Item.Sapling: Terrain.SetTile(x, y, TileID.Sapling); break;
                case Item.Brick: Terrain.SetTile(x, y, TileID.Brick); break;
                case Item.Metal1: Terrain.SetTile(x, y, TileID.Metal1); break;
                case Item.SmoothSlab: Terrain.SetTile(x, y, TileID.SmoothSlab); break;
                case Item.WeatheredStone: Terrain.SetTile(x, y, TileID.WeatheredStone); break;
                case Item.Metal2: Terrain.SetTile(x, y, TileID.Metal2); break;
                case Item.FutureMetal: Terrain.SetTile(x, y, TileID.FutureMetal); break;
                case Item.SmoothSlab2: Terrain.SetTile(x, y, TileID.SmoothSlab2); break;
                case Item.Marble: Terrain.SetTile(x, y, TileID.Marble); break;
                case Item.PlexSpecial: Terrain.SetTile(x, y, TileID.PlexSpecial); break;
                case Item.Nuke: Terrain.SetTile(x, y, TileID.Nuke); break;
                case Item.Cactus: Terrain.SetTile(x, y, TileID.Cactus); break;
                case Item.Bounce: Terrain.SetTile(x, y, TileID.Bounce); break;
                case Item.Water: Terrain.SetTile(x, y, TileID.Water); break;
                case Item.Wire: Terrain.SetTile(x, y, TileID.CreateWire()); break;
                case Item.Switch: Terrain.SetTile(x, y, TileID.CreateSwitch()); break;
                case Item.LogicLamp: Terrain.SetTile(x, y, TileID.CreateLogicLamp()); break;
                case Item.GateAnd: Terrain.SetTile(x, y, TileID.CreateGateAnd()); break;
                case Item.GateOr: Terrain.SetTile(x, y, TileID.CreateGateOr()); break;
                case Item.GateNot: Terrain.SetTile(x, y, TileID.CreateGateNot()); break;
                case Item.LogicBridge: Terrain.SetTile(x, y, TileID.CreateLogicBridge()); break;
                case Item.StickyTilePusher: Terrain.SetTile(x, y, TileID.CreateTilePusher()); break;
                case Item.StickyTilePuller: Terrain.SetTile(x, y, TileID.CreateTilePuller()); break;
                case Item.TileBreaker: Terrain.SetTile(x, y, TileID.CreateTileBreaker()); break;
                case Item.StaffGreen: {
                        Vector2 pos = Player.Instance.data.Position.val;
                        pos += new Vector2(Player.Instance.Hitbox.Width / 2, Player.Instance.Hitbox.Height / 2);
                        pos += MathUtil.RandVector2(Program.Rand, new Vector2(-1, -1), new Vector2(1, 1));
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        StaffParticleGreen.Create(pos, vel);
                    }
                    break;
                case Item.StaffBlue: {
                        Vector2 pos = Player.Instance.data.Position.val;
                        pos += new Vector2(Player.Instance.Hitbox.Width / 2, Player.Instance.Hitbox.Height / 2);
                        pos += MathUtil.RandVector2(Program.Rand, new Vector2(-1, -1), new Vector2(1, 1));
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        StaffParticleBlue.Create(pos, vel);
                    }
                    break;
                case Item.StaffRed: {
                        Vector2 pos = Player.Instance.data.Position.val;
                        pos += new Vector2(Player.Instance.Hitbox.Width / 2, Player.Instance.Hitbox.Height / 2);
                        pos += MathUtil.RandVector2(Program.Rand, new Vector2(-1, -1), new Vector2(1, 1));
                        Vector2 vel = Input.RayCast().Normalize();
                        Vector2 playervel = new Vector2(Player.Instance.data.vel.x, Player.Instance.data.vel.y);
                        vel += playervel;
                        StaffParticleRed.Create(pos, vel);
                    }
                    break;
                case Item.StaffPurple: {
                        Vector2 pos = Player.Instance.data.Position.val;
                        pos += new Vector2(Player.Instance.Hitbox.Width / 2, Player.Instance.Hitbox.Height / 2);
                        StaffParticlePurple.Create(pos);
                    }
                    break;
            }
        }
    }
}
