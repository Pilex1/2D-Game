using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Runtime.InteropServices;
using Game.TitleScreen;
using Game.Terrains;

namespace Game.Core {

    class EntityVAO : IDisposable {
        public uint ID { get; }

        public uint verticesID { get; }
        public uint uvsID { get; }
        public uint elementsID { get; }

        public int count { get; }

        public EntityVAO(Vector2[] vertices, int[] elements, Vector2[] uvs) {

            ShaderProgram program = Entity.shader;
            if (program == null) throw new ArgumentNullException("Entity shader is null");

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

        public void Dispose() {
            Gl.DeleteBuffer(verticesID);
            Gl.DeleteBuffer(uvsID);
            Gl.DeleteBuffer(elementsID);
            Gl.DeleteVertexArrays(1, new uint[] { ID });
        }

        //top left, bottom left, bottom right, top right
        public static EntityVAO CreateRectangle(Vector2 size) {
            Vector2[] vertices = new Vector2[] {
                new Vector2(0,size.y),
                new Vector2(0,0),
                new Vector2(size.x,0),
                new Vector2(size.x,size.y)
            };
            int[] elements = new int[] {
                0,1,2,2,3,0
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1)
            };
            return new EntityVAO(vertices, elements, uvs);
        }


    }

    class TerrainVAO : IDisposable {

        public uint ID { get;}

        public uint verticesID { get; private set; }
        public uint uvsID { get; private set; }
        public uint elementsID { get; private set; }
        public uint lightingsID { get; private set; }

        public int count { get; private set; }

        public TerrainVAO(Vector2[] vertices, int[] elements, Vector2[] uvs, float[] lightings) {

            ShaderProgram shader = Terrain.TerrainShader;
            if (shader == null)
                throw new ArgumentException("Terrain shader null");

            count = elements.Length;

            this.ID = Gl.GenVertexArray();
            Gl.BindVertexArray(ID);

            {
                this.verticesID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StreamDraw);
                uint vertexAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vertexPosition");
                Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(vertexAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.uvsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, BufferUsageHint.StreamDraw);
                uint uvAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vertexUV");
                Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(uvAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.lightingsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, lightingsID);
                Gl.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * lightings.Length, lightings, BufferUsageHint.StreamDraw);
                uint lightingsAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vertexLighting");
                Gl.VertexAttribPointer(lightingsAttribLocation, 1, VertexAttribPointerType.Float, true, sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(lightingsAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.elementsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StreamDraw);
                //don't unbind the element buffer object!
            }

            Gl.BindVertexArray(0);
        }

        public void UpdateData(Vector2[] vertices, int[] elements, Vector2[] uvs, float[] lightings) {
            Gl.BindVertexArray(ID);

            ShaderProgram shader = Terrain.TerrainShader;
            if (shader == null)
                throw new ArgumentException("Terrain shader null");

            Gl.DeleteBuffer(verticesID);
            this.verticesID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, BufferUsageHint.StreamDraw);
            uint vertexAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vertexPosition");
            Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(vertexAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(uvsID);
            this.uvsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
            Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, BufferUsageHint.StreamDraw);
            uint uvAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vertexUV");
            Gl.VertexAttribPointer(uvAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(uvAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(lightingsID);
            this.lightingsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, lightingsID);
            Gl.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * lightings.Length, lightings, BufferUsageHint.StreamDraw);
            uint lightingsAttribLocation = (uint)Gl.GetAttribLocation(shader.ProgramID, "vertexLighting");
            Gl.VertexAttribPointer(lightingsAttribLocation, 1, VertexAttribPointerType.Float, true, sizeof(float), IntPtr.Zero);
            Gl.EnableVertexAttribArray(lightingsAttribLocation);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Gl.DeleteBuffer(elementsID);
            this.elementsID = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, elementsID);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StreamDraw);
            count = elements.Length;

            Gl.BindVertexArray(0);
        }

        public void Dispose() {
            Gl.DeleteBuffer(verticesID);
            Gl.DeleteBuffer(uvsID);
            Gl.DeleteBuffer(elementsID);
            Gl.DeleteBuffer(lightingsID);
            Gl.DeleteVertexArrays(1, new uint[] { ID });
        }
    }

    class GuiVAO : IDisposable {
        public uint ID { get; }

        public uint verticesID { get; }
        public uint uvsID { get; }
        public uint elementsID { get; }

        public int count { get; }

        public GuiVAO(Vector2[] vertices, int[] elements, Vector2[] uvs, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {

            ShaderProgram program = Gui.shader;
            if (program == null) throw new ArgumentNullException("Gui shader null");

            count = elements.Length;

            this.ID = Gl.GenVertexArray();
            Gl.BindVertexArray(ID);

            {
                this.verticesID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, verticesID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * vertices.Length, vertices, verticeshint);
                uint vertexAttribLocation = (uint)Gl.GetAttribLocation(program.ProgramID, "vpos");
                Gl.VertexAttribPointer(vertexAttribLocation, 2, VertexAttribPointerType.Float, true, 2 * sizeof(float), IntPtr.Zero);
                Gl.EnableVertexAttribArray(vertexAttribLocation);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                this.uvsID = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, uvsID);
                Gl.BufferData(BufferTarget.ArrayBuffer, 2 * sizeof(float) * uvs.Length, uvs, uvhint);
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

        public void Dispose() {
            Gl.DeleteBuffer(verticesID);
            Gl.DeleteBuffer(uvsID);
            Gl.DeleteBuffer(elementsID);
            Gl.DeleteVertexArrays(1, new uint[] { ID });
        }

        public static GuiVAO CreateWireRectangle(Vector2 size, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,size.y),
                new Vector2(size.x,size.y),
                new Vector2(size.x,0)
            };
            int[] elements = new int[] {
               0,1,2,3,0
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1)
            };
            return new GuiVAO(vertices, elements, uvs,verticeshint,uvhint);
        }

        public static GuiVAO CreateRectangle(Vector2 size, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,size.y),
                new Vector2(size.x,size.y),
                new Vector2(size.x,0)
            };
            int[] elements = new int[] {
                0,1,2,2,3,0
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1)
            };
            return new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
        }
    }
}
