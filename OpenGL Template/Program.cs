using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    class Program {
        public static int width = 1280, height = 720;
        private static GlfwWindowPtr window;

        static void Main(string[] args) {
            if (!Glfw.Init())
                return;

            window = Glfw.CreateWindow(width, height, "", GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
            Glfw.MakeContextCurrent(window);
            Glfw.SetInputMode(window, InputMode.CursorMode, CursorMode.CursorCaptured);
            Glfw.SetErrorCallback(OnError);
            Input.Init(window);

            Renderer.Init();
            Camera.Init(new Vector3(0, 0, -5), Vector3.Zero);
            Terrain.Init(Terrain.GenerateHeights());
            Init();

            while (!Glfw.WindowShouldClose(window))
                MainLoop();

            Glfw.DestroyWindow(window);
            Glfw.Terminate();

            Renderer.CleanUp();
        }
        private static void Init() {

            var quad_model = Model.CreateRectangle(new Color4[] { Color4.Red, Color4.Blue, Color4.Green, Color4.Violet });
            var quad = new Entity(quad_model, Vector3.Zero, new Vector3(0.5f, 0.5f, 1), Vector3.Zero);
            Renderer.AddEntity(quad);

            var cube_model = Model.CreateCube(new Color4[] { Color4.Red, Color4.Blue, Color4.Green, Color4.Violet, Color4.Orange, Color4.Yellow, Color4.Turquoise, Color4.Peru });
            var cube = new Entity(cube_model, new Vector3(1, 0, -1), 0.1f * Vector3.One, new Vector3(0, 0, 0.3f));
            Renderer.AddEntity(cube);

        }


        private static void MainLoop() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GameTime.Update();
            Glfw.SetWindowTitle(window, "FPS: " + GameTime.FPS);

            CooldownTimer.Update();

            Glfw.PollEvents();
            Update();
            Renderer.Render();


            Glfw.SwapBuffers(window);

        }

        private static void Update() {

            /*
             * 
             * write your code here
             * 
             */

            float speed_x = 0.05f * GameTime.DeltaTime;

            if (Input.KeyDown(Key.W)) {
                Camera.MoveForward(speed_x);
            }
            if (Input.KeyDown(Key.S)) {
                Camera.MoveBackward(speed_x);
            }
            if (Input.KeyDown(Key.D)) {
                Camera.MoveRight(speed_x);
            }
            if (Input.KeyDown(Key.A)) {
                Camera.MoveLeft(speed_x);
            }
            if (Input.KeyDown(Key.Space)) {
                Camera.MoveUp(speed_x);
            }
            if (Input.KeyDown(Key.LeftShift)) {
                Camera.MoveDown(speed_x);
            }

            Camera.rot = new Vector3(Input.mouse_cumul_y,  0 ,Input.mouse_cumul_x);
            
        }

        private static void OnError(GlfwError code, string desc) {
            throw new ArgumentException("OpenGL Error: " + code + " - " + desc);
        }
    }
}
