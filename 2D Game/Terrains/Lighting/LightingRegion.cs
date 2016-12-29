using System;
using System.Collections.Generic;

namespace Game.Terrains.Lighting {
    class LightingRegion {

        internal static LightingRegion[] Regions;

        public static void Init() {
            Regions = new LightingRegion[LightingManager.MaxLightRadius + 1];
            for (int i = 0; i < Regions.Length; i++) {
                Regions[i] = new LightingRegion(i);
            }
        }

        private int radius;
        //quadrant 1
        private float[,] lighting;

        private LightingRegion(int radius) {
            this.radius = radius+1;
            lighting = new float[this.radius, this.radius];
            CalcLighting();
        }

        private void CalcLighting() {
            float radius_sq = radius * radius;
            for (int i = 0; i < radius; i++) {
                for (int j = 0; j < radius; j++) {
                    float quadrance = i * i + j * j;
                    lighting[i, j] = 1 - quadrance / radius_sq;
                }
            }
        }

        public float GetLighting(int x, int y) {
            if (x <= -radius || x >= radius || y <= -radius || y >= radius) throw new ArgumentException("Out of range: " + x + "," + y);
            x = Math.Abs(x);
            y = Math.Abs(y);
            return lighting[x, y];
        }
    }
}
