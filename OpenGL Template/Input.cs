using Pencil.Gaming;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    static class Input {

        private static bool[] keys;
        public static float mouse_cumul_x { get; private set; }
        public static float mouse_cumul_y { get; private set; }
        public static float mouse_x { get; private set; }
        public static float mouse_y { get; private set; }
        public static float mouse_dx { get; private set; }
        public static float mouse_dy { get; private set; }
        private static bool first_mouse;

        private const float mouse_sensitivity = 0.01f;

        private static GlfwWindowPtr window;

        public static void Init(GlfwWindowPtr window) {
            Input.window = window;

            keys = new bool[1024];
            first_mouse = true;

            Glfw.SetKeyCallback(window, OnKey);
            Glfw.SetCursorPosCallback(window, OnCursor);
        }

        private static void OnCursor(GlfwWindowPtr wnd, double x, double y) {
            if (first_mouse) {
                mouse_x = (float)x;
                mouse_y = (float)y;
                first_mouse = false;
            }

            mouse_cumul_x += mouse_sensitivity * ((float)x - mouse_x);
            mouse_cumul_y += mouse_sensitivity * ((float)y-mouse_y);
            mouse_x = (float)x;
            mouse_y = (float)y;
        }

        public static bool KeyDown(Key k) {
            return keys[(int)k];
        }

        private static void OnKey(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) {
            if (action == KeyAction.Press) {
                keys[(int)key] = true;
            } else if (action == KeyAction.Release) {
                keys[(int)key] = false;
            }
        }
    }
}
