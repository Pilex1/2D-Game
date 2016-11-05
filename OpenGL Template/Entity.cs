using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    class Grid : Entity {

        private int x, y;
        private Vector4[,] colours;

        public Grid(int x, int y, Vector2 pos, float cellsize) : base(InitModel(x, y, cellsize), pos) {
            this.x = x;
            this.y = y;
            colours = new Vector4[x, y];
        }

        private static Model InitModel(int x, int y, float cellsize) {
            Vector2[] vertices = new Vector2[4 * x * y];
            int ptr = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    float xpos = i * cellsize - x * cellsize / 2;
                    float ypos = j * cellsize - y * cellsize / 2;
                    vertices[ptr++] = new Vector2(xpos - cellsize / 2, ypos + cellsize / 2);
                    vertices[ptr++] = new Vector2(xpos - cellsize / 2, ypos - cellsize / 2);
                    vertices[ptr++] = new Vector2(xpos + cellsize / 2, ypos - cellsize / 2);
                    vertices[ptr++] = new Vector2(xpos + cellsize / 2, ypos + cellsize / 2);
                }
            }

            int[] elements = new int[6 * x * y];
            ptr = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    float xpos = i * cellsize - x * cellsize / 2;
                    float ypos = j * cellsize - y * cellsize / 2;
                    elements[ptr++] = 0;
                    elements[ptr++] = 1;
                    elements[ptr++] = 2;
                    elements[ptr++] = 2;
                    elements[ptr++] = 0;
                    elements[ptr++] = 3;
                }
            }

            Vector4[] colours = new Vector4[4 * x * y];
            ptr = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    colours[ptr++] = colours[ptr++] = colours[ptr++] = colours[ptr++] = new Vector4((float)i / x, 0, 0, 1);
                }
            }

            VAO vao = new VAO(vertices, colours, elements);
            return new Model(vao, BeginMode.Triangles, new Vector2(1, 1));
        }

        public void SetColour(int x, int y, Vector4 colour) {
            colours[x, y] = colour;
        }

        public void Update() {
            model.vao.UpdateColourData(GenColours());
        }

        private Vector4[] GenColours() {
            Vector4[] colours = new Vector4[4 * x * y];
            int ptr = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    colours[ptr++] = colours[ptr++] = colours[ptr++] = colours[ptr++] = this.colours[i, j];
                }
            }
            return colours;
        }
    }
    class Entity {
        public Model model { get; private set; }
        public Vector2 pos;

        public Entity(Model model, Vector2 pos) {
            this.model = model;
            this.pos = pos;
        }

    }
}
