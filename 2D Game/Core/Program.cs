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


        public static int ScreenWidth, ScreenHeight;
        public const int Width = 1600, Height = 900;
        public const float AspectRatio = (float)Width / Height;

        public static bool FullScreen { get; private set; }

        public static ProgramMode Mode { get; private set; } = ProgramMode.TitleScreen;

        static void Main() {

            Init();

            Glut.glutMainLoop();

            CleanUp();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

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
            Glut.glutDisplayFunc(delegate () { });
            Glut.glutIdleFunc(MainGameLoop);

            Console.SetWindowSize(Console.LargestWindowWidth / 4, Console.LargestWindowHeight / 4);
            Console.SetWindowPosition(0, 0);

            Input.Init();
            Gui.Init();
            SwitchToTitleScreen();
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
            GameLogic.Dispose();
            Gui.SwitchToTitleScreen();
        }

        public static void SwitchToGame() {
            Mode = ProgramMode.Game;
            GameLogic.Init();
            Gui.SwitchToGame();
        }

        private static void MainGameLoop() {
            //  Gl.ClearColor(0, 0, 0, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ErrorCode error = Gl.GetError();
            if (error != ErrorCode.NoError) {
                Console.WriteLine("Opengl Error: " + error);
                Debug.Assert(false);
            }

            if (Mode == ProgramMode.Game) {
                GameLogic.Render();
                GameLogic.Update();
            }
            Gui.Render();
            Gui.Update();
            Input.Update();

            Glut.glutSwapBuffers();
        }

        private static void CleanUp() {
            GameLogic.Dispose();
            Gui.Dispose();
        }

    }
}
