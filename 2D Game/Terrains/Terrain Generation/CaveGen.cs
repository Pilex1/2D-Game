using Game.Util;
using System;
using System.Diagnostics;

namespace Game.Terrains.Terrain_Generation {
    static class CaveGen {

        private const float caveFrequency = 0.001f;
        private const int initCaveRadius = 5;
        private const float initCaveFillFactor = 0.3f;

        private const int neighbourRadius = 1;
        private const int neighbourcutoff = 5;
        private const int smoothCount = 4;

        private static Random rand;

        internal static void GenCaves() {

            Debug.Assert(TerrainGen.rand != null);
            rand = TerrainGen.rand;

            Tile[,] originalcopy = Terrain.ShallowCopyTileData();

            //randomly gen holes
            RandFill();

            for (int i = 0; i < smoothCount; i++) {
                Smooth(originalcopy);
            }

        }

        private static void RandFill() {
            Vector2i min = new Vector2i(0, 0);
            Vector2i max = new Vector2i(Terrain.Tiles.GetLength(0) - 1, Terrain.Tiles.GetLength(1) - 1);
            for (int i = 0; i < caveFrequency * Terrain.Tiles.Length; i++) {
                Vector2i caveloc = MathUtil.RandVector2i(rand, min, max);
                for (int x = -initCaveRadius; x <= initCaveRadius; x++) {
                    for (int y = -initCaveRadius; y <= initCaveRadius; y++) {
                        if (MathUtil.RandFloat(rand, 0, 1) < initCaveFillFactor) {
                            Terrain.SetTile(caveloc.x + x, caveloc.y + y, Tile.Air, true);
                        }
                    }
                }
            }
        }

        private static void Smooth(Tile[,] originalcopy) {

            Tile[,] terraincopy = Terrain.ShallowCopyTileData();

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    int neighbourcount = Neighbours(i, j, neighbourRadius);
                    if (neighbourcount < neighbourcutoff) {
                        terraincopy[i, j] = Tile.Air;
                    } else {
                        terraincopy[i, j] = originalcopy[i, j];
                    }
                }
            }

            Terrain.Tiles = terraincopy;
        }


        private static int Neighbours(int x, int y, int radius) {
            int count = 0;
            for (int i = -radius; i <= radius; i++) {
                for (int j = -radius; j <= radius; j++) {
                    if (i == 0 && j == 0) continue;
                    TileID enumid = Terrain.TileAt(x + i, y + j).enumId;
                    if (enumid != TileID.Air && enumid != TileID.Invalid) count++;
                }
            }
            return count;
        }
    }
}
