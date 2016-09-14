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

            Glut.glutDisplayFunc(delegate () { });
            Glut.glutIdleFunc(MainGameLoop);

            GameLogic.Init();
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
