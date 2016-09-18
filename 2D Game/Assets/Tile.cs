using System;
using Game.Util;
using Game.Terrains;

namespace Game.Assets {
    enum Tile {
        Air = -1, PurpleStone, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt, Sandstone, Sapling
    }

    static class TileInteract {

        public const int TntRadius = 10, TntError = 5;

        public static void Interact(Tile tile, int x, int y) {
            if (tile == Tile.Tnt) {
                for (int i = -TntRadius + MathUtil.RandInt(-TntError, TntError); i <= TntRadius + MathUtil.RandInt(-TntError, TntError); i++) {
                    double jStart = -Math.Sqrt(TntRadius * TntRadius - i * i) + MathUtil.RandInt(-TntError, TntError);
                    double jEnd = Math.Sqrt(TntRadius * TntRadius - i * i) + MathUtil.RandInt(-TntError, TntError);
                    if (jStart == double.NaN || jEnd == double.NaN) continue;
                    for (int j = (int)jStart; j <= jEnd; j++) {
                        if (Terrain.BreakTile(x + i, j + y) == Tile.Tnt) Interact(Tile.Tnt, x + i, j + y);
                    }
                }
            }
        }

        public static bool IsSolid(Tile t) {
            return !(t == Tile.Sapling || t == Tile.Air);
        }

        public static bool IsOnlyPlaceableFromBottom(Tile t) {
            return (t == Tile.Sapling);
        }
    }
}
