using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Game {
    static class Renderer {

        public static ShaderProgram EntityShader { get; private set; }
        public static ShaderProgram GuiShader { get; private set; }
        public static ShaderProgram TerrainShader { get; private set; }

        private const float FOV = 0.45f, Near = 0.1f, Far = 1000f;
        public const float zoom = -100;
        public static Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, (float)Program.Width / Program.Height, Near, Far);
        public static Matrix4 viewMatrix;

        private static PolygonMode drawingMode = PolygonMode.Fill;
        public static PolygonMode DrawingMode {
            get { return drawingMode; }
            set {
                drawingMode = value;
                Gl.PolygonMode(MaterialFace.FrontAndBack, drawingMode);
            }
        }

        public static void Init() {
            //GL Stuff
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);
            Gl.ClearColor(0.1f, 0.2f, 0.6f, 1);
            Gl.LineWidth(3);

            //Initialise shaders
            EntityShader = new ShaderProgram(LoadFromFile("EntityVertex"), LoadFromFile("EntityFragment"));
            Debug.WriteLine("Entity Shader Log: ");
            Debug.WriteLine(EntityShader.ProgramLog);

            GuiShader = new ShaderProgram(LoadFromFile("GuiVertex"), LoadFromFile("GuiFragment"));
            Debug.WriteLine("Gui Shader Log: ");
            Debug.WriteLine(GuiShader.ProgramLog);

            TerrainShader = new ShaderProgram(LoadFromFile("TerrainVertex"), LoadFromFile("TerrainFragment"));
            Debug.WriteLine("Terrain Shader Log: ");
            Debug.WriteLine(TerrainShader.ProgramLog);

            //Load matrices
            UpdateViewMatrix();

            EntityShader.Use();
            EntityShader["projectionMatrix"].SetValue(projectionMatrix);

            TerrainShader.Use();
            TerrainShader["projectionMatrix"].SetValue(projectionMatrix);

            GuiShader.Use();
            GuiShader["position"].SetValue(new Vector2(-1, -1));

        }

        private static void UpdateViewMatrix() {
            viewMatrix = Matrix4.CreateTranslation(new Vector3(-Player.Instance.Position.x, -Player.Instance.Position.y, zoom));

            EntityShader.Use();
            EntityShader["viewMatrix"].SetValue(viewMatrix);

            TerrainShader.Use();
            TerrainShader["viewMatrix"].SetValue(viewMatrix);
        }

        public static void Render() {
            UpdateViewMatrix();

            RenderEntities();
            RenderTerrain();
            RenderHealthbar();
        }

        private static void RenderHealthbar() {
            GuiShader.Use();

            Healthbar.CalculateVertices();
            Gl.BindBufferToShaderAttribute(Healthbar.Vertices, GuiShader, "vertexPosition");
            Gl.BindBufferToShaderAttribute(Healthbar.Colours, GuiShader, "colour");
            Gl.BindBuffer(Healthbar.Elements);
            Gl.DrawElements(BeginMode.TriangleStrip, Healthbar.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        private static void RenderEntities() {
            EntityShader.Use();

            GameLogic.Entities.Add(Player.Instance);
            foreach (Entity entity in GameLogic.Entities) {
                EntityShader["modelMatrix"].SetValue(entity.ModelMatrix());
                //TODO: support for both coloured and textured models
                ColouredModel model = (ColouredModel)entity.Model;

                Gl.BindBufferToShaderAttribute(model.Vertices, EntityShader, "vertexPosition");
                Gl.BindBufferToShaderAttribute(model.Colours, EntityShader, "colour");
                Gl.BindBuffer(model.Elements);
                Gl.DrawElements(model.DrawingMode, model.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
            GameLogic.Entities.Remove(Player.Instance);
        }

        private static void RenderTerrain() {
            TerrainShader.Use();

            TerrainShader["modelMatrix"].SetValue(Matrix4.Identity);
            TexturedModel model = Terrain.Model;
            Gl.BindBufferToShaderAttribute(model.Vertices, TerrainShader, "vertexPosition");
            Gl.BindBufferToShaderAttribute(model.UVs, TerrainShader, "vertexUV");
            Gl.BindBuffer(model.Elements);
            Gl.BindTexture(model.Texture);
            Gl.DrawElements(model.DrawingMode, model.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        private static string LoadFromFile(string source) {
            StreamReader reader = new StreamReader("Shaders/" + source + ".glsl");
            StringBuilder sb = new StringBuilder();
            string s;
            while ((s = reader.ReadLine()) != null) {
                sb.Append(s);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static void Deinit() {
            EntityShader.DisposeChildren = true;
            EntityShader.Dispose();

            TerrainShader.DisposeChildren = true;
            TerrainShader.Dispose();

            GuiShader.DisposeChildren = true;
            GuiShader.Dispose();


        }
    }
}
