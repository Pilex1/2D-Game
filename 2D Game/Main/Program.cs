using Game.Assets;
using Game.Core;
using Game.Core.world_Serialization;
using Game.Guis.Renderers;
using Game.Main.GLConstructs;
using Game.TitleScreen;
using Game.Util;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;
using System.Diagnostics;
using System.Threading;

namespace Game {

    enum ProgramMode {
        None, TitleScreen, Game
    }
    static class Program {

        public static int Width, Height;
        public static int ScreenWidth, ScreenHeight;
        internal static GlfwWindowPtr window;
        internal static GlfwMonitorPtr monitor;

        public static int MainThreadID { get; private set; }
        public static Random Rand = new Random();
        public static float AspectRatio => (float)Width / Height;
        public static ProgramMode Mode { get; private set; }
        internal static string worldname { get; private set; }

        static void Main() {
            Init();
            while (!Glfw.WindowShouldClose(window))
                MainGameLoop();
            Dispose();
        }

        private static void Init() {
            MainThreadID = Thread.CurrentThread.ManagedThreadId;
            if (!Glfw.Init()) {
                throw new Exception("Unable to initialise GLFW");
            }
            monitor = Glfw.GetPrimaryMonitor();
            GlfwVidMode mode = Glfw.GetVideoMode(monitor);
            ScreenWidth = mode.Width;
            ScreenHeight = mode.Height;

            //FULLSCREEN
            //window = Glfw.CreateWindow(mode.Width, mode.Height, "Plexico 2D Game - Copyright Alex Tan 2017", monitor, GlfwWindowPtr.Null);
            //Width = mode.Width;
            //Height = mode.Height;

            //WINDOWED
            Width = 1280;
            Height = 720;
            window = Glfw.CreateWindow(Width, Height, "Plexico 2D Game - Copyright Alex Tan 2017", GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
            Glfw.SetWindowPos(window, (ScreenWidth - Width) / 2, (ScreenHeight - Height) / 2);

            Glfw.MakeContextCurrent(window);
            Glfw.SetErrorCallback(OnError);
            Input.Init();

            //Console.SetWindowSize(Console.LargestWindowWidth / 4, Console.LargestWindowHeight / 4);
            //Console.SetWindowPosition(0, 0);
            Mode = ProgramMode.None;

            Serialization.CreateSaveFolder();
            AssetsManager.Init();
            Input.Init();
            Gui.Init();
            GameTime.Init();
            SwitchToTitleScreen();
        }

        private static void OnError(GlfwError code, string desc) {
            throw new ArgumentException("GLFW Error: " + code + " - " + desc);
        }


        public static void SwitchToTitleScreen() {
            GameLogic.Reset();
            Mode = ProgramMode.TitleScreen;
            TitleScreenRenderer.Reset();
        }


        public static void LoadGame_New(string worldname, int seed) {
            Mode = ProgramMode.Game;
            Program.worldname = worldname;

            GameRenderer.Init();

            GameLogic.InitNew(seed);

            GameTime.Update();
        }

        public static void LoadGame_FromSave(string worldname) {
            Mode = ProgramMode.Game;
            Program.worldname = worldname;

            GameRenderer.Init();

            GameLogic.InitLoad(worldname);

            GameTime.Update();
        }


        private static void MainGameLoop() {
            GL.ClearColor(1, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ErrorCode error = GL.GetError();
            Debug.Assert(error == ErrorCode.NoError);
            GameTime.Update();
            CooldownTimer.Update();

            if (Mode == ProgramMode.Game) {
                GameGuiRenderer.RenderBackground();
                GameRenderer.Render();

                GameLogic.Update();
            }

            GameTime.GuiTimer.Start();
            Gui.Render();
            Gui.Update();
            GameTime.GuiTimer.Pause();

            GameTime.TerrainTimer.Stop();
            GameTime.LightingsTimer.Stop();
            GameTime.EntityUpdatesTimer.Stop();
            GameTime.EntityRenderTimer.Stop();
            GameTime.LogicTimer.Stop();
            GameTime.FluidsTimer.Stop();
            GameTime.GuiTimer.Stop();

            Input.Update();
            Glfw.PollEvents();
            Glfw.SwapBuffers(window);
        }


        private static void Dispose() {
            GameLogic.SaveWorld();
            ResourceManager.CleanUp();

            Glfw.DestroyWindow(window);
            Glfw.Terminate();

            while (GameLogic.saving) Thread.Sleep(1);
        }
    }
}
