using Game.Util;
using System;

namespace Game.Terrains.Terrain_Generation {
    class Desert {
        internal static int Generate(int posX, int posY, int size) {
            float heightVar = 10;
            int lastHeight = 0;

            Func<float> heightsGen = new Func<float>(delegate () {
                return MathUtil.RandFloat(TerrainGen.rand, TerrainGen.minlandheight, TerrainGen.minlandheight + heightVar);
            });

            float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

            int cactusCounter = 0;
            int cactusDist = MathUtil.RandInt(TerrainGen.rand, 30, 60);

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < TerrainGen.widthfactor; j++) {
                    int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / TerrainGen.widthfactor);
                    lastHeight = y;
                    int x = posX + i * TerrainGen.widthfactor + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 25 + MathUtil.RandDouble(TerrainGen.rand, 0, 3)) Terrain.SetTile(x, k, Tile.Stone, true);
                        else if (k <= y - 10 + MathUtil.RandDouble(TerrainGen.rand, 0, 4)) Terrain.SetTile(x, k, Tile.Sandstone, true);
                        else Terrain.SetTile(x, k, Tile.Sand, true);
                    }

                    cactusCounter++;
                    if (cactusCounter == cactusDist) {
                        Nature.GenerateCactus(x, y + 1);
                        cactusCounter = 0;
                        cactusDist = MathUtil.RandInt(TerrainGen.rand, 15, 20);
                    }
                }
                v1 = v2;
                v2 = v3;
                v3 = v4;
                v4 = heightsGen();
            }

            return lastHeight;
        }



    }
}
