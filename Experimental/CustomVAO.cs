using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Experimental {

    class CustomVAO {

        public uint vaoID { get; }

        public uint vboverticesID { get; }
        public uint eboID { get; }

        public int count { get; }

        public CustomVAO(ShaderProgram program, Vector3[] vertices, string vertname, int[] elements) {

            count = elements.Length;

            this.vaoID = Gl.GenVertexArray();
            Gl.BindVertexArray(vaoID);

            {
                this.vboverticesID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, vboverticesID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 3 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
                uint vertexAttribLocation = (uint)Gl.GetAttribLocation(program.ProgramID, vertname);
                Gl.VertexAttribPointer(vertexAttribLocation, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(vertexAttribLocation);

                this.eboID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StaticDraw);
                //don't unbind the element buffer object!
            }

            Gl.BindVertexArray(0);
        }
    }
}
