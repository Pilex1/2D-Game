using System;
using Game.Assets;
using Game.Util;

namespace Game.Terrains {

    enum Biomes {
        Plains, Desert, Mountain, Forest
    }

    internal static class TerrainGen {

        public const int Size = 1000;
        public const int WidthFactor = 10;
        public const int Freq = Size / WidthFactor;

        public const int SeaLevel = 64;

        internal static Random Rand;

        internal static void Generate(int seed) {
            Rand = new Random(seed);

            GenerateTerrain();
        }

        private static void SetTile(int x, int y, Tile tile) {
            if (x < 0 || x >= Terrain.Tiles.GetLength(0) || y < 0 || y >= Terrain.Tiles.GetLength(1)) return;
            Terrain.Tiles[x, y] = tile;
        }

        private static void GenerateTerrain() {
            //init
            Terrain.Tiles = new Tile[Size, 256];
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    SetTile(i, j, Tile.Air);
                }
            }

            //gen
            int ptr = 0;
            int h = MathUtil.RandInt(Rand, SeaLevel, SeaLevel + 20);
            int biomeSizeMin = 10, biomeSizeMax = 20;
            while (ptr < 100) {
                int biomeSize = MathUtil.RandInt(Rand, biomeSizeMin, biomeSizeMax);
                Biomes b = (Biomes)MathUtil.RandInt(Rand, 0, 2);
                h = GenBiome(ptr*WidthFactor, h, biomeSize, b);
                ptr += biomeSize;
            }

            //gen bedrock
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int y = MathUtil.RandInt(Rand,1,6);
                for (int j = 0; j < y; j++) {
                    SetTile(i, j, Tile.Bedrock);
                }
            }
        }

        //returns height of last generated terrain
        private static int GenBiome(int posX, int posY, int size, Biomes biome) {
            int lastHeight = 0;
            if (biome == Biomes.Plains) {
                float heightVar = 5;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(Rand, SeaLevel, SeaLevel+heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < WidthFactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                        lastHeight = y;
                        int x = posX + i * WidthFactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 10 + MathUtil.RandDouble(Rand, 0,3)) SetTile(x, k, Tile.Stone);
                            else if (k <= y - 3 + MathUtil.RandDouble(Rand,0,2)) SetTile(x, k, Tile.Dirt);
                            else SetTile(x, k, Tile.Grass);
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = heightsGen();
                }
            } else if (biome == Biomes.Desert) {
                float heightVar = 10;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(Rand, SeaLevel, SeaLevel + heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < WidthFactor; j++) {
                        int y =MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                        lastHeight = y;
                        int x = posX + i * WidthFactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 25 + MathUtil.RandDouble(Rand, 0, 3)) SetTile(x, k, Tile.Stone);
                            else if (k <= y - 10 + MathUtil.RandDouble(Rand, 0, 4)) SetTile(x, k, Tile.Sandstone);
                            else SetTile(x, k, Tile.Sand);
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = heightsGen();
                }
            } else if (biome == Biomes.Forest) {

            } else if (biome == Biomes.Mountain) {
                float maxHeightVar = 20, minHeightVar = -3;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(Rand, minHeightVar, maxHeightVar);
                });

                float v1 = posY, v2 = v1, v3 =v2+ heightsGen(), v4 = v3+heightsGen();

                int mountainCounter = 0;
                int mountainDist = MathUtil.RandInt(Rand, 30, 150);

                int treeCounter = 0;
                int treeDist = MathUtil.RandInt(Rand, 10, 30);

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < WidthFactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                        lastHeight = y;
                        int x = posX + i * WidthFactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 3 + MathUtil.RandDouble(Rand, 0, 2)) SetTile(x, k, Tile.Stone);
                            else if (k <= y -2 + MathUtil.RandDouble(Rand, 0, 2)) SetTile(x, k, Tile.Dirt);
                            else SetTile(x, k, Tile.Grass);
                        }

                        mountainCounter++;
                        if (mountainCounter == mountainDist / 2) {
                            maxHeightVar *= -1;
                            minHeightVar *= -1;
                        }
                        if (mountainCounter == mountainDist) {
                            mountainCounter = 0;
                            mountainDist= MathUtil.RandInt(Rand, 30, 50);
                        }

                        treeCounter++;
                        if (treeCounter == treeDist) {
                            Tree(x, y + 1);
                            treeCounter = 0;
                            treeDist = MathUtil.RandInt(Rand, 10, 30);
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = v3 + heightsGen();
                }
            }

            return lastHeight;
        }

        private static void Tree(int x, int y) {
            //generate wood
            int trunkHeight = (int)(5 + 3 * Rand.NextDouble());
            for (int i = y; i < y + trunkHeight; i++) {
                SetTile(x, i, Tile.Wood);
            }

            //generate leaves
            int r = (int)(4 + 2 * Rand.NextDouble());
            int yPointer = y + trunkHeight;

            for (int i = 0; i < 2 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    SetTile(x - j, yPointer, Tile.Leaf);
                    SetTile(x + j, yPointer, Tile.Leaf);
                }
                yPointer++;
            }

            r /= 2;
            for (int i = 0; i < 1 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    SetTile(x - j, yPointer, Tile.Leaf);
                    SetTile(x + j, yPointer, Tile.Leaf);
                }
                yPointer++;
            }

            r /= 2;
            if (r < 2) r = 2;
            for (int i = 0; i < 1 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    SetTile(x - j, yPointer, Tile.Leaf);
                    SetTile(x + j, yPointer, Tile.Leaf);
                }
                yPointer++;
            }
        }


    }
}
