using System;
using OpenGL;
using System.Runtime.InteropServices;
using Game.TitleScreen;
using Game.Terrains;
using System.Diagnostics;
using Game.Entities;

namespace Game.Core {

    class EntityVAO {
        public uint ID { get; private set; }

        public uint verticesID { get; private set; }
        public uint uvsID { get; private set; }
        public uint elementsID { get; private set; }

        public int count { get; private set; }

        public EntityVAO(Vector2[] vertices, int[] elements, Vector2[] uvs) {

            ShaderProgram program = EntityManager.shader;
            Debug.Assert(program != null);

            count = elements.Length;

            this.ID = Gl.GenVertexArray();
            Gl.BindVertexArray(ID);

            {
                this.verticesID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
                uint vertexAttribLocation = (uint)Gl.GetAttribLocation(program.ProgramID, "vpos");
                Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(vertexAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.uvsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, BufferUsageHint.StaticDraw);
                uint uvAttribLocation = (uint)Gl.GetAttribLocation(program.ProgramID, "vuv");
                Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(uvAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.elementsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StaticDraw);
                //don't unbind the element buffer object!
            }

            Gl.BindVertexArray(0);
        }

        public void DisposeAll() {
            Gl.DeleteBuffer(verticesID);
            Gl.DeleteBuffer(uvsID);
            Gl.DeleteBuffer(elementsID);
            Gl.DeleteVertexArrays(1, new uint[] { ID });
        }
    }

    class TerrainVAO {

        public uint ID { get; private set; }

        public uint verticesID { get; private set; }
        public uint uvsID { get; private set; }
        public uint elementsID { get; private set; }
        public uint lightingsID { get; private set; }

        public int count { get; private set; }

        public TerrainVAO(Vector2[] vertices, int[] elements, Vector2[] uvs, Vector3[] lightings) {
            count = elements.Length;
            this.ID = Gl.GenVertexArray();
            UpdateData(vertices, elements, uvs, lightings);
        }

        public void UpdateData(Vector2[] vertices, int[] elements, Vector2[] uvs, Vector3[] lightings) {
            Gl.BindVertexArray(ID);
            ShaderProgram shader = Terrain.Shader;

            Gl.DeleteBuffer(verticesID);
            this.verticesID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StreamDraw);
            uint vertexAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vert_pos");
            Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(vertexAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(uvsID);
            this.uvsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, BufferUsageHint.StreamDraw);
            uint uvAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vert_uv");
            Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(uvAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(lightingsID);
            this.lightingsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, lightingsID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 3 * sizeof(float) * lightings.Length, lightings, BufferUsageHint.StreamDraw);
            uint lightingsAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vert_lighting");
            Gl.VertexAttribPointer(lightingsAttribLocation, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(lightingsAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(elementsID);
            this.elementsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StreamDraw);
            count = elements.Length;

            Gl.BindVertexArray(0);
        }

        public void DisposeAll() {
            Gl.DeleteBuffer(verticesID);
            Gl.DeleteBuffer(uvsID);
            Gl.DeleteBuffer(elementsID);
            Gl.DeleteBuffer(lightingsID);
            Gl.DeleteVertexArrays(1, new uint[] { ID });
        }
    }

    class GuiVAO {
        public uint ID { get; private set; }

        public uint verticesID { get; private set; }
        public uint uvsID { get; private set; }
        public uint elementsID { get; private set; }

        public int count { get; private set; }

        public GuiVAO(Vector2[] vertices, int[] elements, Vector2[] uvs, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {

            ShaderProgram shader = Gui.shader;
            Debug.Assert(shader != null);

            count = elements.Length;

            this.ID = Gl.GenVertexArray();
            Gl.BindVertexArray(ID);

            {
                this.verticesID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, verticeshint);
                uint vertexAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vpos");
                Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(vertexAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.uvsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, uvhint);
                uint uvAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vuv");
                Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(uvAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.elementsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StaticDraw);
                //don't unbind the element buffer object!
            }

            Gl.BindVertexArray(0);
        }

        public void UpdateAll(Vector2[] vertices, int[] elements, Vector2[] uvs) {
            Gl.BindVertexArray(ID);

            ShaderProgram shader = Gui.shader;
            Debug.Assert(shader != null);

            Gl.DeleteBuffer(verticesID);
            this.verticesID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StreamDraw);
            uint vertexAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vpos");
            Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(vertexAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(uvsID);
            this.uvsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, BufferUsageHint.StreamDraw);
            uint uvAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vuv");
            Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(uvAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(elementsID);
            this.elementsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StreamDraw);
            count = elements.Length;

            Gl.BindVertexArray(0);
        }

        public void UpdateVertices(Vector2[] vertices) {
            Gl.BindVertexArray(ID);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
            GCHandle data_ptr = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            try {
                Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(2 * sizeof(float) * vertices.Length), data_ptr.AddrOfPinnedObject());
            } finally {
                data_ptr.Free();
            }
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        public void UpdateUVs(Vector2[] uvs) {
            Gl.BindVertexArray(ID);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
            GCHandle data_ptr = GCHandle.Alloc(uvs, GCHandleType.Pinned);
            try {
                Gl.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(2 * sizeof(float) * uvs.Length), data_ptr.AddrOfPinnedObject());
            } finally {
                data_ptr.Free();
            }
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        public void DisposeAll() {
            Gl.DeleteBuffer(verticesID);
            Gl.DeleteBuffer(uvsID);
            Gl.DeleteBuffer(elementsID);
            Gl.DeleteVertexArrays(1, new uint[] { ID });
        }
    }
}
