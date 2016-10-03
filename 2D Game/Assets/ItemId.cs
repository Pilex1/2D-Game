using System;
using Game.Terrains;
using Game.Fluids;
using Game.Logics;

namespace Game.Assets {
    enum ItemId {
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, Crate, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water, Wire, Switch, LogicLamp, LogicBridge, GateOr, GateNot, Snow, SnowWood, SnowLeaf, GrassDeco, GateAnd, TilePusher
    }

    static class ItemInteract {

        public static void Interact(ItemId item, int x, int y) {
            switch (item) {
                case ItemId.PurpleStone: new PurpleStone(x, y); break;
                case ItemId.Grass: new Grass(x, y); break;
                case ItemId.Sand: new Sand(x, y); break;
                case ItemId.Dirt: new Dirt(x, y); break;
                case ItemId.Wood: new Wood(x, y); break;
                case ItemId.Leaf: new Leaf(x, y); break;
                case ItemId.Stone: new Stone(x, y); break;
                case ItemId.Tnt: new Tnt(x, y); break;
                case ItemId.Sapling: new Sapling(x, y); break;
                case ItemId.Brick: new Brick(x, y); break;
                case ItemId.Metal1: new Metal1(x, y); break;
                case ItemId.SmoothSlab: new SmoothSlab(x, y); break;
                case ItemId.WeatheredStone: new WeatheredStone(x, y); break;
                case ItemId.Metal2: new Metal2(x, y); break;
                case ItemId.FutureMetal: new FutureMetal(x, y); break;
                case ItemId.SmoothSlab2: new SmoothSlab2(x, y); break;
                case ItemId.Marble: new Marble(x, y); break;
                case ItemId.PlexSpecial: new PlexSpecial(x, y); break;
                case ItemId.Nuke: new Nuke(x, y); break;
                case ItemId.Cactus: new Cactus(x, y); break;
                case ItemId.Bounce: new Bounce(x, y); break;
                case ItemId.Water: new Water(x, y); break;
                case ItemId.Wire: Wire.Create(x, y); break;
                case ItemId.Switch: Switch.Create(x, y); break;
                case ItemId.LogicLamp: LogicLamp.Create(x, y); break;
                case ItemId.GateAnd: ANDGate.Create(x, y); break;
                case ItemId.GateOr: ORGate.Create(x, y); break;
                case ItemId.GateNot: NOTGate.Create(x, y); break;
                case ItemId.LogicBridge: LogicBridge.Create(x, y); break;
                case ItemId.TilePusher:TilePusher.Create(x, y);break;
            }
        }
    }
}
