using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System.Diagnostics;
using System.Collections;

namespace Tests {
    class Program {

        static int width = 1280, height = 720;
        static GlfwWindowPtr window;
        internal static ShaderProgram shader;

        static Fractal mandelbrot, julia;
        static Fractal.FractalType activeFractal;
        static CooldownTimer activeFractalTimer;

        static void Main(string[] args) {
            if (!Glfw.Init())
                return;

            window = Glfw.CreateWindow(width, height, "GLFW OpenGL", GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
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
            Serialization.Save(mandelbrot, julia, activeFractal);
        }

        private static void OnError(GlfwError code, string desc) {
            throw new ArgumentException("OpenGL Error: " + code + " - " + desc);
        }

        private static void Init() {
            shader = new ShaderProgram("Assets/Shaders/Shader.vert", "Assets/Shaders/Shader.frag");

            shader.AddUniform("vposoffset");
            shader.AddUniform("vsize");

            shader.AddUniform("aspectRatio");
            shader.AddUniform("rot");
            shader.AddUniform("maxIter");
            shader.AddUniform("clrRatio");
            shader.AddUniform("cursorClr");
            shader.AddUniform("julia_mode");
            shader.AddUniform("crosshair");

            shader.AddUniform("fractalType");
            shader.AddUniform("julia_c");
            shader.AddUniform("pos");
            shader.AddUniform("zoom");

            try {
                Serialization.FractalPair pair = Serialization.Load();
                mandelbrot = pair.mandelbrot;
                julia = pair.julia;
                activeFractal = pair.activeFractal;
            } catch (Exception) {
                mandelbrot = Fractal.CreateMandelbrot();
                julia = Fractal.CreateJulia();
            }
            mandelbrot.Load();
            julia.Load();

            activeFractalTimer = new CooldownTimer(60);
            activeFractalTimer.SetTime(activeFractalTimer.GetCooldown());
        }

        private static void MainLoop() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GameTime.Update();
            Glfw.SetWindowTitle(window, "FPS: " + GameTime.FPS);

            CooldownTimer.Update();
            Update();
            Render(mandelbrot);
            Render(julia);

            Glfw.SwapBuffers(window);

            Glfw.PollEvents();
        }

        private static void Update() {
            if (Input.Keys(Key.Tab)) {
                if (activeFractalTimer.Ready()) {
                    activeFractal = activeFractal == Fractal.FractalType.Mandelbrot ? Fractal.FractalType.Julia : Fractal.FractalType.Mandelbrot;
                    activeFractalTimer.Reset();
                }
            }

            if (activeFractal == Fractal.FractalType.Mandelbrot) {
                HandleKeys(mandelbrot);
            } else {
                HandleKeys(julia);
            }

            float clr_amt = GameTime.DeltaTime / 1000 * (Input.Keys(Key.LeftShift) ? 1 : -1);
            if (Input.Keys(Key.Z)) {
                mandelbrot.clrRatio.R += clr_amt;
                julia.clrRatio.R += clr_amt;
            }
            if (Input.Keys(Key.X)) {
                mandelbrot.clrRatio.G += clr_amt;
                julia.clrRatio.G += clr_amt;
            }
            if (Input.Keys(Key.C)) {
                mandelbrot.clrRatio.B += clr_amt;
                julia.clrRatio.B += clr_amt;
            }

            if (Input.Keys(Key.J)) {
                mandelbrot.rot -= GameTime.DeltaTime / 200;
                julia.rot -= GameTime.DeltaTime / 200;
            }
            if (Input.Keys(Key.L)) {
                mandelbrot.rot += GameTime.DeltaTime / 200;
                julia.rot += GameTime.DeltaTime / 200;
            }

            if (Input.Keys(Key.One)) {
                julia.SetMode(Fractal.Mode.Normal);
            }
            if (Input.Keys(Key.Two)) {
                julia.SetMode(Fractal.Mode.Reciprocal);
            }
            if (Input.Keys(Key.Three)) {
                julia.SetMode(Fractal.Mode.SquaredReciprocal);
            }
            if (Input.Keys(Key.Four)) {
                julia.SetMode(Fractal.Mode.t1);
            }
            if (Input.Keys(Key.Five)) {
                julia.SetMode(Fractal.Mode.t2);
            }
            if (Input.Keys(Key.Six)) {
                julia.SetMode(Fractal.Mode.t3);
            }

            if (Input.Keys(Key.Escape)) {
                Glfw.SetWindowShouldClose(window, true);
            }
        }

