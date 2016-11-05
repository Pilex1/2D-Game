using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    class VAO {
        public int ID { get; private set; }

        public int verticesID { get; private set; }
        public int coloursID { get; private set; }
        public int elementsID { get; private set; }

        public int count { get; private set; }

        public VAO(Vector2[] vertices, Vector4[] colours, int[] elements) {

            ShaderProgram program = Program.shader;
            Debug.Assert(program != null);

            count = elements.Length;

            this.ID = GL.GenVertexArray();
            GL.BindVertexArray(ID);

            {
                this.verticesID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(2 * sizeof(float) * vertices.Length), vertices, BufferUsageHint.StaticDraw);
                uint vertexAttribLocation = (uint)GL.GetAttribLocation(program.id, "vpos");
                GL.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                GL.EnableVertexAttribArray(vertexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.coloursID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, coloursID);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(4 * sizeof(float) * colours.Length), colours, BufferUsageHint.StaticDraw);
                uint colourAttribLocation = (uint)GL.GetAttribLocation(program.id, "vcolour");
                GL.VertexAttribPointer(colourAttribLocation, 4, VertexAttribPointerType.Float, true, 4 * sizeof(float), IntPtr.Zero);
                GL.EnableVertexAttribArray(colourAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.elementsID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(int) * elements.Length), elements, BufferUsageHint.StaticDraw);
            }

            GL.BindVertexArray(0);
        }

        public void DisposeAll() {
            GL.DeleteBuffer(verticesID);
            GL.DeleteBuffer(coloursID);
            GL.DeleteBuffer(elementsID);
            GL.DeleteVertexArrays(1, new int[] { ID });
        }
    }
}
