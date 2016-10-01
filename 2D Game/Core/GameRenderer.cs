using Game.Core;
using Game.Entities;
using Game.Fluids;
using Game.Interaction;
using Game.Terrains;
using Game.Util;
using OpenGL;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Game {
    static class GameRenderer {

        public static ShaderProgram EntityShader { get; private set; }
        public static ShaderProgram GuiShader { get; private set; }
        public static ShaderProgram TerrainShader { get; private set; }
        public static ShaderProgram BackgroundShader { get; private set; }

        private const float FOV = 0.45f, Near = 0.1f, Far = 1000f;
        public const float zoom = -100;
        public static Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, (float)Program.Width / Program.Height, Near, Far);
        public static Matrix4 viewMatrix;

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

            BackgroundShader = new ShaderProgram(LoadFromFile("BackgroundVertex"), LoadFromFile("BackgroundFragment"));
            Debug.WriteLine("Background Shader Log: ");
            Debug.WriteLine(BackgroundShader.ProgramLog);

            //Load matrices
            UpdateViewMatrix();

            EntityShader.Use();
            EntityShader["projectionMatrix"].SetValue(projectionMatrix);

            TerrainShader.Use();
            TerrainShader["projectionMatrix"].SetValue(projectionMatrix);


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

           
            //    RenderBackground();
            RenderGUIs();
            
        }

        //private static void RenderBackground() {
        //    BackgroundShader.Use();
        //    Gl.BindVertexArray(Background.vao);
        //    Gl.BindBufferToShaderAttribute(Background.vertices, BackgroundShader, "vpos");
        //    Gl.BindBuffer(Background.elements);
        //    foreach (var back in Background.BackgroundSet) {
        //        Gl.BindTexture(back.texture);
        //        Gl.DrawElements(BeginMode.Triangles, Background.elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        //    }
        //    Gl.BindVertexArray(0);
        //    Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //    Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        //}

        private static void RenderGUIs() {
            GuiShader.Use();

            Healthbar.Update();
            RenderInstanceGUI(Healthbar.Model, new Vector2((2 - Healthbar.BarWidth) / 2, 0.01 + Hotbar.SizeY));

            Gl.LineWidth(7);
            RenderInstanceGUI(Hotbar.CurSelected, new Vector2((2 - Inventory.InvColumns * Hotbar.SizeX) / 2 + Hotbar.CurSelectedSlot * Hotbar.SizeX, 0));
            RenderInstanceGUI(Hotbar.TexturedItems, new Vector2((2 - Inventory.InvColumns * Hotbar.SizeX) / 2, 0));
            RenderInstanceGUI(Hotbar.Frame, new Vector2((2 - Inventory.InvColumns * Hotbar.SizeX) / 2, 0));
            //RenderInstanceGUI(Background.landscape.model, new Vector2(0, 1));
            Gl.LineWidth(3);

        }

        private static void RenderInstanceGUI(TexturedModel model, Vector2 position) {
            GuiShader["position"].SetValue(position);
            Gl.BindBufferToShaderAttribute(model.Vertices, GuiShader, "vertexPosition");
            Gl.BindTexture(model.Texture);
            Gl.BindBufferToShaderAttribute(((TexturedModel)model).UVs, GuiShader, "uv");
            Gl.BindBuffer(model.Elements);
            Gl.DrawElements(model.DrawingMode, model.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static void RenderEntities() {
            EntityShader.Use();

            GameLogic.Entities.Add(Player.Instance);
            foreach (Entity entity in GameLogic.Entities) {
                RenderInstanceEntity(entity);
            }
            GameLogic.Entities.Remove(Player.Instance);
        }

        private static void RenderInstanceEntity(Entity entity) {
            EntityShader["modelMatrix"].SetValue(entity.ModelMatrix());
            ColouredModel model = (ColouredModel)entity.Model;
            Gl.BindBufferToShaderAttribute(model.Vertices, EntityShader, "vertexPosition");
            Gl.BindBufferToShaderAttribute(model.Colours, EntityShader, "colour");
            Gl.BindBuffer(model.Elements);
            Gl.DrawElements(model.DrawingMode, model.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static void RenderTerrain() {
            TerrainShader.Use();

            var terrainModel = Terrain.Model;
            RenderInstanceTerrain(terrainModel);
        }

        private static void RenderInstanceTerrain(LightingTexturedModel model) {
            if (model.Elements.Count > 0) {
                Gl.BindBufferToShaderAttribute(model.Vertices, TerrainShader, "vertexPosition");
                Gl.BindBufferToShaderAttribute(model.UVs, TerrainShader, "vertexUV");
                Gl.BindBufferToShaderAttribute(model.Lightings, TerrainShader, "vertexLighting");
                Gl.BindBuffer(model.Elements);
                Gl.BindTexture(model.Texture);
                Gl.DrawElements(model.DrawingMode, model.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
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

        public static void CleanUp() {
            EntityShader.DisposeChildren = true;
            EntityShader.Dispose();

            TerrainShader.DisposeChildren = true;
            TerrainShader.Dispose();

            GuiShader.DisposeChildren = true;
            GuiShader.Dispose();


        }
    }
}
