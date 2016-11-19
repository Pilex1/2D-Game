using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains.Gen {
    static class Nature {

        #region Tree
        internal static void GenerateTree(int x, int y) {
            //generate wood
            int trunkHeight = (int)(5 + 3 * TerrainGen.rand.NextDouble());
            for (int i = y; i < y + trunkHeight; i++) {
                Terrain.SetTileTerrainGen(x, i, Tile.Wood, false);
            }

            //generate leaves
            int r = (int)(4 + 2 * TerrainGen.rand.NextDouble());
            int yPointer = y + trunkHeight;

            for (int i = 0; i < 2 + 2 * TerrainGen.rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x - j, yPointer, Tile.Leaf, false);
                    Terrain.SetTileTerrainGen(x + j, yPointer, Tile.Leaf, false);
                }
                yPointer++;
            }

            r /= 2;
            for (int i = 0; i < 1 + 2 * TerrainGen.rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x - j, yPointer, Tile.Leaf, false);
                    Terrain.SetTileTerrainGen(x + j, yPointer, Tile.Leaf, false);
                }
                yPointer++;
            }

            r /= 2;
            if (r < 2) r = 2;
            for (int i = 0; i < 1 + 2 * TerrainGen.rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x - j, yPointer, Tile.Leaf, false);
                    Terrain.SetTileTerrainGen(x + j, yPointer, Tile.Leaf, false);
                }
                yPointer++;
            }
        }
        #endregion

        #region Cactus
        internal static void GenerateCactus(int x, int y) {
            int height = MathUtil.RandInt(TerrainGen.rand, 5, 8);
            //stem
            for (int i = 0; i < height; i++) {
                Terrain.SetTileTerrainGen(x, y + i, Tile.Cactus, false);
            }

            if (MathUtil.RandDouble(TerrainGen.rand, 0, 1) < 0.8) {
                RecCactus(x, y + height - 1, 0.3f);
            }

        }

        private static void RecCactus(int x, int y, float recFactor) {
            int l = MathUtil.RandInt(TerrainGen.rand, 1, 2);
            int r = MathUtil.RandInt(TerrainGen.rand, 1, 2);
            for (int i = x - l; i <= x + r; i++) {
                Terrain.SetTileTerrainGen(i, y, Tile.Cactus, false);
            }

            int hl = MathUtil.RandInt(TerrainGen.rand, 3, 5);
            int hr = MathUtil.RandInt(TerrainGen.rand, 3, 5);
            for (int i = y + 1; i <= y + hl; i++) {
                Terrain.SetTileTerrainGen(x - l, i, Tile.Cactus, false);
            }
            for (int i = y + 1; i <= y + hr; i++) {
                Terrain.SetTileTerrainGen(x + r, i, Tile.Cactus, false);
            }

            if (MathUtil.RandDouble(TerrainGen.rand, 0, 1) < recFactor) {
                RecCactus(x - l, y + hl, recFactor * MathUtil.RandFloat(TerrainGen.rand, 0.7f, 0.9f));
                RecCactus(x + hl, y + hr, recFactor * MathUtil.RandFloat(TerrainGen.rand, 0.7f, 0.9f));
            }

        }
        #endregion

        #region Snow Tree
        internal static void GenerateSnowTree(int x, int y) {
            int treeheight = MathUtil.RandInt(TerrainGen.rand, 13, 18);
            int branchesStartingHeight = MathUtil.RandInt(TerrainGen.rand, treeheight / 3, 2 * treeheight / 3);
            for (int i = 0; i < treeheight; i++) {
                Terrain.SetTileTerrainGen(x, y + i, Tile.SnowWood, false);
            }
            for (int i = branchesStartingHeight, c = 0; i <= treeheight + MathUtil.RandInt(TerrainGen.rand, 2, 5); i++, c++) {
                double ratio = 1 - (double)c / (treeheight - branchesStartingHeight);
                ratio *= MathUtil.RandDouble(TerrainGen.rand, 0.9, 1.1);
                double branchLength = MathUtil.RandDouble(TerrainGen.rand, 10, 11);
                int l = (int)(branchLength * ratio), r = (int)(branchLength * ratio);
                for (int j = 0; j < l; j++) {
                    Terrain.SetTileTerrainGen(x - j, y + i, Tile.SnowLeaf, false);
                }
                for (int j = 0; j < r; j++) {
                    Terrain.SetTileTerrainGen(x + j, y + i, Tile.SnowLeaf, false);
                }
            }

        }
        #endregion
    }
}
