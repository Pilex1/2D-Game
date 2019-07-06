using Game.Main.Util;
using Game.Terrains.Core;
using Game.Terrains.Terrain_Generation;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Terrains.Logics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System;

namespace Game.Terrains.Lighting {

	class LightingManager : UpdateTileManager<ILight> {

		internal enum LightingOption {
			None,
			Jagged,
			Averaged,
			Smooth
		}

		public static LightingManager Instance { get; private set; }

		#region Fields

		private LightingRegion lightingRegion;

		internal const int MaxLightRadius = 24;
		internal const int SunRadius = 12;
		internal const float SunStrength = 0.1f;
		internal static readonly Vector3 SunColour = new ColourRGB (255, 230, 240).ToVec3 ();

		private static int[] Heights;
		private static Vector3[,] Lightings;

		private static Vector3?[,] AveragedAroundSquare_Cache;

		#endregion

		#region Initialisation

		private LightingManager(Dictionary<Vector2i, ILight> dict) : base (0, dict) {
			lightingRegion = new LightingRegion ();

			Lightings = new Vector3[TerrainGen.SizeX, TerrainGen.SizeY];
			Heights = new int[TerrainGen.SizeX];

//			foreach (Vector2i v in base.dict.Keys) {
//				ILight light = base.dict [v];
//				AddExistingLight (v, light);
//			}
		}

		internal static void Init(Dictionary<Vector2i, ILight> dict) {
			Instance = new LightingManager (dict == null ? new Dictionary<Vector2i, ILight> () : dict);
		}

