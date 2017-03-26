using System;

namespace Game.Terrains.Lighting {

    class LightingRegion {
        internal  SubLightingRegion[] Regions;

        public LightingRegion() {
            Regions = new SubLightingRegion[LightingManager.MaxLightRadius + 1];
            for (int i = 1; i < Regions.Length; i++) {
                Regions[i] = new SubLightingRegion(i);
            }
        }

        internal class SubLightingRegion {

            private int radius;
            //quadrant 1
            private float[,] lighting;

            internal SubLightingRegion(int radius) {
                this.radius = radius;
                lighting = new float[this.radius, this.radius];
                CalcLighting();
            }

            private void CalcLighting() {
                if (radius == 0) return;
                for (int i = 0; i < radius; i++) {
                    for (int j = 0; j < radius; j++) {
                        lighting[i, j] = Math.Max(0, 1 - (float)Math.Sqrt(i * i + j * j) / radius);
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

   
}
