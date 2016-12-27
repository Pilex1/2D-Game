using Game.Util;
using System.Collections.Generic;
using Game.Core;

namespace Game.Terrains {

    static class Lighting {

        public static float SunStrength = 1f;

        internal const int LightRadius = 12;

        internal static float[,] Lightings;
        internal static Dictionary<Vector2i, int> ArtificialLight = new Dictionary<Vector2i, int>();

        private const int MaxUpdatesPerFrame = 3;
        private static HashSet<Vector2i> QueuedUpdates = new HashSet<Vector2i>();

        internal static void Init() {
            Lightings = new float[Terrain.Tiles.GetLength(0), Terrain.Tiles.GetLength(1)];
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                SunLighting(i);
            }

            //artificial lighting
            foreach (Vector2i v in ArtificialLight.Keys) {
                SpreadLighting(v.x, v.y, ArtificialLight[v]);
            }
        }

        private static void SunLighting(int x) {
            if (x < 0) x = 0;
            if (x >= Terrain.Tiles.GetLength(0)) x = Terrain.Tiles.GetLength(0) - 1;
            for (int j = Terrain.Tiles.GetLength(1) - 1; j >= Terrain.Heights[x]; j--) {
                if (!Terrain.TileAt(x - 1, j).tileattribs.transparent || !Terrain.TileAt(x + 1, j).tileattribs.transparent || j == Terrain.Heights[x] + 1) {
                    SpreadLighting(x, j, LightRadius);
                }
            }
        }

        internal static void RecalcAll() {
            float x = Player.Instance.data.pos.x, y = Player.Instance.data.pos.y;

            int startX = (int)(x + GameRenderer.zoom / 2 - LightRadius), endX = (int)(x - GameRenderer.zoom / 2 + LightRadius);
            int startY = (int)((y + GameRenderer.zoom / 2 - LightRadius) / Program.AspectRatio), endY = (int)((y - GameRenderer.zoom / 2 + LightRadius) / Program.AspectRatio);
            MathUtil.Clamp(ref startX, 0, Terrain.Tiles.GetLength(0) - 1);
            MathUtil.Clamp(ref startY, 0, Terrain.Tiles.GetLength(1) - 1);
            MathUtil.Clamp(ref endX, 0, Terrain.Tiles.GetLength(0) - 1);
            MathUtil.Clamp(ref endY, 0, Terrain.Tiles.GetLength(1) - 1);
            List<float> lightingsList = new List<float>();
            for (int i = startX; i <= endX; i++) {
                SunLighting(i);
            }
            foreach (var v in ArtificialLight.Keys) {
                if (v.x >= x - LightRadius * 2 && v.x <= x + LightRadius * 2 && v.y >= y - LightRadius * 2 && y <= y - LightRadius * 2) {
                    SpreadLighting(v.x, v.y, ArtificialLight[v]);
                }
            }
        }

        private static void UpdateAround(int x, int y) {
            for (int i = -LightRadius; i <= LightRadius; i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++)
                    SetLighting(x + i, j, 0, false);
            }

            for (int i = -LightRadius * 2; i <= LightRadius * 2; i++) {
                SunLighting(x + i);
            }
            foreach (var v in ArtificialLight.Keys) {
                if (v.x >= x - LightRadius * 2 && v.x <= x + LightRadius * 2 && v.y >= y - LightRadius * 2 && y <= y - LightRadius * 2) {
                    SpreadLighting(v.x, v.y, ArtificialLight[v]);
                }
            }
        }

        private static void SpreadLighting(int x, int y, int radius) {
            int radiusSq = radius * radius;
            for (int i = -radius; i <= radius; i++) {
                for (int j = -radius; j <= radius; j++) {
                    if (i * i + j * j <= radiusSq) {
                        float distSq = i * i + j * j;
                        SetLighting(x + i, y + j, SunStrength * radius * (radiusSq - distSq) / radiusSq, true);
                    }
                }
            }
        }

        private static void SetLighting(int x, int y, float lighting, bool over) {
            if (x < 0 || x >= Lightings.GetLength(0) || y < 0 || y >= Lightings.GetLength(1)) return;
            if (!over || (lighting > Lightings[x, y])) Lightings[x, y] = lighting;
        }

        public static void AddLight(int x, int y, int strength) {
            ArtificialLight[new Vector2i(x, y)] = strength;
            UpdateAround(x, y);
        }

        public static void RemoveLight(int x, int y) {
            if (ArtificialLight.Remove(new Vector2i(x, y))) {
                UpdateAround(x, y);
            }
        }

        internal static void QueueUpdate(int x, int y) {
            QueuedUpdates.Add(new Vector2i(x, y));
        }

        internal static float[] CalcMesh() {

            if (QueuedUpdates.Count > MaxUpdatesPerFrame) {
                RecalcAll();
            } else {
                foreach (Vector2i v in QueuedUpdates) {
                    UpdateAround(v.x, v.y);
                }
            }
            QueuedUpdates.Clear();

            float posX = (int)Player.Instance.data.pos.x;
            float posY = (int)Player.Instance.data.pos.y;

            int startX = (int)(posX + GameRenderer.zoom / 2), endX = (int)(posX - GameRenderer.zoom / 2);
            int startY = (int)(posY + GameRenderer.zoom / 2 / Program.AspectRatio), endY = (int)(posY - GameRenderer.zoom / 2 / Program.AspectRatio);
            MathUtil.Clamp(ref startX, 0, Terrain.Tiles.GetLength(0) - 1);
            MathUtil.Clamp(ref startY, 0, Terrain.Tiles.GetLength(1) - 1);
            MathUtil.Clamp(ref endX, 0, Terrain.Tiles.GetLength(0) - 1);
            MathUtil.Clamp(ref endY, 0, Terrain.Tiles.GetLength(1) - 1);
            List<float> lightingsList = new List<float>();
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    if (Terrain.Tiles[i, j].enumId != TileID.Air) {
                        float val = Lightings[i, j] / LightRadius;
                        lightingsList.AddRange(new float[] {
                            val,val,val,val
                        });
                    }
                }
            }
            return lightingsList.ToArray();
        }
    }
}
