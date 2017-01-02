using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Main.GLConstructs {

    abstract class VBO : IDisposable {
        public abstract void Dispose();
    }

    class VBO<T> : VBO where T : struct {

        public int ID { get; private set; }

        private ShaderProgram shader;
        private string attributeName;

        private BufferUsageHint hint;
        private VertexAttribPointerType type;
        private int elements;
        private int size;

        public VBO(ShaderProgram shader, string attributeName, T[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) {
            this.shader = shader;
            this.attributeName = attributeName;
            this.hint = hint;

            if (data is int[]) {
                type = VertexAttribPointerType.Int;
                elements = 1;
            } else if (data is float[]) {
                type = VertexAttribPointerType.Float;
                elements = 1;
            } else if (data is Vector2i[]) {
                type = VertexAttribPointerType.Int;
                elements = 2;
            } else if (data is Vector2[]) {
                type = VertexAttribPointerType.Float;
                elements = 2;
            } else if (data is Vector3i[]) {
                type = VertexAttribPointerType.Int;
                elements = 3;
            } else if (data is Vector3[]) {
                type = VertexAttribPointerType.Float;
                elements = 3;
            } else if (data is Vector4i[]) {
                type = VertexAttribPointerType.Int;
                elements = 4;
            } else if (data is Vector4[] || data is Color4[]) {
                type = VertexAttribPointerType.Float;
                elements = 4;
            } else {
                throw new ArgumentException("Invalid type: " + data.GetType());
            }
            size = (type == VertexAttribPointerType.Int ? sizeof(int) : sizeof(float)) * elements;
            UpdateData(data);
            ResourceManager.Resources.Add(this);
        }

        public void UpdateData(T[] data) {
            GL.DeleteBuffer(ID);
            ID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(size * data.Length), data, hint);
            uint location = (uint)GL.GetAttribLocation(shader.ID, attributeName);
            GL.VertexAttribPointer(location, elements, VertexAttribPointerType.Float, true, size, IntPtr.Zero);
            GL.EnableVertexAttribArray(location);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public override void Dispose() {
            GL.DeleteBuffer(ID);
        }

    }
}
