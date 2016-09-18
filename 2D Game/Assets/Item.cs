using System;
using Game.Terrains;

namespace Game.Assets {
    enum Item {
        None = -1, PurpleStone, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword, Tnt, Sapling
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
        }
    }
}
