using System;
using OpenGL;
using Tao.FreeGlut;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Game.Util;
using System.Linq;

namespace Game {
    static class Program {

        public static int ScreenWidth, ScreenHeight;
        public const int Width = 1600, Height = 900;
        public const float AspectRatio = (float)Width / Height;

        public static bool FullScreen { get; private set; }

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

            GameLogic.Init();
        }

        public static void EnterFullScreen() {
            Glut.glutEnterGameMode();
            FullScreen = true;
        }

        public static void ExitFullScreen() {
            Glut.glutLeaveGameMode();
            FullScreen = false;
        }

        private static void MainGameLoop() {
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ErrorCode error = Gl.GetError();
            if (error != ErrorCode.NoError) {
                Debug.Write("Opengl ERROR: " + error);
                Debug.Assert(false);
            }

            GameLogic.Update();
            GameLogic.Render();

            Glut.glutSwapBuffers();
        }

        private static void CleanUp() {
            GameLogic.CleanUp();
        }

    }
}