        private static void HandleKeys(Fractal fractal) {
            double x = GameTime.DeltaTime / 60 * fractal.zoom * Math.Sin(fractal.rot) * (Input.Keys(Key.LeftShift) ? 0.05 : 1) * (Input.Keys(Key.LeftControl) ? 0.05 : 1) * (Input.Keys(Key.LeftAlt) ? 0.05 : 1);
            double y = GameTime.DeltaTime / 60 * fractal.zoom * Math.Cos(fractal.rot) * (Input.Keys(Key.LeftShift) ? 0.05 : 1) * (Input.Keys(Key.LeftControl) ? 0.05 : 1) * (Input.Keys(Key.LeftAlt) ? 0.05 : 1);
            if (Input.Keys(Key.W)) {
                fractal.pos.x += x;
                fractal.pos.y += y;
            }
            if (Input.Keys(Key.S)) {
                fractal.pos.x -= x;
                fractal.pos.y -= y;
            }
            if (Input.Keys(Key.D)) {
                fractal.pos.x += y;
                fractal.pos.y -= x;
            }
            if (Input.Keys(Key.A)) {
                fractal.pos.x -= y;
                fractal.pos.y += x;
            }

            if (Input.Keys(Key.I)) {
                fractal.zoom *= (float)Math.Pow(0.99, GameTime.DeltaTime);
            }
            if (Input.Keys(Key.K)) {
                fractal.zoom /= (float)Math.Pow(0.99, GameTime.DeltaTime);
            }

            if (Input.Keys(Key.Y)) fractal.maxIter /= (float)Math.Pow(0.999, GameTime.DeltaTime);
            if (Input.Keys(Key.H)) fractal.maxIter *= (float)Math.Pow(0.999, GameTime.DeltaTime);
            if (fractal.maxIter < 1) fractal.maxIter = 1;

            if (Input.Keys(Key.T)) {
                fractal.ToggleCrosshair();
            }

            if (Input.Keys(Key.R)) {
                fractal.Reset();
            }
        }

        private static void Render(Fractal fractal) {
            Model model = fractal.quad.model;
            GL.UseProgram(shader.ID);
            GL.BindVertexArray(model.vao.ID);

            shader.SetUniform2f("vposoffset", fractal.quad.pos);
            shader.SetUniform2f("vsize", fractal.quad.model.size);

            shader.SetUniform1f("aspectRatio", (float)width / height);
            shader.SetUniform4m("rot", Matrix.CreateRotationZ(fractal.rot));
            shader.SetUniform1i("maxIter", (int)fractal.maxIter);
            shader.SetUniform4c("clrRatio", fractal.clrRatio);
            if (activeFractal == fractal.fractalType) {
                float factor = activeFractalTimer.GetTime() / activeFractalTimer.GetCooldown();
                if (factor > 1) factor = 1;
                shader.SetUniform3f("cursorClr", new Vector3(factor, factor, factor));
            } else {
                shader.SetUniform3f("cursorClr", new Vector3(1f, 1f, 1f));
            }
            shader.SetUniform1b("crosshair",fractal.crosshair);
            shader.SetUniform1i("julia_mode", (int)fractal.mode);

            shader.SetUniform1i("fractalType", (int)fractal.fractalType);
            if (fractal.fractalType == Fractal.FractalType.Mandelbrot) {
                shader.SetUniform2d("julia_c", fractal.pos);
            }
            shader.SetUniform2d("pos", fractal.pos);
            shader.SetUniform1d("zoom", fractal.zoom);

            GL.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }



    }
}
