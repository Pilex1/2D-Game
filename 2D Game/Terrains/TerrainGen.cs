using System;
using System.Diagnostics;
using Game.Assets;
using Game.Util;

namespace Game.Terrains {

    enum Biome {
        None, Plains, Desert, Mountain, SnowForest, Forest,
    }

    internal static class TerrainGen {

        public const int size = 4000;
        private const int widthfactor = 10;
        private const int freq = size / widthfactor;

        private const int minlandheight = 128;

        internal static Random rand;

        internal static void Generate(int seed) {
            rand = new Random(seed);

            Terrain.Tiles = new Tile[size, 512];
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    Terrain.Tiles[i, j] = Tile.Air;
                }
            }

            Terrain.TerrainBiomes = new Biome[size];
            for (int i = 0; i < Terrain.TerrainBiomes.GetLength(0); i++) {
                Terrain.TerrainBiomes[i] = Biome.None;
            }



            GenTerrain();

           // CaveGen.GenCaves();

            GenBedrock();

            GenDeco();
        }

        private static void GenDeco() {
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {

            }
        }

        private static void GenBedrock() {
            //gen bedrock
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int y = MathUtil.RandInt(rand, 1, 6);
                for (int j = 0; j < y; j++) {
                    Terrain.SetTileTerrainGen(i, j, Tile.Bedrock, true);
                }
            }
        }

        private static void GenTerrain() {

            //gen
            int ptr = 0;
            int h = MathUtil.RandInt(rand, minlandheight, minlandheight + 20);
            int biomeSizeMin = 10, biomeSizeMax = 20;
            while (ptr < size / widthfactor) {
                int biomeSize = MathUtil.RandInt(rand, biomeSizeMin, biomeSizeMax);
                Biome b = (Biome)MathUtil.RandInt(rand, (int)Biome.Plains, (int)Biome.SnowForest);
                h = GenBiome(ptr * widthfactor, h, biomeSize, b);
                for (int i = ptr; i < ptr + biomeSize; i++) {
                    Terrain.TerrainBiomes[i] = b;
                }
                ptr += biomeSize;
            }


        }



        //returns height of last generated terrain
        private static int GenBiome(int posX, int posY, int size, Biome biome) {
            int lastHeight = 0;
            if (biome == Biome.Plains) {
                float heightVar = 5;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(rand, minlandheight, minlandheight + heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < widthfactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / widthfactor);
                        lastHeight = y;
                        int x = posX + i * widthfactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 10 + MathUtil.RandDouble(rand, 0, 3)) Terrain.SetTileTerrainGen(x, k, Tile.Stone, true);
                            else if (k <= y - 3 + MathUtil.RandDouble(rand, 0, 2)) Terrain.SetTileTerrainGen(x, k, Tile.Dirt, true);
                            else Terrain.SetTileTerrainGen(x, k, Tile.Grass, true);
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = heightsGen();
                }
            } else if (biome == Biome.Desert) {
                float heightVar = 10;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(rand, minlandheight, minlandheight + heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                int cactusCounter = 0;
                int cactusDist = MathUtil.RandInt(rand, 30, 60);

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < widthfactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / widthfactor);
                        lastHeight = y;
                        int x = posX + i * widthfactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 25 + MathUtil.RandDouble(rand, 0, 3)) Terrain.SetTileTerrainGen(x, k, Tile.Stone, true);
                            else if (k <= y - 10 + MathUtil.RandDouble(rand, 0, 4)) Terrain.SetTileTerrainGen(x, k, Tile.Sandstone, true);
                            else Terrain.SetTileTerrainGen(x, k, Tile.Sand, true);
                        }

                        cactusCounter++;
                        if (cactusCounter == cactusDist) {
                            Cactus(x, y + 1);
                            cactusCounter = 0;
                            cactusDist = MathUtil.RandInt(rand, 15, 20);
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = heightsGen();
                }
            } else if (biome == Biome.Forest) {

            } else if (biome == Biome.Mountain) {
                float maxHeightVar = 20, minHeightVar = -3;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(rand, minHeightVar, maxHeightVar);
                });

                float v1 = posY, v2 = v1, v3 = v2 + heightsGen(), v4 = v3 + heightsGen();

                int mountainCounter = 0;
                int mountainDist = MathUtil.RandInt(rand, 30, 150);

                int treeCounter = 0;
                int treeDist = MathUtil.RandInt(rand, 10, 30);

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < widthfactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / widthfactor);
                        lastHeight = y;
                        int x = posX + i * widthfactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 3 + MathUtil.RandDouble(rand, 0, 2)) Terrain.SetTileTerrainGen(x, k, Tile.Stone, true);
                            else if (k <= y - 2 + MathUtil.RandDouble(rand, 0, 2)) Terrain.SetTileTerrainGen(x, k, Tile.Dirt, true);
                            else Terrain.SetTileTerrainGen(x, k, Tile.Grass, true);
                        }

                        mountainCounter++;
                        if (mountainCounter == mountainDist / 2 || y < minlandheight) {
                            maxHeightVar *= -1;
                            minHeightVar *= -1;
                        }
                        if (mountainCounter == mountainDist) {
                            mountainCounter = 0;
                            mountainDist = MathUtil.RandInt(rand, 30, 50);
                        }

                        treeCounter++;
                        if (treeCounter == treeDist) {
                            Tree(x, y + 1);
                            treeCounter = 0;
                            treeDist = MathUtil.RandInt(rand, 10, 30);
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = v3 + heightsGen();
                }
            } else if (biome == Biome.SnowForest) {
                float heightVar = 15;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(rand, minlandheight, minlandheight + heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                int treeCounter = 0;
                Func<int> treeGen = new Func<int>(delegate () { return MathUtil.RandInt(rand, 15, 30); });
                int treeDist = treeGen();

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < widthfactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / widthfactor);
                        lastHeight = y;
                        int x = posX + i * widthfactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k >= y - (6 + MathUtil.RandInt(rand, 0, 3))) {
                                Terrain.SetTileTerrainGen(x, k, Tile.Snow, true);
                            } else if (k >= 12 + MathUtil.RandInt(rand, 0, 3)) {
                                Terrain.SetTileTerrainGen(x, k, Tile.Dirt, true);
                            } else {
                                Terrain.SetTileTerrainGen(x, k, Tile.Stone, true);
                            }
                        }

                        treeCounter++;
                        if (treeCounter == treeDist) {
                            SnowTree(x, y + 1);
                            treeCounter = 0;
                            treeDist = treeGen();
                        }
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                    v4 = heightsGen();
                }
            }

            return lastHeight;
        }

        private static void SnowTree(int x, int y) {
            int treeheight = MathUtil.RandInt(rand, 13, 18);
            int branchesStartingHeight = MathUtil.RandInt(rand, treeheight / 3, 2 * treeheight / 3);
            for (int i = 0; i < treeheight; i++) {
                Terrain.SetTileTerrainGen(x, y + i, Tile.SnowWood, false);
            }
            for (int i = branchesStartingHeight, c = 0; i <= treeheight + MathUtil.RandInt(rand, 2, 5); i++, c++) {
                double ratio = 1 - (double)c / (treeheight - branchesStartingHeight);
                ratio *= MathUtil.RandDouble(rand, 0.9, 1.1);
                double branchLength = MathUtil.RandDouble(rand, 10, 11);
                int l = (int)(branchLength * ratio), r = (int)(branchLength * ratio);
                for (int j = 0; j < l; j++) {
                    Terrain.SetTileTerrainGen(x - j, y + i, Tile.SnowLeaf, false);
                }
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x + j, y + i, Tile.SnowLeaf, false);
                }
            }

        }

        private static void Tree(int x, int y) {
            //generate wood
            int trunkHeight = (int)(5 + 3 * rand.NextDouble());
            for (int i = y; i < y + trunkHeight; i++) {
                Terrain.SetTileTerrainGen(x, i, Tile.Wood, false);
            }

            //generate leaves
            int r = (int)(4 + 2 * rand.NextDouble());
            int yPointer = y + trunkHeight;

            for (int i = 0; i < 2 + 2 * rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x - j, yPointer, Tile.Leaf, false);
                    Terrain.SetTileTerrainGen(x + j, yPointer, Tile.Leaf, false);
                }
                yPointer++;
            }

            r /= 2;
            for (int i = 0; i < 1 + 2 * rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x - j, yPointer, Tile.Leaf, false);
                    Terrain.SetTileTerrainGen(x + j, yPointer, Tile.Leaf, false);
                }
                yPointer++;
            }

            r /= 2;
            if (r < 2) r = 2;
            for (int i = 0; i < 1 + 2 * rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x - j, yPointer, Tile.Leaf, false);
                    Terrain.SetTileTerrainGen(x + j, yPointer, Tile.Leaf, false);
                }
                yPointer++;
            }
        }


        private static void Cactus(int x, int y) {
            int height = MathUtil.RandInt(rand, 5, 8);
            //stem
            for (int i = 0; i < height; i++) {
                Terrain.SetTileTerrainGen(x, y + i, Tile.Cactus, false);
            }

            if (MathUtil.RandDouble(rand, 0, 1) < 0.8) {
                RecCactus(x, y + height - 1, 0.3f);
            }

        }

        private static void RecCactus(int x, int y, float recFactor) {
            int l = MathUtil.RandInt(rand, 1, 2);
            int r = MathUtil.RandInt(rand, 1, 2);
            for (int i = x - l; i <= x + r; i++) {
                Terrain.SetTileTerrainGen(i, y, Tile.Cactus, false);
            }

            int hl = MathUtil.RandInt(rand, 3, 5);
            int hr = MathUtil.RandInt(rand, 3, 5);
            for (int i = y + 1; i <= y + hl; i++) {
                Terrain.SetTileTerrainGen(x - l, i, Tile.Cactus, false);
            }
            for (int i = y + 1; i <= y + hr; i++) {
                Terrain.SetTileTerrainGen(x + r, i, Tile.Cactus, false);
            }

            if (MathUtil.RandDouble(rand, 0, 1) < recFactor) {
                RecCactus(x - l, y + hl, recFactor * MathUtil.RandFloat(rand, 0.7f, 0.9f));
                RecCactus(x + hl, y + hr, recFactor * MathUtil.RandFloat(rand, 0.7f, 0.9f));
            }

        }
    }
}
