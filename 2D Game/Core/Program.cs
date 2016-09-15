using System;
using OpenGL;
using Tao.FreeGlut;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Game {
    static class Program {

        public const int Width = 1280, Height = 720;
        public const float AspectRatio = (float)Width / Height;

        public static bool FullScreen { get; private set; }
       
        static void Main() {

            Init();

            Glut.glutMainLoop();

            Deinit();
        }

        private static void Init() {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(Width, Height);
            Glut.glutCreateWindow("");
            //Glut.glutGameModeString(Width+"x"+Height+":32@60");

            Glut.glutDisplayFunc(delegate () { });
            Glut.glutIdleFunc(MainGameLoop);

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
            Gl.Viewport(0, 0, Width, Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GameLogic.Update();
            GameLogic.Render();

            Glut.glutSwapBuffers();
        }

        private static void Deinit() {
            GameLogic.Deinit();
        }
        
    }
}
