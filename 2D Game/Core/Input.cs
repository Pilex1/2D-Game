using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;

namespace Game.Core {
    static class Input {

        public static bool[] Keys { get; private set; }
        public static bool[] Mouse { get; private set; }
        public static int MouseX { get; private set; }
        public static int MouseY { get; private set; }
        public static float NDCMouseX { get { return (2.0f * MouseX) / Program.Width - 1.0f; } }
        public static float NDCMouseY { get { return 1.0f - (2.0f * MouseY) / Program.Height; } }
        public static int MouseScroll { get; private set; }
        public const int MouseLeft = 0, MouseMiddle = 1, MouseRight = 2;

        public static void Init() {
            Keys = new bool[255];
            Mouse = new bool[3];

            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutMouseFunc(OnMousePress);
            Glut.glutMotionFunc(OnMouseMove);
            Glut.glutPassiveMotionFunc(OnMouseMove);
            Glut.glutMouseWheelFunc(OnMouseScroll);
        }

        public static void Update() {
            MouseScroll = 0;
        }

        private static void OnMouseScroll(int button, int dir, int x, int y) {
            MouseScroll = dir;
        }

        private static void OnMouseMove(int x, int y) {
            MouseX = x;
            MouseY = y;
        }

        private static void OnMousePress(int button, int state, int mx, int my) {
            if (button == Glut.GLUT_LEFT_BUTTON) {
                Mouse[MouseLeft] = (state == Glut.GLUT_DOWN);
            }

            if (button == Glut.GLUT_MIDDLE_BUTTON) {
                Mouse[MouseMiddle] = (state == Glut.GLUT_DOWN);
            }

            if (button == Glut.GLUT_RIGHT_BUTTON) {
                Mouse[MouseRight] = (state == Glut.GLUT_DOWN);
            }

            MouseX = mx;
            MouseY = my;
        }
        private static void OnKeyboardDown(byte key, int x, int y) {
            Keys[key] = true;
           // if (key == 27) Glut.glutLeaveMainLoop();
        }
        private static void OnKeyboardUp(byte key, int x, int y) {
            Keys[key] = false;
        }
    }
}