		// new
		internal void LoadLightings(int region, Vector3[,] lightings) {
			for (int i = 0; i < TerrainGen.ChunkSize; i++) {
				int x = region * TerrainGen.ChunkSize + i;
				for (int y = 0; y < lightings.GetLength (1); y++) {
					Lightings [x, y] += lightings [i, y];
				}
				for (int y = Lightings.GetLength (1) - 1; y >= 0; y--) {
					if (Terrain.TileAt (x, y).enumId != TileID.Air) {
						Heights [x] = y;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Calculates lighting values from sunlight for the given chunk
		/// </summary>
		//		internal void CalculateSunlight(int chunk) {
		//			for (int i = chunk * TerrainGen.ChunkSize; i < (chunk + 1) * TerrainGen.ChunkSize; i++) {
		//				AddExistingLight (new Vector2i (i, Heights [i]), new CLight (SunRadius, SunColour.Hue, SunColour.Saturation, SunColour.Brightness));
		//			}
		//		}

		/// <summary>
		/// Calculates lighting levels for newly generated terrain by calculating only sun lighting
		/// </summary>
		internal void CalcFromNew() {


			for (int i = 0; i < Lightings.GetLength (0); i++) {
				for (int j = 0; j < Lightings.GetLength (1); j++) {
					Lightings [i, j] = Vector3.Zero;
				}
			}

			for (int i = 0; i < Terrain.Tiles.GetLength (0); i++) {
				for (int j = Terrain.Tiles.GetLength (1) - 1; j > 0; j--) {
					if (Terrain.TileAt (i, j).enumId != TileID.Air) {
						Heights [i] = j;
						break;
					}
				}
			}

			for (int i = 0; i < Terrain.Tiles.GetLength (0); i++) {
				AddLight (i, Heights [i], SunRadius, SunStrength, SunColour);
			}
		}

		#endregion

		//		internal void CalculateAllSunlight() {
		//			for (int i = 0; i < TerrainGen.ChunksPerWorld; i++) {
		//				CalculateSunlight (i);
		//			}
		//		}

		//		internal void CalculateHeights(int chunk) {
		//			for (int i = chunk * TerrainGen.ChunkSize; i < (chunk + 1) * TerrainGen.ChunkSize; i++) {
		//				for (int j = Terrain.Tiles.GetLength (1) - 1; j > 0; j--) {
		//					if (Terrain.TileAt (i, j).enumId != TileID.Air) {
		//						Heights [i] = j;
		//						break;
		//					}
		//				}
		//			}
		//		}
		//
		//		internal void CalculateAllHeights() {
		//			for (int i = 0; i < TerrainGen.ChunksPerWorld; i++) {
		//				CalculateHeights (i);
		//			}
		//		}

		#region Calculations

		public Vector3 GetLighting(int x, int y) {
			if (x < 0 || x >= Lightings.GetLength (0) || y < 0 || y >= Lightings.GetLength (1))
				return Vector3.Zero;
			return Lightings [x, y];
		}

		public void OnTilePlace(int x, int y) {
			if (y > Heights [x]) {
				RemoveLight (x, Heights [x], SunRadius, SunStrength, SunColour);
				AddLight (x, y, SunRadius, SunStrength, SunColour);
				Heights [x] = y;
			}
		}

		public void OnTileRemove(int x, int y) {
			if (y == Heights [x]) {
				RemoveLight (x, y, SunRadius, SunStrength, SunColour);
				for (int j = y - 1; j >= 0; j--) {
					if (Terrain.TileAt (x, j).enumId != TileID.Air) {
						Heights [x] = j;
						AddLight (x, j, SunRadius, SunStrength, SunColour);
						break;
					}
				}
			}
		}

		public void AddLight(Vector2i pos, ILight light) => AddLight(pos.x, pos.y, light);

		public void AddLight(int x, int y, ILight light) => AddLight(x, y, light.Radius(), light.Strength(), light.Colour());

		private void AddLight(int x, int y, int radius, float strength, Vector3 colour) {
			radius = MathUtil.Clamp (radius, 0, MaxLightRadius);
			for (int i = -(radius - 1); i <= (radius - 1); i++) {
				for (int j = -(radius - 1); j <= (radius - 1); j++) {
					AddLighting (x + i, y + j, strength * lightingRegion.Regions [radius].GetLighting (i, j) * colour);
				}
			}
		}

		public void RemoveLight(Vector2i pos, ILight light) => RemoveLight (pos.x, pos.y, light);

		public void RemoveLight(int x, int y, ILight light) => RemoveLight(x, y, light.Radius(), light.Strength(), light.Colour());

		private void RemoveLight(int x, int y, int radius, float strength, Vector3 colour) {
			AddLight (x, y, radius, -strength, colour);
		}

		private static void AddLighting(int x, int y, Vector3 lighting) {
			if (x < 0 || x >= Lightings.GetLength (0) || y < 0 || y >= Lightings.GetLength (1))
				return;
			Lightings [x, y] += lighting;
		}



	



		/// <summary>
		/// Adds the light, and returns if it added successfully (i.e. there isn't already a light at the place to be added)
		/// </summary>
		/// <param name="v">The position of the light.</param>
		/// <param name="light">The light.</param>
		/// <returns></returns>
		//		public bool AddLight(Vector2i v, ILight light) {
		//			bool contains = dict.Keys.Contains (v);
		//			if (!contains) {
		//				dict.Add (v, light);
		//				AddExistingLight (v, light);
		//			}
		//			return contains;
		//		}
		//
		//		private void SetAveragedCache(int x, int y, ColourHSB colour) {
		//			if (!Terrain.WithinBounds (x, y))
		//				return;
		//			AveragedLightingCache [x, y] = colour;
		//		}
		//
		//		private void AddExistingLight(Vector2i v, ILight light) {
		//			ColourHSB colour = light.Colour ();
		//			int radius = light.Radius ();
		//			radius = MathUtil.Clamp (radius, 0, MaxLightRadius);
		//			for (int i = -(radius - 1); i <= (radius - 1); i++) {
		//				for (int j = -(radius - 1); j <= (radius - 1); j++) {
		//					float factor = lightingRegion.Regions [radius].GetLighting (i, j);
		//					AddLighting (v.x + i, v.y + j, colour.Hue, colour.Saturation, factor * colour.Brightness);
		//				}
		//			}
		//
		//			for (int i = -(radius - 1); i <= (radius - 1); i++) {
		//				for (int j = -(radius - 1); j <= (radius - 1); j++) {
		//					SetAveragedCache (v.x + i, v.y + j, CalculateAveragedLighting (v.x + i, v.y + j));
		//				}
		//			}
		//		}
		//
		//		private void AddLighting(int x, int y, float hue, float saturation, float brightness) {
		//			if (!Terrain.WithinBounds (x, y))
		//				return;
		//
		//			Brightnesses [x, y] += brightness;
		//
		//			Saturations [x, y] += saturation;
		//
		//			Hues [x, y].Add (hue);
		//			AveragedHues [x, y] = Hues [x, y].Average ();
		//		}
		//
		//		/// <summary>
		//		/// Removes the light, and returns whether it was removed successfully (i.e. there was a light to remove)
		//		/// </summary>
		//		/// <param name="v">Position of the light.</param>
		//		/// <param name="light">The light.</param>
		//		/// <returns></returns>
		//		public bool RemoveLight(Vector2i v, ILight light) {
		//			ColourHSB colour = light.Colour ();
		//			int radius = light.Radius ();
		//			bool removed = dict.Remove (v);
		//			radius = MathUtil.Clamp (radius, 0, MaxLightRadius);
		//			for (int i = -(radius - 1); i <= (radius - 1); i++) {
		//				for (int j = -(radius - 1); j <= (radius - 1); j++) {
		//					float factor = lightingRegion.Regions [radius].GetLighting (i, j);
		//					RemoveLighting (v.x + i, v.y + j, colour.Hue, colour.Saturation, factor * colour.Brightness);
		//				}
		//			}
		//
		//			for (int i = -(radius - 1); i <= (radius - 1); i++) {
		//				for (int j = -(radius - 1); j <= (radius - 1); j++) {
		//					SetAveragedCache (v.x + i, v.y + j, CalculateAveragedLighting (v.x + i, v.y + j));
		//				}
		//			}
		//			return removed;
		//		}
		//
		//		private void RemoveLighting(int x, int y, float hue, float saturation, float brightness) {
		//			if (!Terrain.WithinBounds (x, y))
		//				return;
		//
		//			Brightnesses [x, y] -= brightness;
		//
		//			Saturations [x, y] -= saturation;
		//
		//			Hues [x, y].Remove (hue);
		//			AveragedHues [x, y] = Hues [x, y].Count > 0 ? Hues [x, y].Average () : 0;
		//		}



		#endregion

		#region Updates

		internal Vector3[] CalcMesh() {
			AveragedAroundSquare_Cache = new Vector3?[Lightings.GetLength (0), Lightings.GetLength (1)];
			int startX, endX, startY, endY;
			Terrain.GetRange (out startX, out endX, out startY, out endY);
			List<Vector3> lightingsList = new List<Vector3> ();
			for (int i = startX; i <= endX; i++) {
				for (int j = startY; j <= endY; j++) {
					Tile t = Terrain.Tiles [i, j];
					if (t == null)
						continue;
					if (t.enumId != TileID.Air) {
						switch (GameLogic.LightingOption.Get ()) {
						case LightingOption.None:
							lightingsList.AddRange (new [] { Vector3.One, Vector3.One, Vector3.One, Vector3.One });
							break;
						case LightingOption.Jagged:
							lightingsList.AddRange (JaggedLighting (i, j));
							break;
						case LightingOption.Averaged:
							lightingsList.AddRange (AveragedLighting (i, j));
							break;
						case LightingOption.Smooth:
							lightingsList.AddRange (SmoothLighting (i, j));
							break;
						}
					}
				}
			}
			return lightingsList.ToArray ();
		}

		//top left, bottom left, bottom right, top right
		private  Vector3[] SmoothLighting(int i, int j) {
			return new Vector3[] {
				AverageAroundSquare (i - 1, j),
				AverageAroundSquare (i - 1, j - 1),
				AverageAroundSquare (i, j - 1),
				AverageAroundSquare (i, j)
			};
		}

		private  Vector3[] AveragedLighting(int i, int j) {
			Vector3 x = AverageAroundCross (i, j);
			return new Vector3[] { x, x, x, x };
		}
		//origin at bottom left
		private  Vector3 AverageAroundSquare(int i, int j) {
			if (i < 0 || i >= Lightings.GetLength (0) || j < 0 || j >= Lightings.GetLength (1))
				return Vector3.Zero;
			if (AveragedAroundSquare_Cache [i, j] != null) {
				return (Vector3)AveragedAroundSquare_Cache [i, j];
			}
			Vector3 avg = (GetLighting (i, j) + GetLighting (i + 1, j) + GetLighting (i, j + 1) + GetLighting (i + 1, j + 1)) / 4;
			AveragedAroundSquare_Cache [i, j] = avg;
			return avg;
		}

		private  Vector3 AverageAroundCross(int i, int j) => (GetLighting(i, j + 1) + GetLighting(i - 1, j) + GetLighting(i, j) + GetLighting(i + 1, j) + GetLighting(i, j - 1)) / 5;

		private  Vector3[] JaggedLighting(int i, int j) => new Vector3[] { Lightings[i, j], Lightings[i, j], Lightings[i, j], Lightings[i, j] };


		internal  Vector3[,] GetLightings() {
			return Lightings;
		}

		protected override void OnUpdate() {

		}


		#endregion
	}
}
