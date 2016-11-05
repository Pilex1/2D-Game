using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    class Model {

        public VAO vao;
        public BeginMode drawmode;
        public Vector2 size;

        private const int EntityTextureSize = 16;

        public Model(VAO vao, BeginMode drawmode, Vector2 size) {
            this.vao = vao;
            this.drawmode = drawmode;
            this.size = size;
        }

        public void DisposeAll() {
            vao.DisposeAll();
        }

        public static Model CreateRectangle(Vector2 size) {
            Vector2[] vertices = new Vector2[] { new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1) };
            int[] elements = new int[] { 0, 1, 2, 3, 0 };
            VAO vao = new VAO(vertices, elements);
            return new Model(vao, BeginMode.TriangleStrip, size);
        }
    }
}
