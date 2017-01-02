using Pencil.Gaming.Graphics;
using System;

namespace Game.Main.GLConstructs {
    class EBO : IDisposable {

        public int ID { get; private set; }

        private ShaderProgram shader;

        private BufferUsageHint hint;
        public int Count { get; private set; }

        public EBO(ShaderProgram shader, int[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw) {
            this.shader = shader;
            this.hint = hint;
            UpdateData(data);
            ResourceManager.Resources.Add(this);
        }

        public void UpdateData(int[] data) {
            GL.DeleteBuffer(ID);
            ID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(int) * data.Length), data, hint);
            Count = data.Length;
        }

        public void Dispose() {
            GL.DeleteBuffer(ID);
        }

    }
}
