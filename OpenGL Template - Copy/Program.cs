using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    class Program {
        public static int width = 1280, height = 720;
        private static GlfwWindowPtr window;

        internal static ShaderProgram shader;

        private static Entity quad;

        static void Main(string[] args) {
            if (!Glfw.Init())
                return;

            window = Glfw.CreateWindow(width, height, "", GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
            Glfw.MakeContextCurrent(window);
            Glfw.SetErrorCallback(OnError);
            Input.Init(window);

            Init();

            while (!Glfw.WindowShouldClose(window))
                MainLoop();

            Glfw.DestroyWindow(window);
            Glfw.Terminate();

            CleanUp();
        }

        private static void CleanUp() {
            shader.Dispose();
        }

        private static void Init() {
            shader = new ShaderProgram("Assets/Shaders/Shader.vert", "Assets/Shaders/Shader.frag");

            shader.AddUniform("vposoffset");
            shader.AddUniform("vsize");

            /*
             * 
             * write your code here
             * 
             */

            quad = new Entity(Model.CreateRectangle(new Vector2(0.5f, 0.25f), new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 1, 1) }), new Vector2(0, 0));

            /*
             * 
             *
             * 
             */
        }


        private static void MainLoop() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GameTime.Update();
            Glfw.SetWindowTitle(window, "FPS: " + GameTime.FPS);

            CooldownTimer.Update();

           
            Update();
            Render(quad);
            

            Glfw.SwapBuffers(window);

            Glfw.PollEvents();
        }

        private static void Update() {

            /*
             * 
             * write your code here
             * 
             */

            float x = 0.01f * GameTime.DeltaTime;
            float y = 0.01f * GameTime.DeltaTime;

            if (Input.Keys(Key.W)) {
                quad.pos.Y += y;
            }
            if (Input.Keys(Key.S)) {
                quad.pos.Y -= y;
            }
            if (Input.Keys(Key.D)) {
                quad.pos.X += x;
            }
            if (Input.Keys(Key.A)) {
                quad.pos.X -= x;
            }

            /*
             *
             *
             * 
             */
        }

        private static void Render(Entity entity) {

            /*
            * 
            * write your code here
            * 
            */

            Model model = entity.model;
            GL.UseProgram(shader.id);
            GL.BindVertexArray(model.vao.ID);

            shader.SetUniform2f("vposoffset", entity.pos);
            shader.SetUniform2f("vsize", model.size);

            GL.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            /*
            *
            *
            * 
            */
        }

        private static void OnError(GlfwError code, string desc) {
            throw new ArgumentException("OpenGL Error: " + code + " - " + desc);
        }
    }
}
