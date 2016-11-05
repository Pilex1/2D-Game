using Pencil.Gaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    static class Input {

        private static bool[] keys = new bool[1024];

        public static void Init(GlfwWindowPtr window) {
            Glfw.SetKeyCallback(window, OnKey);
        }

        public static bool Keys(Key k) {
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
