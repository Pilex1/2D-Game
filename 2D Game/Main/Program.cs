using System;
using OpenGL;
using Tao.FreeGlut;
using System.Diagnostics;
using Game.Util;
using Game.Core;
using Game.TitleScreen;
using Game.Interaction;
using Game.Assets;
using System.Threading;

namespace Game {

    enum ProgramMode {
        None, TitleScreen, Game
    }
    static class Program {

        public static Random Rand = new Random();

        public static int ScreenWidth, ScreenHeight;
        public static int Width = 1280, Height = 720;
        public static float AspectRatio => (float)Width / Height;
        public static ProgramMode Mode { get; private set; }

        internal static string worldname { get; private set; }

        static void Main() {
            Init();
            Glut.glutMainLoop();
            Dispose();
            while (GameLogic.saving) Thread.Sleep(1);
        }

        private static void Init() {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(Width, Height);

            ScreenWidth = Glut.glutGet(Glut.GLUT_SCREEN_WIDTH);
            ScreenHeight = Glut.glutGet(Glut.GLUT_SCREEN_HEIGHT);

            Glut.glutInitWindowPosition((ScreenWidth - Width) / 2, (ScreenHeight - Height) / 2);
            Glut.glutCreateWindow("Plexico 2D Game - Copyright Alex Tan 2016");
            //Glut.glutGameModeString(Width+"x"+Height+":32@60");

            Glut.glutSetOption(Glut.GLUT_ACTION_ON_WINDOW_CLOSE, Glut.GLUT_ACTION_CONTINUE_EXECUTION);
            Gl.Viewport(0, 0, Width, Height);
            Glut.glutReshapeFunc(OnReshape);
            Glut.glutDisplayFunc(() => { });
            Glut.glutIdleFunc(MainGameLoop);

            //Console.SetWindowSize(Console.LargestWindowWidth / 4, Console.LargestWindowHeight / 4);
            //Console.SetWindowPosition(0, 0);
            Mode = ProgramMode.None;

            AssetsManager.Init();
            Input.Init();
            Gui.Init();
            GameTime.Init();
            SwitchToTitleScreen();
        }

        private static void OnReshape(int width, int height) {
            Width = width;
            Height = height;
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
            Gl.ClearColor(1, 1, 1, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ErrorCode error = Gl.GetError();
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
            Glut.glutSwapBuffers();
        }


        private static void Dispose() {
            GameLogic.CleanUp();
            Gui.Dispose();


        }
    }
}
