using Game.Util;
using System;

namespace Game.Terrains.Terrain_Generation {
    static class Mountain {
        internal static int Generate(int posX, int posY, int size) {
            float maxHeightVar = 20, minHeightVar = -3;
            int lastHeight = 0;

            Func<float> heightsGen = new Func<float>(delegate () {
                return MathUtil.RandFloat(TerrainGen.rand, minHeightVar, maxHeightVar);
            });

            float v1 = posY, v2 = v1, v3 = v2 + heightsGen(), v4 = v3 + heightsGen();

            int mountainCounter = 0;
            int mountainDist = MathUtil.RandInt(TerrainGen.rand, 30, 150);

            int treeCounter = 0;
            int treeDist = MathUtil.RandInt(TerrainGen.rand, 10, 30);

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < TerrainGen.widthfactor; j++) {
                    int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / TerrainGen.widthfactor);
                    lastHeight = y;
                    int x = posX + i * TerrainGen.widthfactor + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 3 + MathUtil.RandDouble(TerrainGen.rand, 0, 2)) Terrain.SetTile(x, k, Tile.Stone, true);
                        else if (k <= y - 2 + MathUtil.RandDouble(TerrainGen.rand, 0, 2)) Terrain.SetTile(x, k, Tile.Dirt, true);
                        else Terrain.SetTile(x, k, Tile.Grass, true);
                    }

                    mountainCounter++;
                    if (mountainCounter == mountainDist / 2 || y < TerrainGen.minlandheight) {
                        maxHeightVar *= -1;
                        minHeightVar *= -1;
                    }
                    if (mountainCounter == mountainDist) {
                        mountainCounter = 0;
                        mountainDist = MathUtil.RandInt(TerrainGen.rand, 30, 50);
                    }

                    treeCounter++;
                    if (treeCounter == treeDist) {
                        Nature.GenerateTree(x, y + 1);
                        treeCounter = 0;
                        treeDist = MathUtil.RandInt(TerrainGen.rand, 10, 30);
                    }
                }
                v1 = v2;
                v2 = v3;
                v3 = v4;
                v4 = v3 + heightsGen();
            }

            return lastHeight;
        }

    }
}
