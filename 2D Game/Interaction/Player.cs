using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;
using Game.Util;

namespace Game {
    class Player : Rectangle {

        public static bool[] Keys { get; private set; }
        public static bool[] Mouse { get; private set; }
        public static int MouseX { get; private set; }
        public static int MouseY { get; private set; }
        public const int Left = 0, Middle = 1, Right = 2;

        public const int StartX = 580, StartY = 70;

        private static BoolSwitch Flying = true;

        public static Player Instance { get; private set; }

        private Player() : base(new Vector2(1, 2), new Vector2(StartX, StartY), new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 1, 1) }, PolygonMode.Fill, 0.5f) {
            Speed = 0.08f;
        }

        public static void Init() {
            Keys = new bool[255];
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
            if (Keys['a']) {
                Instance.MoveLeft();
            }
            if (Keys['d']) {
                Instance.MoveRight();
            }
            if (Keys['w']) {
                Instance.Jump();
            }
            if (Flying) {
                Instance.UseGravity = false;
                if (Keys['s']) Instance.Fall();
            } else {
                Instance.UseGravity = true;
            }

            if (Instance.UpdatePosition()) {
                Terrain.UpdateMesh = true;
            }


            if (Keys['1']) Hotbar.CurSelectedSlot = 0;
            if (Keys['2']) Hotbar.CurSelectedSlot = 1;
            if (Keys['3']) Hotbar.CurSelectedSlot = 2;
            if (Keys['4']) Hotbar.CurSelectedSlot = 3;
            if (Keys['5']) Hotbar.CurSelectedSlot = 4;
            if (Keys['6']) Hotbar.CurSelectedSlot = 5;
            if (Keys['7']) Hotbar.CurSelectedSlot = 6;
            if (Keys['8']) Hotbar.CurSelectedSlot = 7;
            if (Keys['9']) Hotbar.CurSelectedSlot = 8;


            if (Mouse[Left]) {
                if (Hotbar.CurrentlySelectedItem() == ItemId.None) {
                    Vector2 v = RayCast(MouseX, MouseY);
                    Terrain.BreakTile((int)v.x, (int)v.y);
                }
            }
            if (Mouse[Right]) {
                Vector2 v = RayCast(MouseX, MouseY);
                int x = (int)v.x, y = (int)v.y;

                if (Hotbar.CurrentlySelectedItem() == ItemId.None) {
                    Tile t = Terrain.TileAt(x, y);
                    if (t is IRightInteractable) {
                        ((IRightInteractable)t).Interact();
                    }
                } else {
                    ItemInteract.Interact(Hotbar.CurrentlySelectedItem(), x, y);
                }
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
            if (dir < 0) Hotbar.IncrSlot();
            if (dir > 0) Hotbar.DecrSlot();
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
            Vector4 eyeCoords = GameRenderer.projectionMatrix.Inverse() * clipCoords;
            eyeCoords.z = -1;
            eyeCoords.w = 0;
            Matrix4 inverseViewMatrix = GameRenderer.viewMatrix.Inverse();
            Vector4 rayWorldTemp = inverseViewMatrix * eyeCoords;
            Vector2 rayWorld = new Vector2(rayWorldTemp.x, rayWorldTemp.y);

            Vector2 intersectTerrain = Instance.Position - rayWorld * GameRenderer.zoom - new Vector2(0, 1);
            return intersectTerrain;
        }

        private static void OnKeyboardDown(byte key, int x, int y) {
            Keys[key] = true;


            if (key == 'l') {
                Terrain.UpdateLighting.Toggle();
                Terrain.UpdateMesh = true;
            }
            if (key == 'f') Flying.Toggle();
            if (key == 'e') GameLogic.RemoveAllEntities();
            if (key == 27) Glut.glutLeaveMainLoop();
        }
        private static void OnKeyboardUp(byte key, int x, int y) {
            Keys[key] = false;
        }

        public static Vector2 ToPlayer(Vector2 pos) { return new Vector2(Instance.Position.x - pos.x, Instance.Position.y - pos.y).Normalize(); }

        public static bool InRange(Entity entity, float maxDist) {
            float x = entity.Position.x, y = entity.Position.y;
            return (Instance.Position.x - x) * (Instance.Position.x - x) + (Instance.Position.y - y) * (Instance.Position.y - y) <= maxDist;
        }

        public static bool Intersecting(Entity entity) {
            return Instance.Hitbox.Intersecting(entity.Hitbox);
        }



    }
}
