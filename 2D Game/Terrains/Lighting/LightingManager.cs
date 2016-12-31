using Game.Util;
using System.Collections.Generic;
using OpenGL;
using Game.Terrains.Terrain_Generation;

namespace Game.Terrains.Lighting {

    static class LightingManager {

        internal enum LightingOption {
            None, Jagged, Averaged, Smooth
        }

        #region Fields
        internal const int MaxLightRadius = 24;
        internal const int SunRadius = 12;
        internal const float SunStrength = 0.1f;
        internal static readonly Vector3 SunColour = new Vector3(1, 0.9, 0.95);

        private static int[] Heights;
        private static Vector3[,] Lightings;

        private static Vector3?[,] AveragedAroundSquare_Cache;

        #endregion

        #region Initialisation

        internal static void Init() {
            LightingRegion.Init();
            Lightings = new Vector3[TerrainGen.SizeX, TerrainGen.SizeY];
            Heights = new int[TerrainGen.SizeX];
        }

        internal static void LoadLightings(int region, Vector3[,] lightings) {
            for (int i = 0; i < TerrainGen.ChunkSize; i++) {
                int x = region * TerrainGen.ChunkSize + i;
                for (int y = 0; y < lightings.GetLength(1); y++) {
                    Lightings[x, y] += lightings[i, y];
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
                    Lightings[i, j] = Vector3.Zero;
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
                AddLight(i, Heights[i], SunRadius, SunStrength, SunColour);
            }
        }
        #endregion

        #region Calculations

        public static Vector3 GetLighting(int x, int y) {
            if (x < 0 || x >= Lightings.GetLength(0) || y < 0 || y >= Lightings.GetLength(1)) return Vector3.Zero;
            return Lightings[x, y];
        }

        public static void AddTile(int x, int y) {
            if (y > Heights[x]) {
                RemoveLight(x, Heights[x], SunRadius, SunStrength, SunColour);
                AddLight(x, y, SunRadius, SunStrength, SunColour);
                Heights[x] = y;
            }
        }

        public static void RemoveTile(int x, int y) {
            if (y == Heights[x]) {
                RemoveLight(x, y, SunRadius, SunStrength, SunColour);
                for (int j = y - 1; j >= 0; j--) {
                    if (Terrain.TileAt(x, j).enumId != TileID.Air) {
                        Heights[x] = j;
                        AddLight(x, j, SunRadius, SunStrength, SunColour);
                        break;
                    }
                }
            }
        }

        public static void AddLight(int x, int y, int radius, float strength, Vector3 colour) {
            radius = MathUtil.Clamp(radius, 0, MaxLightRadius);
            for (int i = -(radius-1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    AddLighting(x + i, y + j, strength * LightingRegion.Regions[radius].GetLighting(i, j) * colour);
                }
            }
        }

        public static void RemoveLight(int x, int y, int radius, float strength, Vector3 colour) {
            AddLight(x, y, radius, -strength, colour);
        }

        private static void AddLighting(int x, int y, Vector3 lighting) {
            if (x < 0 || x >= Lightings.GetLength(0) || y < 0 || y >= Lightings.GetLength(1)) return;
            Lightings[x, y] += lighting;
        }

        #endregion

        #region Updates

        internal static Vector3[] CalcMesh() {
            AveragedAroundSquare_Cache = new Vector3?[Lightings.GetLength(0), Lightings.GetLength(1)];
            int startX, endX, startY, endY;
            Terrain.Range(out startX, out endX, out startY, out endY);
            List<Vector3> lightingsList = new List<Vector3>();
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    Tile t = Terrain.Tiles[i, j];
                    if (t == null) continue;
                    if (t.enumId != TileID.Air) {
                        switch (GameLogic.LightingOption.Get()) {
                            case LightingOption.None:
                                lightingsList.AddRange(new Vector3[] { Vector3.Identity, Vector3.Identity, Vector3.Identity, Vector3.Identity });
                                break;
                            case LightingOption.Jagged:
                                lightingsList.AddRange(JaggedLighting(i, j));
                                break;
                            case LightingOption.Averaged:
                                lightingsList.AddRange(AveragedLighting(i, j));
                                break;
                            case LightingOption.Smooth:
                                lightingsList.AddRange(SmoothLighting(i, j));
                                break;
                        }
                    }
                }
            }
            return lightingsList.ToArray();
        }

        //top left, bottom left, bottom right, top right
        private static Vector3[] SmoothLighting(int i, int j) {
            return new Vector3[] { AverageAroundSquare(i - 1, j), AverageAroundSquare(i - 1, j - 1), AverageAroundSquare(i, j - 1), AverageAroundSquare(i, j) };
        }
        private static Vector3[] AveragedLighting(int i, int j) {
            Vector3 x = AverageAroundCross(i, j);
            return new Vector3[] { x, x, x, x };
        }
        //origin at bottom left
        private static Vector3 AverageAroundSquare(int i, int j) {
            if (i < 0 || i >= Lightings.GetLength(0) || j < 0 || j >= Lightings.GetLength(1)) return Vector3.Zero;
            if (AveragedAroundSquare_Cache[i,j] != null) {
                return (Vector3)AveragedAroundSquare_Cache[i, j];
            }
            Vector3 avg = (GetLighting(i, j) + GetLighting(i + 1, j) + GetLighting(i, j + 1) + GetLighting(i + 1, j + 1)) / 4;
            AveragedAroundSquare_Cache[i, j] = avg;
            return avg;
        }
        private static Vector3 AverageAroundCross(int i, int j) => (GetLighting(i, j + 1) + GetLighting(i - 1, j) + GetLighting(i, j) + GetLighting(i + 1, j) + GetLighting(i, j - 1)) / 5;
        private static Vector3[] JaggedLighting(int i, int j) => new Vector3[] { Lightings[i, j], Lightings[i, j], Lightings[i, j], Lightings[i, j] };


        internal static Vector3[,] GetLightings() {
            return Lightings;
        }

        #endregion
    }
}
