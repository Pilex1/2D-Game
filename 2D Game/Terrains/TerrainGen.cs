using System;
using Game.Assets;
using Game.Util;

namespace Game.Terrains {

    enum Biomes {
        Plains, Desert, Mountain, SnowForest, Forest,
    }

    internal static class TerrainGen {

        public const int Size = 2000;
        public const int WidthFactor = 10;
        public const int Freq = Size / WidthFactor;

        public const int SeaLevel = 64;

        internal static Random Rand;

        internal static void Generate(int seed) {
            Rand = new Random(seed);

            GenerateTerrain();
            Lighting.Init();
        }

        private static void GenerateTerrain() {
            //init
            Terrain.Tiles = new TileID[Size, 256];

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    Terrain.Tiles[i, j] = TileID.Air;
                }
            }

            //gen
            int ptr = 0;
            int h = MathUtil.RandInt(Rand, SeaLevel, SeaLevel + 20);
            int biomeSizeMin = 10, biomeSizeMax = 20;
            while (ptr < Size / WidthFactor) {
                int biomeSize = MathUtil.RandInt(Rand, biomeSizeMin, biomeSizeMax);
                Biomes b = (Biomes)MathUtil.RandInt(Rand, 0, 3);
                if (b == Biomes.Plains && MathUtil.RandFloat(Rand, 0, 1) < 0.8) b = Biomes.Desert;
                h = GenBiome(ptr * WidthFactor, h, biomeSize, b);
                ptr += biomeSize;
            }

