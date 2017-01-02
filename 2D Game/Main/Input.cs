using Game.Util;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;

namespace Game.Core {
    static class Input {

        #region Fields
        public const int MaxKey = 511;
        private static bool[] Keys;
        private static bool backspace;
        //public static bool Backspace => (backspacefirst && backspace) ? true : backspace &&BackspaceCooldown.Ready();
        private static CooldownTimer BackspaceCooldown;
        public static Queue<char> CharsTyped;
        private static bool[] Mouse;
        public static int MouseX { get; private set; }
        public static int MouseY { get; private set; }
        public static Vector2i MouseCoords { get { return new Vector2i(MouseX, MouseY); } }
        public static float NDCMouseX { get { return (2.0f * MouseX) / Program.Width - 1.0f; } }
        public static float NDCMouseY { get { return 1.0f - (2.0f * MouseY) / Program.Height; } }
        public static Vector2 NDCMouse { get { return new Vector2(NDCMouseX, NDCMouseY); } }
        public static float MouseScroll { get; private set; }
        public static bool Mod_Shift, Mod_Ctrl, Mod_Alt, Mod_Super;
        #endregion

        #region Init
        public static void Init() {
            Keys = new bool[MaxKey];
            BackspaceCooldown = new CooldownTimer(20);
            CharsTyped = new Queue<char>();
            Mouse = new bool[8];

            Glfw.SetKeyCallback(Program.window, KeyCallback);
            Glfw.SetCursorPosCallback(Program.window, CursorPosCallback);
            Glfw.SetMouseButtonCallback(Program.window, MouseButtonCallback);
            Glfw.SetScrollCallback(Program.window, ScrollCallback);
            Glfw.SetCharCallback(Program.window, CharCallback);
        }

        #endregion

        #region Callbacks

        private static void CharCallback(GlfwWindowPtr wnd, char ch) {
            CharsTyped.Enqueue(ch);
        }

        private static void ScrollCallback(GlfwWindowPtr wnd, double xoffset, double yoffset) {
            MouseScroll = (float)yoffset;
        }

        private static void CursorPosCallback(GlfwWindowPtr wnd, double x, double y) {
            MouseX = (int)x;
            MouseY = (int)y;
        }

        private static void KeyCallback(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) {
            if (key < 0) return;
            if (action == KeyAction.Press) {
                Keys[(int)key] = true;
                if (key == Key.Backspace) {
                    CharsTyped.Enqueue('\b');
                    backspace = true;
                    BackspaceCooldown.Reset();
                }
                int i = (int)mods;
                if (i >= 8) {
                    Mod_Super = true;
                    i -= 8;
                }
                if (i >= 4) {
                    Mod_Alt = true;
                    i -= 4;
                }
                if (i >= 2) {
                    Mod_Ctrl = true;
                    i -= 2;
                }
                if (i >= 1) {
                    Mod_Shift = true;
                    i -= 1;
                }
            } else if (action == KeyAction.Release) {
                Keys[(int)key] = false;
                int i = (int)mods;
                if (key == Key.Backspace) {
                    backspace = false;
                    BackspaceCooldown.Reset();
                }
                if (i >= 8) {
                    Mod_Super = false;
                    i -= 8;
                }
                if (i >= 4) {
                    Mod_Alt = false;
                    i -= 4;
                }
                if (i >= 2) {
                    Mod_Ctrl = false;
                    i -= 2;
                }
                if (i >= 1) {
                    Mod_Shift = false;
                    i -= 1;
                }
            }
        }

        private static void MouseButtonCallback(GlfwWindowPtr wnd, MouseButton btn, KeyAction action) {
            if (action == KeyAction.Press) {
                Mouse[(int)btn] = true;
            } else if (action == KeyAction.Release) {
                Mouse[(int)btn] = false;
            }
        }

        #endregion

        #region Update
        public static void Update() {
            MouseScroll = 0;
            if (backspace && BackspaceCooldown.Ready()) {
                CharsTyped.Enqueue('\b');
            }
            Mod_Shift = Mod_Ctrl = Mod_Alt = Mod_Super = false;
        }
        #endregion

        #region Util
        public static bool MouseDown(MouseButton btn) {
            return Mouse[(int)btn];
        }

        public static bool KeyDown(Key k) {
            return Keys[(int)k];
        }

        public static Vector2 RayCast() {
            Vector2 normalizedCoords = new Vector2(NDCMouseX, NDCMouseY);
            Vector4 clipCoords = new Vector4(normalizedCoords.x, normalizedCoords.y, -1, 1);
            Matrix inverseProjectionMatrix = Matrix.Identity;
            Matrix projectionMatrix = GameRenderer.projectionMatrix;
            Matrix.Invert(ref projectionMatrix, out inverseProjectionMatrix);
            Vector4 eyeCoords = Matrix.Mult(clipCoords, inverseProjectionMatrix);
            eyeCoords.z = -1;
            eyeCoords.w = 0;
            Matrix inverseViewMatrix = Matrix.Identity;
            Matrix viewMatrix = GameRenderer.viewMatrix;
            Matrix.Invert(ref viewMatrix, out inverseViewMatrix);
            Vector4 rayWorldTemp = Matrix.Mult(eyeCoords, inverseViewMatrix);
            Vector2 rayWorld = new Vector2(rayWorldTemp.x, rayWorldTemp.y);
            return rayWorld;
        }

        public static Vector2 TerrainIntersect() {
            Vector2 rayWorld = RayCast();
            Vector2 intersectTerrain = Player.Instance.data.pos.val - rayWorld * GameRenderer.zoom;
            return intersectTerrain;
        }

        public static float RayCastAngle() {
            Vector2 playerpos = Player.Instance.data.pos.val;
            Vector2 raycast = TerrainIntersect();
            return MathUtil.GetAngleFrom(playerpos, raycast);
        }
        #endregion
    }
}
