using Game.Main.Util;
using Game.Terrains.Terrain_Generation;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;
using System.Linq;

namespace Game.Terrains.Lighting {

    static class LightingManager {

        internal enum LightingOption {
            None, Jagged, Averaged, Smooth
        }

        #region Fields
        internal const int MaxLightRadius = 24;

        internal const int SunRadius = 12;
        internal static ColourHSB SunColour = new ColourHSB(42, 1, 1);

        private static int[] Heights;

        private static List<float>[,] Hues;
        private static float[,] AveragedHues;

        private static float[,] Saturations;

        private static float[,] Brightnesses;

        private static ColourHSB[,] AveragedAroundSquare_Cache;

        #endregion

        #region Initialisation

        internal static void Init() {
            LightingRegion.Init();

            Hues = new List<float>[TerrainGen.SizeX, TerrainGen.SizeY];
            AveragedHues = new float[TerrainGen.SizeX, TerrainGen.SizeY];
            Saturations = new float[TerrainGen.SizeX, TerrainGen.SizeY];
            Brightnesses = new float[TerrainGen.SizeX, TerrainGen.SizeY];

            AveragedAroundSquare_Cache = new ColourHSB[TerrainGen.SizeX, TerrainGen.SizeY];

            Heights = new int[TerrainGen.SizeX];

        }

        /// <summary>
        /// Calculates lighting levels for newly generated terrain by calculating only sun lighting
        /// </summary>
        internal static void CalcFromNew() {


            for (int i = 0; i < TerrainGen.SizeX; i++) {
                for (int j = 0; j < TerrainGen.SizeY; j++) {
                    Hues[i, j] = new List<float>();
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
                AddLight(i, Heights[i], SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness);
            }
        }
        #endregion

        #region Calculations

        public static ColourHSB GetLighting(int x, int y) {
            if (!Terrain.WithinBounds(x, y)) return ColourHSB.White;
            return new ColourHSB(AveragedHues[x, y], Saturations[x, y], Brightnesses[x, y]);
        }

        public static void AddTile(int x, int y) {
            if (y > Heights[x]) {
                RemoveLight(x, Heights[x], SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness);
                AddLight(x, y, SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness);
                Heights[x] = y;
            }
        }

        public static void RemoveTile(int x, int y) {
            if (y == Heights[x]) {
                RemoveLight(x, y, SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness);
                for (int j = y - 1; j >= 0; j--) {
                    if (Terrain.TileAt(x, j).enumId != TileID.Air) {
                        Heights[x] = j;
                        AddLight(x, j, SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness);
                        break;
                    }
                }
            }
        }

        public static void AddLight(int x, int y, ILight light) => AddLight(x, y, light.Radius(), light.Colour().Hue, light.Colour().Saturation, light.Colour().Brightness);
        public static void AddLight(int x, int y, int radius, float hue, float saturation, float brightness) {
            radius = MathUtil.Clamp(radius, 0, MaxLightRadius);
            for (int i = -(radius - 1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    float factor = LightingRegion.Regions[radius].GetLighting(i, j);
                    AddLighting(x + i, y + j, hue, saturation, factor * brightness);
                }
            }
        }
        private static void AddLighting(int x, int y, float hue, float saturation, float brightness) {
            if (!Terrain.WithinBounds(x, y)) return;

            Brightnesses[x, y] += brightness;

            Saturations[x, y] += saturation;

            Hues[x, y].Add(hue);
            AveragedHues[x, y] = Hues[x, y].Average();
        }

        public static void RemoveLight(int x, int y, ILight light) => RemoveLight(x, y, light.Radius(), light.Colour().Hue, light.Colour().Saturation, light.Colour().Brightness);
        public static void RemoveLight(int x, int y, int radius, float hue, float saturation, float brightness) {
            radius = MathUtil.Clamp(radius, 0, MaxLightRadius);
            for (int i = -(radius - 1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    float factor = LightingRegion.Regions[radius].GetLighting(i, j);
                    RemoveLighting(x + i, y + j, hue, saturation, factor * brightness);
                }
            }
        }
        private static void RemoveLighting(int x, int y, float hue, float saturation, float brightness) {
            if (!Terrain.WithinBounds(x, y)) return;

            Brightnesses[x, y] -= brightness;

            Saturations[x, y] -= saturation;

            Hues[x, y].Remove(hue);
            AveragedHues[x, y] = Hues[x, y].Average();
        }



        #endregion

        #region Updates

        internal static Vector3[] CalcMesh() {
            int startX, endX, startY, endY;
            Terrain.GetRange(out startX, out endX, out startY, out endY);

            var colourHSBList = new List<ColourHSB>();
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    Tile t = Terrain.Tiles[i, j];
                    if (t == null || t.enumId == TileID.Air) continue;
                    switch (GameLogic.LightingOption.Get()) {
                        case LightingOption.None:
                            colourHSBList.AddRange(new ColourHSB[] { ColourHSB.White, ColourHSB.White, ColourHSB.White, ColourHSB.White });
                            break;
                        case LightingOption.Jagged:
                            ColourHSB colourJagged = new ColourHSB(AveragedHues[i, j], Saturations[i, j], Brightnesses[i, j]);
                            colourHSBList.AddRange(new ColourHSB[] { colourJagged, colourJagged, colourJagged, colourJagged });
                            break;
                        case LightingOption.Averaged:
                            ColourHSB colourAveraged = new ColourHSBList(GetLighting(i, j + 1), GetLighting(i - 1, j), GetLighting(i, j), GetLighting(i + 1, j), GetLighting(i, j - 1)).GetAverage();
                            colourHSBList.AddRange(new ColourHSB[] { colourAveraged, colourAveraged, colourAveraged, colourAveraged });
                            break;
                        case LightingOption.Smooth:
                            colourHSBList.AddRange(new ColourHSB[] { AverageAroundSquare(i - 1, j), AverageAroundSquare(i - 1, j - 1), AverageAroundSquare(i, j - 1), AverageAroundSquare(i, j) });
                            break;
                    }
                }

            }

            var array = new Vector3[colourHSBList.Count];
            for (int i = 0; i < colourHSBList.Count; i++) {
                var hsb = colourHSBList[i];
                array[i] = new Vector3(hsb.Hue, hsb.Saturation, hsb.Brightness);
            }

            return array;
        }

        //origin at bottom left
        private static ColourHSB AverageAroundSquare(int i, int j) {
            if (!Terrain.WithinBounds(i, j)) return ColourHSB.White;
            if (AveragedAroundSquare_Cache[i, j] != null) return AveragedAroundSquare_Cache[i, j];

            ColourHSB average = new ColourHSBList(GetLighting(i, j), GetLighting(i + 1, j), GetLighting(i, j + 1), GetLighting(i + 1, j + 1)).GetAverage();
            AveragedAroundSquare_Cache[i, j] = average;
            return average;
        }


        #endregion
    }
}
