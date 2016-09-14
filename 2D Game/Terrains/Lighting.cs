using System;
using Game.Assets;
using Game.Util;

namespace Game.Terrains {
    class Lighting {
       
        internal static void CalculateLighting() {

            int posX, posY;
            if (Player.Instance != null) {
                posX = (int)Player.Instance.Position.x;
                posY = (int)Player.Instance.Position.y;
            } else {
                posX = Player.StartX;
                posY = Player.StartY;
            }

            int min = (int)(posY + Renderer.zoom / 2 / Program.AspectRatio) - Light.MaxLightLevel;
            int max = (int)(posY - Renderer.zoom / 2 / Program.AspectRatio) + Light.MaxLightLevel;
            Terrain.Lightings = new int[Terrain.Tiles.GetLength(0), Terrain.Tiles.GetLength(1)];

            //calculate lightings for each tile
            //sun lighting
            for (int i = (int)(posX + Renderer.zoom / 2)-Light.MaxLightLevel; i <=(int)(posX - Renderer.zoom / 2)+Light.MaxLightLevel; i++) {

                int top, bottom;
                LightingRange(i, out top, out bottom);

                MathUtil.ClampMin(ref bottom, min);
                MathUtil.ClampMax(ref top, max);

                for (int j = bottom; j <= top; j++) {
                    SpreadLighting(i, j, Light.MaxLightLevel);
                }
            }

            //artificial lighting
            foreach (Light l in Terrain.Lights) {
                SpreadLighting(l.x, l.y, l.LightLevel);
            }
        }

        private static void LightingRange(int cx, out int top, out int bottom) {
            int outTop = 0, outBottom = 0;
            for (int j = Terrain.Tiles.GetLength(1) - 1; j >= 0; j--) {
                if (Terrain.TileAt(cx - 1, j) != Tile.Air || Terrain.TileAt(cx + 1, j) != Tile.Air || Terrain.TileAt(cx, j - 1) != Tile.Air) {
                    outTop = j;
                    break;
                }
            }
            for (int j = outTop; j >= 1; j--) {
                if (Terrain.TileAt(cx, j - 1) != Tile.Air) {
                    outBottom = j;
                    break;
                }
            }
            top = outTop;
            bottom = outBottom;
        }

        private static void SpreadLighting(int x, int y, int strength) {
            for (int i = 0; i < strength; i++) {
                DiamondLighting(x, y, i, strength - i);
            }
        }

        private static void DiamondLighting(int x, int y, int radius, int strength) {
            int i = 0;
            for (int j = -radius; j <= radius; j++) {

                SetLighting(x - i, y + j, strength);
                SetLighting(x + i, y + j, strength);

                if (j < 0) i++;
                else i--;
            }
        }

        private static void SetLighting(int x, int y, int lighting) {
            if (x < 0 || x >= Terrain.Lightings.GetLength(0) || y < 0 || y >= Terrain.Lightings.GetLength(1)) return;
            if (lighting > Terrain.Lightings[x, y]) Terrain.Lightings[x, y] = lighting;
        }

    }
}
