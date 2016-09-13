using System;
using Game.Assets;

namespace Game.Terrains {
    internal static class TerrainGen {

        public const int Size = 1000;
        public const int MaxHeightVariation = 20;
        public const int WidthFactor = 10;

        private static Random Rand;

        internal static void Generate(int seed) {
            Rand = new Random(seed);

            GenerateTerrain();
        }

        private static void GenerateTerrain() {
            int freq = Size / WidthFactor;
            Terrain.Tiles = new Tile[Size, 256];

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    SetTile(i, j, Tile.Air);
                }
            }

            float v1 = (float)(MaxHeightVariation * Rand.NextDouble()), v2 = (float)(MaxHeightVariation * Rand.NextDouble()), v3 = (float)(MaxHeightVariation * Rand.NextDouble()), v4 = (float)(MaxHeightVariation * Rand.NextDouble());

            for (int i = 0; i < freq; i++) {
                for (int j = 0; j < WidthFactor; j++) {
                    int y = 64 + CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / WidthFactor);
                    if (y < 1) y = 1;
                    int x = i * WidthFactor + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 10 + 3 * Rand.NextDouble()) SetTile(x, k, Tile.Stone);
                        else if (k <= y - 3 + 2 * Rand.NextDouble()) SetTile(x, k, Tile.Dirt);
                        else SetTile(x, k, Tile.Grass);
                    }

                    if (x == 0) continue;
                    if (x % 20 == 0) Tree(x, y);
                }
                v1 = v2;
                v2 = v3;
                v3 = v4;
                v4 = (float)(MaxHeightVariation * Rand.NextDouble());
            }

            //generate bedrock
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int y = (int)(1 + 5 * Rand.NextDouble());
                for (int j = 0; j < y; j++) {
                    SetTile(i, j, Tile.Bedrock);
                }
            }
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

        private static void SetTile(int x, int y, Tile tile) {
            if (x < 0 || x >= Terrain.Tiles.GetLength(0) || y < 0 || y >= Terrain.Tiles.GetLength(1)) return;
            Terrain.Tiles[x, y] = tile;
        }

        private static int CatmullRomCubicInterpolate(float y0, float y1, float y2, float y3, float mu) {
            float a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = (float)(-0.5 * y0 + 1.5 * y1 - 1.5 * y2 + 0.5 * y3);
            a1 = (float)(y0 - 2.5 * y1 + 2 * y2 - 0.5 * y3);
            a2 = (float)(-0.5 * y0 + 0.5 * y2);
            a3 = y1;

            return (int)(a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }
    }
}
