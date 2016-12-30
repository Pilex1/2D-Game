using Game.Util;
using System.Collections.Generic;
using Game.Core;
using System;
using OpenGL;
using Game.Terrains.Terrain_Generation;

namespace Game.Terrains.Lighting {

    static class LightingManager {

        #region Fields
        internal const int MaxLightRadius = 16;
        internal const int SunRadius = 12;
        internal static readonly Vector4 SunColour = new Vector4(1, 0.9, 0.95, 1);

        private static int[] Heights;
        private static Vector4?[,] Lightings;

        #endregion

        #region Initialisation

        internal static void Init() {
            LightingRegion.Init();
            Lightings = new Vector4?[TerrainGen.SizeX, TerrainGen.SizeY];
            Heights = new int[TerrainGen.SizeX];
        }

        internal static void LoadLightings(int region, Vector4[,] lightings) {
            LightingRegion.Init();

            for (int i = 0; i < TerrainGen.ChunkSize; i++) {
                int x = region * TerrainGen.ChunkSize + i;
                for (int y = 0; y < lightings.GetLength(1); y++) {
                    Lightings[x, y] = lightings[i, y];
                }
                for (int y = Lightings.GetLength(1) - 1; y >= 0; y--) {
                    if (Terrain.TileAt(x, y).enumId != TileID.Air) {
                        Heights[x] = y;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates lighting levels for newly generated terrain by calculating only sun lighting
        /// </summary>
        internal static void CalcFromNew() {


            for (int i = 0; i < Lightings.GetLength(0); i++) {
                for (int j = 0; j < Lightings.GetLength(1); j++) {
                    Lightings[i, j] = Vector4.Zero;
                }
            }

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = Terrain.Tiles.GetLength(1) - 1; j > 0; j--) {
                    if (Terrain.TileAt(i, j).enumId != TileID.Air) {
                        Heights[i] = j;
                        break;
                    }
                }
            }

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                AddLight(i, Heights[i], SunRadius, SunColour);
            }
        }
        #endregion

        #region Calculations

        public static Vector4? GetLighting(int x, int y) {
            return Lightings[x, y];
        }

        public static void AddTile(int x, int y) {
            if (y > Heights[x]) {
                RemoveLight(x, Heights[x], SunRadius, SunColour);
                AddLight(x, y, SunRadius, SunColour);
                Heights[x] = y;
            }
        }

        public static void RemoveTile(int x, int y) {
            if (y == Heights[x]) {
                RemoveLight(x, y, SunRadius, SunColour);
                for (int j = y - 1; j >= 0; j--) {
                    if (Terrain.TileAt(x, j).enumId != TileID.Air) {
                        Heights[x] = j;
                        AddLight(x, j, SunRadius, SunColour);
                        break;
                    }
                }
            }
        }

        public static void AddLight(int x, int y, int strength, Vector4 colour) {
            for (int i = -strength; i <= strength; i++) {
                for (int j = -strength; j <= strength; j++) {
                    AddLighting(x + i, y + j, LightingRegion.Regions[strength].GetLighting(i, j) * colour);
                }
            }
        }

        public static void RemoveLight(int x, int y, int strength, Vector4 colour) {
            for (int i = -strength; i <= strength; i++) {
                for (int j = -strength; j <= strength; j++) {
                    AddLighting(x + i, y + j, -LightingRegion.Regions[strength].GetLighting(i, j) * colour);
                }
            }
        }

        private static void AddLighting(int x, int y, Vector4 lighting) {
            if (x < 0 || x >= Lightings.GetLength(0) || y < 0 || y >= Lightings.GetLength(1)) return;
            Lightings[x, y] += lighting;
        }

        #endregion



        #region Updates

        internal static Vector4[] CalcMesh() {
            int startX, endX, startY, endY;
            Terrain.Range(out startX, out endX, out startY, out endY);
            List<Vector4> lightingsList = new List<Vector4>();
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    Tile t = Terrain.Tiles[i, j];
                    if (t == null) continue;
                    if (t.enumId != TileID.Air) {
                        Vector4 light = Lightings[i, j] != null ? (Vector4)Lightings[i, j] : Vector4.Zero;
                        float r = Math.Min(light.x / MaxLightRadius, 1);
                        float g = Math.Min(light.y / MaxLightRadius, 1);
                        float b = Math.Min(light.z / MaxLightRadius, 1);
                        float a = Math.Min(light.w / MaxLightRadius, 1);
                        Vector4 colour = new Vector4(r, g, b, a);
                        lightingsList.AddRange(new Vector4[] {
                           colour,colour,colour,colour
                        });
                    }
                }
            }
            return lightingsList.ToArray();
        }

        internal static Vector4?[,] GetLightings() {
            return Lightings;
        }

        #endregion
    }
}
