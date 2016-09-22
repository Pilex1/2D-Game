using System;
using Game.Terrains;
using Game.Fluids;

namespace Game.Assets {
    enum ItemId {
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, Crate, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce, Water
    }

    static class ItemInteract {
        public static object FluidManager { get; private set; }

        public static void Interact(ItemId item, int x, int y) {
            if (item == ItemId.PurpleStone) new PurpleStone(x, y);
            else if (item == ItemId.Grass) new Grass(x, y);
            else if (item == ItemId.Sand) new Sand(x, y);
            else if (item == ItemId.Dirt) new Dirt(x, y);
            else if (item == ItemId.Wood) new Wood(x, y);
            else if (item == ItemId.Leaf) new Leaf(x, y);
            else if (item == ItemId.Stone) new Stone(x, y);
            else if (item == ItemId.Tnt) new Tnt(x, y);
            else if (item == ItemId.Sapling) new Sapling(x, y);
            else if (item == ItemId.Crate) new Crate(x, y);
            else if (item == ItemId.Sapling) new Sapling(x, y);
            else if (item == ItemId.Brick) new Brick(x, y);
            else if (item == ItemId.Metal1) new Metal1(x, y);
            else if (item == ItemId.SmoothSlab) new SmoothSlab(x, y);
            else if (item == ItemId.WeatheredStone) new WeatheredStone(x, y);
            else if (item == ItemId.Metal2) new Metal2(x, y);
            else if (item == ItemId.FutureMetal) new SmoothSlab2(x, y);
            else if (item == ItemId.SmoothSlab2) new PurpleStone(x, y);
            else if (item == ItemId.Marble) new Marble(x, y);
            else if (item == ItemId.PlexSpecial) new PlexSpecial(x, y);
            else if (item == ItemId.Nuke) new Nuke(x, y);
            else if (item == ItemId.Cactus) new Cactus(x, y);
            else if (item == ItemId.Bounce) new Bounce(x, y);
            else if (item == ItemId.Water) FluidsManager.AddFluid(new Water(x, y, 1));
        }
    }
}
