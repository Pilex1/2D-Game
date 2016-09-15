using System;
using Game.Util;
using Game.Terrains;

namespace Game.Assets {
    enum Tile {
        Air = -1, PurpleStone, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt
    }

    static class TileInteract {
        public static void Interact(Tile tile, int x, int y) {
            if (tile == Tile.Tnt) {
                int radius = 65;
                for (int i = -radius + MathUtil.RandInt(-1, 1); i <= radius + MathUtil.RandInt(-1, 1); i++) {
                    double jStart = -Math.Sqrt(radius * radius - i * i) + MathUtil.RandInt(-1, 1);
                    double jEnd = Math.Sqrt(radius * radius - i * i) + MathUtil.RandInt(-1, 1);
                    if (jStart == double.NaN || jEnd == double.NaN) continue;
                    for (int j = (int)jStart; j <= jEnd; j++) {
                        if (Terrain.BreakTile(x + i, j + y) == Tile.Tnt) Interact(Tile.Tnt, x + i, j + y);
                    }
                }
            }
        }
    }
}
