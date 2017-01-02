using System;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using Game.TitleScreen;
using Game.Terrains;
using Game.Entities;

namespace Game.Main.GLConstructs {

    abstract class VAO : IDisposable {

        public int ID { get; private set; }

        public EBO Elements;

        private ShaderProgram Shader;

        public VAO(ShaderProgram shader, int[] elements) {
            Shader = shader;
            ID = GL.GenVertexArray();
            GL.BindVertexArray(ID);
            Elements = new EBO(shader, elements);
            GL.BindVertexArray(0);
            ResourceManager.Resources.Add(this);
        }

        public void Dispose() {
            GL.DeleteVertexArray(ID);
        }

    }

    class EntityVAO : VAO {

        private VBO<Vector2> Vertices;
        private VBO<Vector2> UVs;

        public EntityVAO(Vector2[] vertices, int[] elements, Vector2[] uvs) : base(EntityManager.Shader, elements) {
            GL.BindVertexArray(ID);
            Vertices = new VBO<Vector2>(EntityManager.Shader, "vpos", vertices);
            UVs = new VBO<Vector2>(EntityManager.Shader, "vuv", uvs);
            GL.BindVertexArray(0);
        }

    }

    class TerrainVAO : VAO {

        private VBO<Vector2> Vertices;
        private VBO<Vector2> UVs;
        private VBO<Vector3> Lightings;

        public TerrainVAO(Vector2[] vertices, int[] elements, Vector2[] uvs, Vector3[] lightings) : base(Terrain.Shader, elements) {
            GL.BindVertexArray(ID);
            Vertices = new VBO<Vector2>(Terrain.Shader, "vert_pos", vertices, BufferUsageHint.StreamDraw);
            UVs = new VBO<Vector2>(Terrain.Shader, "vert_uv", uvs, BufferUsageHint.StreamDraw);
            Lightings = new VBO<Vector3>(Terrain.Shader, "vert_lighting", lightings, BufferUsageHint.StreamDraw);
            GL.BindVertexArray(0);
        }

        public void UpdateData(Vector2[] vertices, int[] elements, Vector2[] uvs, Vector3[] lightings) {
            GL.BindVertexArray(ID);
            Vertices.UpdateData(vertices);
            Elements.UpdateData(elements);
            UVs.UpdateData(uvs);
            Lightings.UpdateData(lightings);
            GL.BindVertexArray(0);
        }
    }

    class GuiVAO : VAO {

        private VBO<Vector2> Vertices;
        private VBO<Vector2> UVs;

        public GuiVAO(Vector2[] vertices, int[] elements, Vector2[] uvs) : base(Gui.shader, elements) {
            GL.BindVertexArray(ID);
            Vertices = new VBO<Vector2>(Gui.shader, "vpos", vertices);
            UVs = new VBO<Vector2>(Gui.shader, "vuv", uvs);
            GL.BindVertexArray(0);
        }

        public void UpdateAll(Vector2[] vertices, int[] elements, Vector2[] uvs) {
            GL.BindVertexArray(ID);
            Vertices.UpdateData(vertices);
            Elements.UpdateData(elements);
            UVs.UpdateData(uvs);
            GL.BindVertexArray(0);
        }

        public void UpdateVertices(Vector2[] vertices) {
            GL.BindVertexArray(ID);
            Vertices.UpdateData(vertices);
            GL.BindVertexArray(0);
        }

        public void UpdateUVs(Vector2[] uvs) {
            GL.BindVertexArray(ID);
            UVs.UpdateData(uvs);
            GL.BindVertexArray(0);
        }
    }
}
