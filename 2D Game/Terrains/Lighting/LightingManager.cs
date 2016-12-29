using Game.Util;
using System.Collections.Generic;
using Game.Core;
using System;

namespace Game.Terrains.Lighting {

    static class LightingManager {

        #region Fields
        internal const int MaxLightRadius = 16;
        internal const int SunRadius = 12;

        private static int[] Heights;
        internal static float[,] Lightings;

        #endregion

        #region Initialisation
        internal static void Init() {
            LightingRegion.Init();
            Lightings = new float[Terrain.Tiles.GetLength(0), Terrain.Tiles.GetLength(1)];

            Heights = new int[Terrain.Tiles.GetLength(0)];
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = Terrain.Tiles.GetLength(1) - 1; j > 0; j--) {
                    if (Terrain.TileAt(i, j).enumId != TileID.Air) {
                        Heights[i] = j;
                        break;
                    }
                }
            }

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                AddLight(i, Heights[i], SunRadius);
            }
        }
        #endregion

        #region Calculations

        public static void AddTile(int x, int y) {
            if (y > Heights[x]) {
                RemoveLight(x, Heights[x], SunRadius);
                AddLight(x, y, SunRadius);
                Heights[x] = y;
            }
        }

        public static void RemoveTile(int x, int y) {
            if (y == Heights[x]) {
                RemoveLight(x, y, SunRadius);
                for (int j = y - 1; j >= 0; j--) {
                    if (Terrain.TileAt(x, j).enumId != TileID.Air) {
                        Heights[x] = j;
                        AddLight(x, j, SunRadius);
                        break;
                    }
                }
            }
        }

        public static void AddLight(int x, int y, int strength) {
            for (int i = -strength; i <= strength; i++) {
                for (int j = -strength; j <= strength; j++) {
                    AddLighting(x + i, y + j, LightingRegion.Regions[strength].GetLighting(i, j));
                }
            }
        }

        public static void RemoveLight(int x, int y, int strength) {
            for (int i = -strength; i <= strength; i++) {
                for (int j = -strength; j <= strength; j++) {
                    AddLighting(x + i, y + j, -LightingRegion.Regions[strength].GetLighting(i, j));
                }
            }
        }

        private static void AddLighting(int x, int y, float lighting) {
            if (x < 0 || x >= Lightings.GetLength(0) || y < 0 || y >= Lightings.GetLength(1)) return;
            Lightings[x, y] += lighting;
        }

        #endregion



        #region Updates



        internal static float[] CalcMesh() {
            int startX, endX, startY, endY;
            Terrain.Range(out startX, out endX, out startY, out endY);
            List<float> lightingsList = new List<float>();
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    if (Terrain.Tiles[i, j].enumId != TileID.Air) {
                        float val = Math.Min(Lightings[i, j] / MaxLightRadius, 1);
                        lightingsList.AddRange(new float[] {
                            val,val,val,val
                        });
                    }
                }
            }
            return lightingsList.ToArray();
        }
        #endregion
    }
}
