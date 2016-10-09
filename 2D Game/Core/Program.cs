using System;
using OpenGL;
using Tao.FreeGlut;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Game.Util;
using System.Linq;
using Game.Core;
using Game.Fonts;
using Game.TitleScreen;

namespace Game {

    enum ProgramMode {
        TitleScreen, Game
    }
    static class Program {

        public static Random Rand = new Random();

        public static int ScreenWidth, ScreenHeight;
        public static int Width = 1600, Height = 900;
        public static float AspectRatio = (float)Width / Height;

        public static bool FullScreen { get; private set; }

        public static ProgramMode Mode { get; private set; } = ProgramMode.TitleScreen;

        static void Main() {

            Init();

            Glut.glutMainLoop();

            Dispose();

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

        }

        private static void Init() {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(Width, Height);

            ScreenWidth = Glut.glutGet(Glut.GLUT_SCREEN_WIDTH);
            ScreenHeight = Glut.glutGet(Glut.GLUT_SCREEN_HEIGHT);

            Glut.glutInitWindowPosition((ScreenWidth - Width) / 2, (ScreenHeight - Height) / 2);
            Glut.glutCreateWindow("");
            //Glut.glutGameModeString(Width+"x"+Height+":32@60");

            Glut.glutSetOption(Glut.GLUT_ACTION_ON_WINDOW_CLOSE, Glut.GLUT_ACTION_CONTINUE_EXECUTION);
            Gl.Viewport(0, 0, Width, Height);
            Glut.glutReshapeFunc(OnReshape);
            Glut.glutDisplayFunc(delegate () { });
            Glut.glutIdleFunc(MainGameLoop);

            //Console.SetWindowSize(Console.LargestWindowWidth / 4, Console.LargestWindowHeight / 4);
            //Console.SetWindowPosition(0, 0);


            Input.Init();
            Gui.Init();
            SwitchToTitleScreen();
        }

        private static void OnReshape(int width, int height) {
            Width = width;
            Height = height;
            AspectRatio = (float)Width / height;
        }

        //todo
        public static void EnterFullScreen() {
            Glut.glutEnterGameMode();
            FullScreen = true;
        }

        //todo
        public static void ExitFullScreen() {
            Glut.glutLeaveGameMode();
            FullScreen = false;
        }

        public static void SwitchToTitleScreen() {
            Mode = ProgramMode.TitleScreen;
            GameRenderer.CleanUp();
            Gui.SwitchToTitleScreen();
        }

        public static void SwitchToGame() {
            Mode = ProgramMode.Game;
            GameRenderer.Init();
            GameLogic.Init();
            Gui.SwitchToGame();
        }

        private static void MainGameLoop() {
            //  Gl.ClearColor(0, 0, 0, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ErrorCode error = Gl.GetError();
            if (error != ErrorCode.NoError) {
                Console.WriteLine("Opengl Error: " + error);
                //Debug.Assert(false);
            }

            GameTime.Update();
            if (Mode == ProgramMode.Game) {
                GameRenderer.Render();
                GameLogic.Update();
            }
            Input.Update();
            Gui.Render();
            Gui.Update();
            CooldownTimer.Update();




            Glut.glutSwapBuffers();
        }

        private static void Dispose() {
            GameRenderer.CleanUp();
            Gui.Dispose();
        }

    }
}
