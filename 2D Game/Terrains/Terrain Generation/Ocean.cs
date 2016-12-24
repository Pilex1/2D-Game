using Game.Util;
using System;

namespace Game.Terrains.Gen {
    static class Ocean {
        internal static int Generate(int posX, int posY, int size) {
            int lastHeight = 0;

            Func<float, float> heightsGen = new Func<float, float>(delegate (float f) {
                float height = (float)Math.Pow(2 * f - 1, 8);
                height += MathUtil.RandFloat(TerrainGen.rand, -5, 5);
                return height;
            });

            float v1 = posY, v2 = v1, v3 = TerrainGen.minlandheight, v4 = heightsGen(0);

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < TerrainGen.widthfactor; j++) {
                    int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / TerrainGen.widthfactor);
                    lastHeight = y;
                    int x = posX + i * TerrainGen.widthfactor + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 10 + MathUtil.RandDouble(TerrainGen.rand, 0, 3)) Terrain.SetTileTerrainGen(x, k, Tile.Stone, true);
                        else if (k <= y - 3 + MathUtil.RandDouble(TerrainGen.rand, 0, 2)) Terrain.SetTileTerrainGen(x, k, Tile.Dirt, true);
                        else Terrain.SetTileTerrainGen(x, k, Tile.Sand, true);
                    }
                }
                v1 = v2;
                v2 = v3;
                v3 = v4;
                v4 = heightsGen((float)i / size);
            }

            for (int x = posX; x < posX + size * TerrainGen.widthfactor; x++) {
                for (int y = Terrain.HighestPoint(x) + 1; y < TerrainGen.minlandheight; y++) {
                    Terrain.SetTileTerrainGen(x, y, Tile.Water(), false);
                }
            }

            return lastHeight;
        }
    }
}
