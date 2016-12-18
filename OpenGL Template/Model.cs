using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    class Model {

        public VAO vao;
        public BeginMode drawmode;

        private const int EntityTextureSize = 16;

        public Model(VAO vao, BeginMode drawmode) {
            this.vao = vao;
            this.drawmode = drawmode;
        }

        public void DisposeAll() {
            vao.DisposeAll();
        }

        public static Model CreateRectangle(Color4[] colours) {
            Debug.Assert(colours.Length == 4);
            Vector3[] vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) };
            int[] elements = new int[] { 0, 1, 2, 2, 3, 0 };
            VAO vao = new VAO(vertices, colours, elements);
            return new Model(vao, BeginMode.Triangles);
        }

        public static Model CreateCube(Color4[] colours) {
            Debug.Assert(colours.Length == 8);
            Vector3[] vertices = new Vector3[] {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3(  1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f,  1.0f,  1.0f),
                new Vector3( -1.0f,  1.0f,  1.0f),

                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(  1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f,  1.0f, -1.0f),
                new Vector3( -1.0f,  1.0f, -1.0f)
            };
            int[] elements = new int[] {
                0, 1, 2,
                2, 3, 0,

                1, 5, 6,
                6, 2, 1,

                7, 6, 5,
                5, 4, 7,

                4, 0, 3,
                3, 7, 4,

                4, 5, 1,
                1, 0, 4,

                3, 2, 6,
                6, 7, 3
            };
            VAO vao = new VAO(vertices, colours, elements);
            return new Model(vao, BeginMode.Triangles);
        }
        
    }
}
