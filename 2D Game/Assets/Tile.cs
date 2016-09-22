using System;
using Game.Util;
using Game.Terrains;

namespace Game.Assets {
    enum Tile {
        Air, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Tnt, Sandstone, Sapling, Crate, Brick, Metal1, SmoothSlab, WeatheredStone, Metal2, FutureMetal, SmoothSlab2, Marble, PlexSpecial, PurpleStone, Nuke, Cactus, Bounce
    }

    static class TileInteract {

        public const int TntRadius = 10;
        public const int NukeRadius = 50;
        public const int ExplosionError = 2;

        private static Random Rand = new Random();

        public static void Interact(Tile tile, int x, int y) {
            if (tile == Tile.Tnt) Explode(x, y, TntRadius);
            else if (tile == Tile.Nuke) Explode(x, y, NukeRadius);
        }

        private static void Explode(int x, int y, int radius) {
            for (int i = -radius + MathUtil.RandInt(Rand, -ExplosionError, ExplosionError); i <= radius + MathUtil.RandInt(Rand, -ExplosionError, ExplosionError); i++) {
                double jStart = -Math.Sqrt(radius * radius - i * i) + MathUtil.RandInt(Rand, -ExplosionError, ExplosionError);
                double jEnd = Math.Sqrt(radius * radius - i * i) + MathUtil.RandInt(Rand, -ExplosionError, ExplosionError);
                if (jStart == double.NaN || jEnd == double.NaN) continue;
                for (int j = (int)jStart; j <= jEnd; j++) {
                    if (Terrain.BreakTile(x + i, j + y) == Tile.Tnt) Explode(x + i, j + y, radius);
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
