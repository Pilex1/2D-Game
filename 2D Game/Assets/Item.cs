using System;
using Game.Terrains;

namespace Game.Assets {
    enum Item {
        None, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling, Crate, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce,
    }

    static class ItemInteract {
        public static void Interact(Item item, int x, int y) {
            if (item == Item.PurpleStone) Terrain.PlaceTile(x, y, Tile.PurpleStone);
            else if (item == Item.Grass) Terrain.PlaceTile(x, y, Tile.Grass);
            else if (item == Item.Sand) Terrain.PlaceTile(x, y, Tile.Sand);
            else if (item == Item.Dirt) Terrain.PlaceTile(x, y, Tile.Dirt);
            else if (item == Item.Wood) Terrain.PlaceTile(x, y, Tile.Wood);
            else if (item == Item.Leaf) Terrain.PlaceTile(x, y, Tile.Leaf);
            else if (item == Item.Stone) Terrain.PlaceTile(x, y, Tile.Stone);
            else if (item == Item.Tnt) Terrain.PlaceTile(x, y, Tile.Tnt);
            else if (item == Item.Sapling) Terrain.PlaceTile(x, y, Tile.Sapling);
            else if (item == Item.Crate) Terrain.PlaceTile(x, y, Tile.Crate);
            else if (item == Item.Sapling) Terrain.PlaceTile(x, y, Tile.Sapling);
            else if (item == Item.Brick) Terrain.PlaceTile(x, y, Tile.Brick);
            else if (item == Item.Metal1) Terrain.PlaceTile(x, y, Tile.Metal1);
            else if (item == Item.SmoothSlab) Terrain.PlaceTile(x, y, Tile.SmoothSlab);
            else if (item == Item.WeatheredStone) Terrain.PlaceTile(x, y, Tile.WeatheredStone);
            else if (item == Item.Metal2) Terrain.PlaceTile(x, y, Tile.Metal2);
            else if (item == Item.FutureMetal) Terrain.PlaceTile(x, y, Tile.FutureMetal);
            else if (item == Item.SmoothSlab2) Terrain.PlaceTile(x, y, Tile.SmoothSlab2);
            else if (item == Item.Marble) Terrain.PlaceTile(x, y, Tile.Marble);
            else if (item == Item.PlexSpecial) Terrain.PlaceTile(x, y, Tile.PlexSpecial);
            else if (item == Item.Nuke) Terrain.PlaceTile(x, y, Tile.Nuke);
            else if (item == Item.Cactus) Terrain.PlaceTile(x, y, Tile.Cactus);
            else if (item == Item.Bounce) Terrain.PlaceTile(x, y, Tile.Bounce);
        }
    }
}
