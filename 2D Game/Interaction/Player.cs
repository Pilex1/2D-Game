using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;

namespace Game {
    class Player : Rectangle {

        public static bool[] Keys { get; private set; }
        public static bool[] Mouse { get; private set; }
        public static int MouseX { get; private set; }
        public static int MouseY { get; private set; }
        public const int W = 0, A = 1, S = 2, D = 3, Space = 4;
        public const int Left = 0, Middle = 1, Right = 2;

        public const int StartX = 500, StartY = 0;

        public static Player Instance { get; private set; }

        private static bool Flying = false;

        private Player() : base(new Vector2(1, 2), new Vector2(StartX,StartY), new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 1, 1) }, PolygonMode.Fill, 10) {
            Speed = 0.5f;
        }

        public static void Init() {
            Keys = new bool[100];
            Mouse = new bool[3]; 

            Instance = new Player();
            Instance.CorrectTerrainCollision();

            Healthbar.Init(20);
            Inventory.Init();

            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutMouseFunc(OnMousePress);
            Glut.glutMotionFunc(OnMouseMove);
            Glut.glutMouseWheelFunc(OnMouseScroll);
        }

        public override void Update() {
            if (Keys[A]) {
                Instance.MoveLeft();
                Terrain.UpdatePosition = true;
                Terrain.UpdateLighting = true;
            }
            if (Keys[D]) {
                Instance.MoveRight();
                Terrain.UpdatePosition = true;
                Terrain.UpdateLighting = true;
            }
            if (Flying) {
                UseGravity = false;
                if (Keys[W]) {
                    Instance.MoveUp();
                    Terrain.UpdatePosition = true;
                    Terrain.UpdateLighting = true;
                }
                if (Keys[S]) {
                    Instance.MoveDown();
                    Terrain.UpdatePosition = true;
                    Terrain.UpdateLighting = true;
                }
            } else {
                UseGravity = true;
                if (Keys[W]) {
                    Instance.MoveUp();
                    Terrain.UpdatePosition = true;
                    Terrain.UpdateLighting = true;
                } else {
                    Falling = true;
                }
                if (Instance.MoveDown()) {
                    Terrain.UpdatePosition = true;
                    Terrain.UpdateLighting = true;
                }
            }

            if (Mouse[Left]) {
                Vector2 v = RayCast(MouseX, MouseY);
                Terrain.BreakTile((int)v.x, (int)v.y);
                Terrain.UpdatePosition = true;
                Terrain.UpdateLighting = true;
            }
            if (Mouse[Right]) {
                Vector2 v = RayCast(MouseX, MouseY);
                Terrain.SetTile((int)v.x, (int)v.y, Tile.Sand);
                Terrain.UpdatePosition = true;
                Terrain.UpdateLighting = true;
            }
            Hitbox.Position = Position;
        }

        public static void Damage(float hp) {
            Healthbar.Damage(hp);
        }

        public static void Heal(float hp) {
            Healthbar.Heal(hp);
        }

        private static void OnMouseScroll(int button, int dir, int x, int y) {

        }

        private static void OnMouseMove(int x, int y) {
            MouseX = x;
            MouseY = y;
        }

        private static void OnMousePress(int button, int state, int mx, int my) {
            if (button == Glut.GLUT_LEFT_BUTTON) {
                Mouse[Left] = (state == Glut.GLUT_DOWN);
            }
            if (button == Glut.GLUT_RIGHT_BUTTON) {
                Mouse[Right] = (state == Glut.GLUT_DOWN);
            }

            MouseX = mx;
            MouseY = my;
        }

        private static Vector2 RayCast(int mx, int my) {
            float x = (2.0f * mx) / Program.Width - 1.0f;
            float y = 1.0f - (2.0f * my) / Program.Height;
            Vector2 normalizedCoords = new Vector2(x, y);
            Vector4 clipCoords = new Vector4(normalizedCoords.x, normalizedCoords.y, -1, 1);
            Vector4 eyeCoords = Renderer.projectionMatrix.Inverse() * clipCoords;
            eyeCoords.z = -1;
            eyeCoords.w = 0;
            Matrix4 inverseViewMatrix = Renderer.viewMatrix.Inverse();
            Vector4 rayWorldTemp = inverseViewMatrix * eyeCoords;
            Vector2 rayWorld = new Vector2(rayWorldTemp.x, rayWorldTemp.y);

            Vector2 intersectTerrain = Instance.Position - rayWorld * Renderer.zoom - new Vector2(0, 1);
            return intersectTerrain;
        }

        private static void OnKeyboardDown(byte key, int x, int y) {
            if (key == 'w') Keys[W] = true;
            if (key == 'a') Keys[A] = true;
            if (key == 's') Keys[S] = true;
            if (key == 'd') Keys[D] = true;
            if (key == ' ') Keys[Space] = true;
            if (key == 'f') Flying = !Flying;
            if (key == 'r') Renderer.RenderWireframe.Toggle();
            if (key == 'e') GameLogic.RemoveAllEntities();
            if (key == 27) Glut.glutLeaveMainLoop();
        }
        private static void OnKeyboardUp(byte key, int x, int y) {
            if (key == 'w') Keys[W] = false;
            if (key == 'a') Keys[A] = false;
            if (key == 's') Keys[S] = false;
            if (key == 'd') Keys[D] = false;
            if (key == ' ') Keys[Space] = false;
        }

        public static Vector2 ToPlayer(Vector2 pos) { return new Vector2(Instance.Position.x - pos.x, Instance.Position.y - pos.y).Normalize(); }

        public static bool Intersecting(Entity entity) { return Instance.Hitbox.Intersecting(entity.Hitbox); }

    }
}
