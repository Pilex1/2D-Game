using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    static class Renderer {

        internal static ShaderProgram shader;


        private static HashSet<Entity> Entities;

        public static void Init() {
            Entities = new HashSet<Entity>();

            shader = new ShaderProgram("Assets/Shaders/Shader.vert", "Assets/Shaders/Shader.frag");
            shader.AddUniform("modelMatrix");
            shader.AddUniform("viewMatrix");
            shader.AddUniform("projectionMatrix");
        }

        public static void AddEntity(Entity e) {
            Entities.Add(e);
        }
        public static void RemoveEntity(Entity e) {
            Entities.Remove(e);
        }

        public static void Render() {

            GL.ClearColor(Color4.WhiteSmoke);

            GL.UseProgram(shader.id);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            shader.SetUniform4m("viewMatrix", Camera.GetViewMatrix());
            shader.SetUniform4m("projectionMatrix", Camera.GetProjectionMatrix());

            foreach (var e in Entities) {
                RenderInstance(e.model, e.GetModelMatrix());
            }
            RenderInstance(Terrain.model, Matrix.CreateScale(0.2f));

            GL.Disable(EnableCap.DepthTest);

            GL.UseProgram(0);
        }

        private static void RenderInstance(Model model, Matrix modelMatrix) {
            GL.BindVertexArray(model.vao.ID);
            shader.SetUniform4m("modelMatrix", modelMatrix);
            GL.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public static void CleanUp() {
            shader.Dispose();
        }

    }
}