            //gen bedrock
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int y = MathUtil.RandInt(Rand, 1, 6);
                for (int j = 0; j < y; j++) {
                    Terrain.OverwriteTile(i, j, TileID.Bedrock);
                }
            }
        }

        //returns height of last generated terrain
        private static int GenBiome(int posX, int posY, int size, Biomes biome) {
            int lastHeight = 0;
            if (biome == Biomes.Plains) {
                float heightVar = 5;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(Rand, SeaLevel, SeaLevel + heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < WidthFactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                        lastHeight = y;
                        int x = posX + i * WidthFactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 10 + MathUtil.RandDouble(Rand, 0, 3)) Terrain.SetTile(x, k, TileID.Stone);
                            else if (k <= y - 3 + MathUtil.RandDouble(Rand, 0, 2)) Terrain.SetTile(x, k, TileID.Dirt);
                            else Terrain.SetTile(x, k, TileID.Grass);
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

                int cactusCounter = 0;
                int cactusDist = MathUtil.RandInt(Rand, 30, 60);

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < WidthFactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                        lastHeight = y;
                        int x = posX + i * WidthFactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k <= y - 25 + MathUtil.RandDouble(Rand, 0, 3)) Terrain.SetTile(x, k, TileID.Stone);
                            else if (k <= y - 10 + MathUtil.RandDouble(Rand, 0, 4)) Terrain.SetTile(x, k, TileID.Sandstone);
                            else Terrain.SetTile(x, k, TileID.Sand);
                        }

                        cactusCounter++;
                        if (cactusCounter == cactusDist) {
                            Cactus(x, y + 1);
                            cactusCounter = 0;
                            cactusDist = MathUtil.RandInt(Rand, 15, 20);
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

                float v1 = posY, v2 = v1, v3 = v2 + heightsGen(), v4 = v3 + heightsGen();

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
                            if (k <= y - 3 + MathUtil.RandDouble(Rand, 0, 2)) Terrain.SetTile(x, k, TileID.Stone);
                            else if (k <= y - 2 + MathUtil.RandDouble(Rand, 0, 2)) Terrain.SetTile(x, k, TileID.Dirt);
                            else Terrain.SetTile(x, k, TileID.Grass);
                        }

                        mountainCounter++;
                        if (mountainCounter == mountainDist / 2) {
                            maxHeightVar *= -1;
                            minHeightVar *= -1;
                        }
                        if (mountainCounter == mountainDist) {
                            mountainCounter = 0;
                            mountainDist = MathUtil.RandInt(Rand, 30, 50);
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
            } else if (biome == Biomes.SnowForest) {
                float heightVar = 15;

                Func<float> heightsGen = new Func<float>(delegate () {
                    return MathUtil.RandFloat(Rand, SeaLevel, SeaLevel + heightVar);
                });

                float v1 = posY, v2 = v1, v3 = heightsGen(), v4 = heightsGen();

                int treeCounter = 0;
                Func<int> treeGen = new Func<int>(delegate () { return MathUtil.RandInt(Rand, 15, 30); });
                int treeDist = treeGen();

                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < WidthFactor; j++) {
                        int y = MathUtil.CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                        lastHeight = y;
                        int x = posX + i * WidthFactor + j;

                        for (int k = 0; k <= y; k++) {
                            if (k >= y - (6 + MathUtil.RandInt(Rand, 0, 3))) {
                                Terrain.SetTile(x, k, TileID.Snow);
                            } else if (k >= 12 + MathUtil.RandInt(Rand, 0, 3)) {
                                Terrain.SetTile(x, k, TileID.Dirt);
                            } else {
                                Terrain.SetTile(x, k, TileID.Stone);
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
            int treeheight = MathUtil.RandInt(Rand, 8, 13);
            int branchesStartingHeight = MathUtil.RandInt(Rand, treeheight / 3, 2 * treeheight / 3);
            for (int i = 0; i < treeheight; i++) {
                Terrain.SetTile(x, y + i, TileID.SnowWood);
            }
            for (int i = branchesStartingHeight, c = 0; i <= treeheight; i++, c++) {
                double ratio = 1 - (double)c / (treeheight - branchesStartingHeight);
                ratio *= MathUtil.RandDouble(Rand, 0.8, 1.2);
                double branchLength = MathUtil.RandDouble(Rand, 8, 12);
                int l = (int)(branchLength * ratio * MathUtil.RandDouble(Rand, 0.8, 1.2)), r = (int)(branchLength * ratio * MathUtil.RandDouble(Rand, 0.8, 1.2));
                for (int j = 0; j < l; j++) {
                    Terrain.SetTile(x - j, y + i, TileID.SnowLeaf);
                }
                for (int j = 0; j < r; j++) {
                    Terrain.SetTile(x + j, y + i, TileID.SnowLeaf);
                }
            }

        }

        private static void Tree(int x, int y) {
            //generate wood
            int trunkHeight = (int)(5 + 3 * Rand.NextDouble());
            for (int i = y; i < y + trunkHeight; i++) {
                Terrain.SetTile(x, i, TileID.Wood);
            }

            //generate leaves
            int r = (int)(4 + 2 * Rand.NextDouble());
            int yPointer = y + trunkHeight;

            for (int i = 0; i < 2 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTile(x - j, yPointer, TileID.Leaf);
                    Terrain.SetTile(x + j, yPointer, TileID.Leaf);
                }
                yPointer++;
            }

            r /= 2;
            for (int i = 0; i < 1 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTile(x - j, yPointer, TileID.Leaf);
                    Terrain.SetTile(x + j, yPointer, TileID.Leaf);
                }
                yPointer++;
            }

            r /= 2;
            if (r < 2) r = 2;
            for (int i = 0; i < 1 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTile(x - j, yPointer, TileID.Leaf);
                    Terrain.SetTile(x + j, yPointer, TileID.Leaf);
                }
                yPointer++;
            }
        }


        private static void Cactus(int x, int y) {
            int height = MathUtil.RandInt(Rand, 5, 8);
            //stem
            for (int i = 0; i < height; i++) {
                Terrain.SetTile(x, y + i, TileID.Cactus);
            }

            if (MathUtil.RandDouble(Rand, 0, 1) < 0.8) {
                RecCactus(x, y + height - 1, 0.3f);
            }

        }

        private static void RecCactus(int x, int y, float recFactor) {
            int l = MathUtil.RandInt(Rand, 1, 2);
            int r = MathUtil.RandInt(Rand, 1, 2);
            for (int i = x - l; i <= x + r; i++) {
                Terrain.SetTile(i, y, TileID.Cactus);
            }

            int hl = MathUtil.RandInt(Rand, 3, 5);
            int hr = MathUtil.RandInt(Rand, 3, 5);
            for (int i = y + 1; i <= y + hl; i++) {
                Terrain.SetTile(x - l, i, TileID.Cactus);
            }
            for (int i = y + 1; i <= y + hr; i++) {
                Terrain.SetTile(x + r, i, TileID.Cactus);
            }

            if (MathUtil.RandDouble(Rand, 0, 1) < recFactor) {
                RecCactus(x - l, y + hl, recFactor * MathUtil.RandFloat(Rand, 0.7f, 0.9f));
                RecCactus(x + hl, y + hr, recFactor * MathUtil.RandFloat(Rand, 0.7f, 0.9f));
            }

        }

    }
}
