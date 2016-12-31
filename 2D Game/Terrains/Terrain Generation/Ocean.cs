using Game.Util;
using System;

namespace Game.Terrains.Terrain_Generation {
    static class Ocean {
        internal static int Generate(int posX, int posY, int size) {
            float heightVar = 2;
            int lastHeight = 0;

            var heightsGen = new Func<float, float>(delegate (float f) {
                float x = f * (f - 1);
                x += 1 / 4;
                x *= 4;
                x = Math.Max(x, 0.25f);
                return (1 - x) * 64 + MathUtil.RandFloat(TerrainGen.rand, -heightVar, heightVar);
            });

            float v1 = posY, v2 = v1, v3 = TerrainGen.minlandheight, v4 = heightsGen(0);

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < TerrainGen.widthfactor; j++) {
                    int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / TerrainGen.widthfactor);
                    lastHeight = y;
                    int x = posX + i * TerrainGen.widthfactor + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 10 + MathUtil.RandDouble(TerrainGen.rand, 0, 3)) Terrain.SetTile(x, k, Tile.Stone, true);
                        else if (k <= y - 3 + MathUtil.RandDouble(TerrainGen.rand, 0, 2)) Terrain.SetTile(x, k, Tile.Dirt, true);
                        else Terrain.SetTile(x, k, Tile.Sand, true);
                    }
                }
                v1 = v2;
                v2 = v3;
                v3 = v4;
                v4 = heightsGen((float)i / size);
            }

            for (int x = posX; x < posX + size * TerrainGen.widthfactor; x++) {
                for (int y = Terrain.HighestPoint(x) + 1; y < 64; y++) {
                    Terrain.SetTile(x, y, Tile.Water(), false);
                }
            }

            return lastHeight;
        }
    }
}
