using Game.Main.Util;
using Game.Terrains.Core;
using Game.Terrains.Terrain_Generation;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game.Terrains.Lighting {

    class LightingManager : UpdateTileManager<ILight> {

        internal enum LightingOption {
            None, Jagged, Averaged, Smooth
        }

        public static LightingManager Instance { get; private set; }

        #region Fields
        internal static int MaxLightRadius = 24;
        internal static int SunRadius = 12;
        internal static ColourHSB SunColour = new ColourHSB(42, 0, 1);

        private int[] Heights;

        private List<float>[,] Hues;
        private float[,] AveragedHues;
        private float[,] Saturations;
        private float[,] Brightnesses;
        private ColourHSB[,] AveragedLightingCache;

        private LightingRegion lightingRegion;
        #endregion

        #region Initialisation
        private LightingManager(Dictionary<Vector2i, ILight> dict) : base(0, dict) {
            Hues = new List<float>[TerrainGen.SizeX, TerrainGen.SizeY];
            AveragedHues = new float[TerrainGen.SizeX, TerrainGen.SizeY];
            Saturations = new float[TerrainGen.SizeX, TerrainGen.SizeY];
            Brightnesses = new float[TerrainGen.SizeX, TerrainGen.SizeY];

            AveragedLightingCache = new ColourHSB[TerrainGen.SizeX, TerrainGen.SizeY];

            Heights = new int[TerrainGen.SizeX];

            for (int i = 0; i < TerrainGen.SizeX; i++) {
                for (int j = 0; j < TerrainGen.SizeY; j++) {
                    Hues[i, j] = new List<float>();
                }
            }

            lightingRegion = new LightingRegion();

            foreach (Vector2i v in base.dict.Keys) {
                ILight light = base.dict[v];
                AddExistingLight(v, light);
            }
        }
        internal static void Init(Dictionary<Vector2i, ILight> dict) {
            Instance = new LightingManager(dict == null ? new Dictionary<Vector2i, ILight>() : dict);
        }

        /// <summary>
        /// Calculates lighting values from sunlight for the given chunk
        /// </summary>
        internal void CalculateSunlight(int chunk) {
            for (int i = chunk * TerrainGen.ChunkSize; i < (chunk + 1) * TerrainGen.ChunkSize; i++) {
                AddExistingLight(new Vector2i(i, Heights[i]), new CLight(SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness));
            }
        }
        #endregion

        internal void CalculateAllSunlight() {
            for (int i = 0; i < TerrainGen.ChunksPerWorld; i++) {
                CalculateSunlight(i);
            }
        }

        internal void CalculateHeights(int chunk) {
            for (int i = chunk * TerrainGen.ChunkSize; i < (chunk + 1) * TerrainGen.ChunkSize; i++) {
                for (int j = Terrain.Tiles.GetLength(1) - 1; j > 0; j--) {
                    if (Terrain.TileAt(i, j).enumId != TileID.Air) {
                        Heights[i] = j;
                        break;
                    }
                }
            }
        }

        internal void CalculateAllHeights() {
            for (int i = 0; i < TerrainGen.ChunksPerWorld; i++) {
                CalculateHeights(i);
            }
        }

        #region Calculations

        public ColourHSB GetLighting(int x, int y) {
            if (!Terrain.WithinBounds(x, y)) return ColourHSB.White;
            return new ColourHSB(AveragedHues[x, y], Saturations[x, y], Brightnesses[x, y]);
        }

        public void OnTilePlace(int x, int y) {
            if (y > Heights[x]) {
                RemoveLight(new Vector2i(x, Heights[x]), new CLight(SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness));
                AddExistingLight(new Vector2i(x, y), new CLight(SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness));
                Heights[x] = y;
            }
        }

        public void OnTileRemove(int x, int y) {
            if (y == Heights[x]) {
                RemoveLight(new Vector2i(x, y), new CLight(SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness));
                for (int j = y - 1; j >= 0; j--) {
                    if (Terrain.TileAt(x, j).enumId != TileID.Air) {
                        Heights[x] = j;
                        AddExistingLight(new Vector2i(x, j), new CLight(SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the light, and returns if it added successfully (i.e. there isn't already a light at the place to be added)
        /// </summary>
        /// <param name="v">The position of the light.</param>
        /// <param name="light">The light.</param>
        /// <returns></returns>
        public bool AddLight(Vector2i v, ILight light) {
            bool contains = dict.Keys.Contains(v);
            if (!contains) {
                dict.Add(v, light);
                AddExistingLight(v, light);
            }
            return contains;
        }

        private void SetAveragedCache(int x, int y, ColourHSB colour) {
            if (!Terrain.WithinBounds(x, y)) return;
            AveragedLightingCache[x, y] = colour;
        }

        private void AddExistingLight(Vector2i v, ILight light) {
            ColourHSB colour = light.Colour();
            int radius = light.Radius();
            radius = MathUtil.Clamp(radius, 0, MaxLightRadius);
            for (int i = -(radius - 1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    float factor = lightingRegion.Regions[radius].GetLighting(i, j);
                    AddLighting(v.x + i, v.y + j, colour.Hue, colour.Saturation, factor * colour.Brightness);
                }
            }

            for (int i = -(radius - 1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    SetAveragedCache(v.x + i, v.y + j, CalculateAveragedLighting(v.x + i, v.y + j));
                }
            }
        }
        private void AddLighting(int x, int y, float hue, float saturation, float brightness) {
            if (!Terrain.WithinBounds(x, y)) return;

            Brightnesses[x, y] += brightness;

            Saturations[x, y] += saturation;

            Hues[x, y].Add(hue);
            AveragedHues[x, y] = Hues[x, y].Average();
        }

        /// <summary>
        /// Removes the light, and returns whether it was removed successfully (i.e. there was a light to remove)
        /// </summary>
        /// <param name="v">Position of the light.</param>
        /// <param name="light">The light.</param>
        /// <returns></returns>
        public bool RemoveLight(Vector2i v, ILight light) {
            ColourHSB colour = light.Colour();
            int radius = light.Radius();
            bool removed =            dict.Remove(v);
            radius = MathUtil.Clamp(radius, 0, MaxLightRadius);
            for (int i = -(radius - 1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    float factor = lightingRegion.Regions[radius].GetLighting(i, j);
                    RemoveLighting(v.x + i, v.y + j, colour.Hue, colour.Saturation, factor * colour.Brightness);
                }
            }

            for (int i = -(radius - 1); i <= (radius - 1); i++) {
                for (int j = -(radius - 1); j <= (radius - 1); j++) {
                    SetAveragedCache(v.x + i, v.y + j, CalculateAveragedLighting(v.x + i, v.y + j));
                }
            }
            return removed;
        }
        private void RemoveLighting(int x, int y, float hue, float saturation, float brightness) {
            if (!Terrain.WithinBounds(x, y)) return;

            Brightnesses[x, y] -= brightness;

            Saturations[x, y] -= saturation;

            Hues[x, y].Remove(hue);
            AveragedHues[x, y] = Hues[x, y].Count > 0 ? Hues[x, y].Average() : 0;
        }



        #endregion

        #region Updates

        internal Vector3[] CalcMesh() {
            int startX, endX, startY, endY;
            Terrain.GetRange(out startX, out endX, out startY, out endY);

            var colourHSBList = new List<ColourHSB>();
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                 //   Debug.Assert(i != 623 && j != 136);
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
        private ColourHSB AverageAroundSquare(int i, int j) {

            //error checking - if requesting for iighting outside of range of the terrain, return by default white
            if (!Terrain.WithinBounds(i, j)) return ColourHSB.White;

            //if the average lighting is cached, return it
            if (AveragedLightingCache[i, j] != null) return AveragedLightingCache[i, j];

            //else, calculate the averaged lighting and cache it for future use
            ColourHSB average = CalculateAveragedLighting(i, j);
            AveragedLightingCache[i, j] = average;
            return average;
        }

        private ColourHSB CalculateAveragedLighting(int i, int j) {
            ColourHSB average = new ColourHSBList(GetLighting(i, j), GetLighting(i + 1, j), GetLighting(i, j + 1), GetLighting(i + 1, j + 1)).GetAverage();
            return average;
        }

        protected override void OnUpdate() {

        }


        #endregion
    }
}
