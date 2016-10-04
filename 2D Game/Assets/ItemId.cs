using System;
using Game.Terrains;
using Game.Fluids;
using Game.Logics;

namespace Game.Assets {
    enum ItemId {
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, TileBreaker, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, Wire, Switch, LogicLamp, LogicBridge, GateOr, GateNot, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, StickyTilePusher, StickyTilePuller
    }

    static class ItemInteract {

        public static void Interact(ItemId item, int x, int y) {
            switch (item) {
                case ItemId.PurpleStone: Terrain.SetTile(x, y, TileID.PurpleStone); break;
                case ItemId.Grass: Terrain.SetTile(x, y, TileID.Grass); break;
                case ItemId.Sand: Terrain.SetTile(x, y, TileID.Sand); break;
                case ItemId.Dirt: Terrain.SetTile(x, y, TileID.Dirt); break;
                case ItemId.Wood: Terrain.SetTile(x, y, TileID.Wood); break;
                case ItemId.Leaf: Terrain.SetTile(x, y, TileID.Leaf); break;
                case ItemId.Stone: Terrain.SetTile(x, y, TileID.Stone); break;
                case ItemId.Tnt: Terrain.SetTile(x, y, TileID.Tnt); break;
                case ItemId.Sapling: Terrain.SetTile(x, y, TileID.Sapling); break;
                case ItemId.Brick: Terrain.SetTile(x, y, TileID.Brick); break;
                case ItemId.Metal1: Terrain.SetTile(x, y, TileID.Metal1); break;
                case ItemId.SmoothSlab: Terrain.SetTile(x, y, TileID.SmoothSlab); break;
                case ItemId.WeatheredStone: Terrain.SetTile(x, y, TileID.WeatheredStone); break;
                case ItemId.Metal2: Terrain.SetTile(x, y, TileID.Metal2); break;
                case ItemId.FutureMetal: Terrain.SetTile(x, y, TileID.FutureMetal); break;
                case ItemId.SmoothSlab2: Terrain.SetTile(x, y, TileID.SmoothSlab2); break;
                case ItemId.Marble: Terrain.SetTile(x, y, TileID.Marble); break;
                case ItemId.PlexSpecial: Terrain.SetTile(x, y, TileID.PlexSpecial); break;
                case ItemId.Nuke: Terrain.SetTile(x, y, TileID.Nuke); break;
                case ItemId.Cactus: Terrain.SetTile(x, y, TileID.Cactus); break;
                case ItemId.Bounce: Terrain.SetTile(x, y, TileID.Bounce); break;
                case ItemId.Water: Terrain.SetTile(x, y, TileID.Water); break;
                case ItemId.Wire: Terrain.SetTile(x, y, TileID.CreateWire()); break;
                case ItemId.Switch: Terrain.SetTile(x, y, TileID.CreateSwitch()); break;
                case ItemId.LogicLamp: Terrain.SetTile(x, y, TileID.CreateLogicLamp()); break;
                case ItemId.GateAnd: Terrain.SetTile(x, y, TileID.CreateGateAnd()); break;
                case ItemId.GateOr: Terrain.SetTile(x, y, TileID.CreateGateOr()); break;
                case ItemId.GateNot: Terrain.SetTile(x, y, TileID.CreateGateNot()); break;
                case ItemId.LogicBridge: Terrain.SetTile(x, y, TileID.CreateLogicBridge()); break;
                case ItemId.StickyTilePusher: Terrain.SetTile(x, y, TileID.CreateTilePusher()); break;
                case ItemId.StickyTilePuller: Terrain.SetTile(x, y, TileID.CreateTilePuller()); break;
                case ItemId.TileBreaker: Terrain.SetTile(x, y, TileID.CreateTileBreaker()); break;
            }
        }
    }
}
