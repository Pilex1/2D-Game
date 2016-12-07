using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;

namespace Game.Core {
    static class Input {

        public static bool[] Keys { get; private set; }
        public static bool[] SpecialKeys { get; private set; }
        public static bool[] KeysTyped { get; private set; }
        private static CooldownTimer[] KeysTypedCooldown;
        public static bool[] Mouse { get; private set; }
        public static int MouseX { get; private set; }
        public static int MouseY { get; private set; }
        public static Vector2i MouseCoords { get { return new Vector2i(MouseX, MouseY); } }
        public static float NDCMouseX { get { return (2.0f * MouseX) / Program.Width - 1.0f; } }
        public static float NDCMouseY { get { return 1.0f - (2.0f * MouseY + 28) / Program.Height; } }
        public static Vector2 NDCMouse { get { return new Vector2(NDCMouseX, NDCMouseY); } }
        public static int MouseScroll { get; private set; }
        public const int MouseLeft = 0, MouseMiddle = 1, MouseRight = 2;

        public static void Init() {
            Keys = new bool[255];
            KeysTyped = new bool[255];
            SpecialKeys = new bool[255];
            Mouse = new bool[3];
            KeysTypedCooldown = new CooldownTimer[255];
            for (int i = 0; i < KeysTypedCooldown.Length; i++) {
                KeysTypedCooldown[i] = new CooldownTimer(3);
            }

            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutSpecialFunc(OnSpecialDown);
            Glut.glutSpecialUpFunc(OnSpecialUp);
            Glut.glutMouseFunc(OnMousePress);
            Glut.glutMotionFunc(OnMouseMove);
            Glut.glutPassiveMotionFunc(OnMouseMove);
            Glut.glutMouseWheelFunc(OnMouseScroll);
        }

        private static void OnSpecialUp(int key, int x, int y) {
            SpecialKeys[key] = false;
        }

        private static void OnSpecialDown(int key, int x, int y) {
            SpecialKeys[key] = true;
        }

        public static void Update() {
            MouseScroll = 0;
            for (int i = 0; i < KeysTyped.Length; i++) {
                if (KeysTypedCooldown[i].Ready())
                    KeysTyped[i] = false;
            }
        }

        public static Vector2 RayCast() {
            Vector2 normalizedCoords = new Vector2(NDCMouseX, NDCMouseY);
            Vector4 clipCoords = new Vector4(normalizedCoords.x, normalizedCoords.y, -1, 1);
            Vector4 eyeCoords = GameRenderer.projectionMatrix.Inverse() * clipCoords;
            eyeCoords.z = -1;
            eyeCoords.w = 0;
            Matrix4 inverseViewMatrix = GameRenderer.viewMatrix.Inverse();
            Vector4 rayWorldTemp = inverseViewMatrix * eyeCoords;
            Vector2 rayWorld = new Vector2(rayWorldTemp.x, rayWorldTemp.y);
            return rayWorld;
        }

        public static Vector2 TerrainIntersect() {
            Vector2 rayWorld = RayCast();
            Vector2 intersectTerrain = Player.Instance.data.Position.val - rayWorld * GameRenderer.zoom;
            return intersectTerrain;
        }

        public static float RayCastAngle() {
            Vector2 playerpos = Player.Instance.data.Position.val;
            Vector2 raycast = TerrainIntersect();
            return MathUtil.AngleFrom(playerpos, raycast);
        }

        #region Glut Callbacks
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
            KeysTyped[key] = true;
            KeysTypedCooldown[key].Reset();
        }
        private static void OnKeyboardUp(byte key, int x, int y) {
            Keys[key] = false;
        }
        #endregion Glut Callbacks
    }
}
