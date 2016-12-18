using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    static class Terrain {

        private const int size = 128;

        public static Model model { get; private set; }

        public static void Init(float[,] heights) {
            var vertices = new List<Vector3>();
            var elements = new List<int>();
            var colours = new List<Color4>();
            for (int i = 0; i < heights.GetLength(0); i++) {
                for (int j = 0; j < heights.GetLength(1); j++) {
                    float x = i - (float)heights.GetLength(0) / 2;
                    float y = j - (float)heights.GetLength(1) / 2;
                    float z = heights[i, j];
                    vertices.Add(new Vector3(x, y, z));
                    colours.Add(new Color4((float)Math.Sin(i)/2, 0.2f, 0.5f, 1));

                }
            }
            for (int i = 0; i < heights.GetLength(0) - 1; i++) {
                for (int j = 0; j < heights.GetLength(1) - 1; j++) {
                    int topleft = i * heights.GetLength(1) + j;
                    int topright = topleft + 1;
                    int bottomleft = (i + 1) * heights.GetLength(1) + j;
                    int bottomright = bottomleft + 1;
                    elements.AddRange(new int[] { topleft, bottomleft, topright, topright, bottomleft, bottomright });
                }
            }
            VAO vao = new VAO(vertices.ToArray(), colours.ToArray(), elements.ToArray());
            model = new Model(vao, BeginMode.Triangles);
        }

        public static float[,] GenerateHeights() {
            var heights = new float[size, size];
            for (int i = 0; i < heights.GetLength(0); i++) {
                for (int j = 0; j < heights.GetLength(1); j++) {
                    heights[i, j] = 10 * (float)Math.Sin(0.005f * i * j);
                }
            }
            return heights;
        }
    }
}
