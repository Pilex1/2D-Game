using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Game.Core {
    class CustomVAO {

        public uint ID { get; }

        public uint verticesID { get; }
        public uint uvsID { get; }
        public uint elementsID { get; }

        public int count { get; }

        public CustomVAO(ShaderProgram program, Vector2[] vertices, string vertname, int[] elements, Vector2[] uvs, string uvname) {

            count = elements.Length;

            this.ID = Gl.GenVertexArray();
            Gl.BindVertexArray(ID);

            {
                this.verticesID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
                uint vertexAttribLocation = (uint)Gl.GetAttribLocation(program.ProgramID, vertname);
                Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(vertexAttribLocation);

                this.uvsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, BufferUsageHint.StaticDraw);
                uint uvAttribLocation = (uint)Gl.GetAttribLocation(program.ProgramID, uvname);
                Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(uvAttribLocation);

                this.elementsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StaticDraw);
                //don't unbind the element buffer object!
            }

            Gl.BindVertexArray(0);
        }
    }
}
