using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;

namespace Game {
    class Player : Rectangle {

        public static bool[] Keys { get; private set; } = new bool[100];
        public const int W = 0, A = 1, S = 2, D = 3;

        public static Player Instance { get; private set; }


        private static bool Flying = false;

        private Player() : base(new Vector2(1, 2), new Vector2(0, 0), new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 1, 1) }) {
            Speed = 0.5f;
        }

        public static void Init() {
            Instance = new Player();
            Instance.CorrectTerrainCollision();
            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutMouseFunc(OnMousePress);
        }

        public override void Update() {
            if (Keys[A]) Instance.MoveLeft();
            if (Keys[D]) Instance.MoveRight();
            if (Flying) {
                UseGravity = false;
                if (Keys[W]) Instance.MoveUp();
                if (Keys[S]) Instance.MoveDown();
            } else {
                UseGravity = true;
                if (Keys[W]) {
                    Instance.MoveUp();
                }else {
                    Instance.MoveDown();
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


        private static void OnMousePress(int button, int state, int mx, int my) {
            //left click
            if (button == 0 && state == 0) {
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
                int cx = (int)intersectTerrain.x, cy = (int)intersectTerrain.y;
                Tile t = Terrain.TileAt(cx, cy);
                if (t != Tile.Air) Terrain.BreakTile(cx, cy);
                else Terrain.SetTile(cx, cy, Tile.PurpleStone);
            }
        }

        private static void OnKeyboardDown(byte key, int x, int y) {
            if (key == 'w') Keys[W] = true;
            if (key == 'a') Keys[A] = true;
            if (key == 's') Keys[S] = true;
            if (key == 'd') Keys[D] = true;
            if (key == 'f') Flying = !Flying;
            if (key == 'r') Renderer.DrawingMode = Renderer.DrawingMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill;
            if (key == 'e') GameLogic.RemoveAllEntities();
            if (key == 27) Glut.glutLeaveMainLoop();
        }
        private static void OnKeyboardUp(byte key, int x, int y) {
            if (key == 'w') Keys[W] = false;
            if (key == 'a') Keys[A] = false;
            if (key == 's') Keys[S] = false;
            if (key == 'd') Keys[D] = false;
        }

        public static Vector2 ToPlayer(Vector2 pos) => new Vector2(Instance.Position.x - pos.x, Instance.Position.y - pos.y).Normalize();

        public static bool Intersecting(Entity entity) => Instance.Hitbox.Intersecting(entity.Hitbox);

    }

    static class Healthbar {

        public static float MaxHealth { get; private set; }
        public static float Health { get; private set; }
        public static int Deaths { get; private set; } = 0;
        public static bool Dead { get; private set; } = false;

        private static float BarWidth = 1, BarHeight = 0.2f;
        public static VBO<Vector2> Vertices;
        public static VBO<Vector4> Colours;
        public static VBO<int> Elements;

        public static void Init(float maxHealth) {
            Health = MaxHealth = maxHealth;

            CalculateVertices();

            Colours = new VBO<Vector4>(new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), });
            Elements = new VBO<int>(new int[] {
                0,1,2,3
            }, BufferTarget.ElementArrayBuffer);
        }

        public static void CalculateVertices() {
            Vertices = new VBO<Vector2>(new Vector2[] {
                new Vector2(0,BarHeight),
                new Vector2(0,0),
                 new Vector2(BarWidth * Health/MaxHealth,BarHeight),
                new Vector2(BarWidth * Health/MaxHealth,0)
            }, Hint: BufferUsageHint.DynamicDraw);
        }

        public static void Revive() {
            Health = MaxHealth;
        }

        public static void Heal(float hp) {
            if (!Dead) {
                Health += hp;
                if (Health > MaxHealth) Health = MaxHealth;
            }
        }

        public static void Damage(float hp) {
            Health -= hp;
            if (Health <= 0) {
                Dead = true;
                Deaths++;
            }
        }

        public static Matrix4 ModelMatrix() {
            float ratio = Health / MaxHealth;
            Vector3 playerPos = new Vector3(Player.Instance.Position.x, Player.Instance.Position.y, 0);
            //  return Matrix4.CreateTranslation(playerPos + new Vector3(-22.5, -22.5, 1)) * Matrix4.CreateScaling(new Vector3(ratio, 1, 1));
            return Matrix4.CreateTranslation(playerPos) * Matrix4.CreateScaling(new Vector3(ratio, 1, 1));
        }
    }

    static class Hotbar {

    }
}
