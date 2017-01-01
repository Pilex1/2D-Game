using System;
using Game.Util;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;

namespace Game.Core {
    static class Input {

        #region Fields
        public const int MaxKey = 511;
        private static bool[] Keys;
        public static bool[] KeysTyped { get; private set; }
        private static CooldownTimer[] KeysTypedCooldown;
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
            KeysTyped = new bool[MaxKey];
            KeysTypedCooldown = new CooldownTimer[MaxKey];
            CharsTyped = new Queue<char>();
            for (int i = 0; i < KeysTypedCooldown.Length; i++) {
                KeysTypedCooldown[i] = new CooldownTimer(3);
            }
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
            if (action == KeyAction.Press) {
                Keys[(int)key] = true;
                KeysTyped[(int)key] = true;
                KeysTypedCooldown[(int)key].Reset();
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
            for (int i = 0; i < KeysTyped.Length; i++) {
                if (KeysTypedCooldown[i].Ready())
                    KeysTyped[i] = false;
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

        public static bool KeyTyped(char c) {
            if (char.IsUpper(c)) {
                char lower = char.ToLower(c);
                if (lower >= KeysTyped.Length) return false;
                return Mod_Shift && KeysTyped[lower];
            } else {
                if (c >= KeysTyped.Length) return false;
                return !Mod_Shift && KeysTyped[c];
            }
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
