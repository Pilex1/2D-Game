using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;
using Game.Util;
using Game.Terrains.Gen;

namespace Game.Core {

    [Serializable]
    class PlayerData : EntityData {
        public BoolSwitch flying = new BoolSwitch(false, 10);
        public Tuple<Item, uint>[,] items = null;
        public int slot = 0;
    }

    class Player : Entity {

        public const float StartX = TerrainGen.size / 2, StartY = 200;

        public static Player Instance { get; private set; }

        private Player(Vector2 position) : base(EntityID.PlayerSimple, position) {
            data = new PlayerData { };
            data.pos.val = position;
            data.speed = 0.08f;
            data.jumppower = 0.5f;
            data.life = new BoundedFloat(20, 0, 20);
        }

        public override void InitTimers() {
            ((PlayerData)data).flying.AddTimer();
        }

        public static void CreateNew() {
            Instance = new Player(new Vector2(StartX, StartY));
        }

        public static void LoadPlayer(PlayerData data) {
            CreateNew();
            Instance.data = data;
        }

        public static void CleanUp() {
            PlayerData playerdata = (PlayerData)Instance.data;
            playerdata.items = Inventory.Items;
            playerdata.slot = Hotbar.CurSelectedSlot;
        }

        public override void UpdateHitbox() {
            hitbox.Position = data.pos.val;
        }

        public override void Update() {
            Vector2 prevpos = Instance.data.pos;
            PlayerData playerdata = (PlayerData)data;
            bool[] Keys = Input.Keys;
            bool[] Mouse = Input.Mouse;
            int dir = Input.MouseScroll;
            int MouseX = Input.MouseX, MouseY = Input.MouseY;
            if (!Inventory.toggle) {
                Vector2 v = Input.TerrainIntersect();

                Tile tile = Terrain.TileAt(v.x, v.y);
                GameLogic.AdditionalDebugText = tile.ToString() + Environment.NewLine + tile.tileattribs.ToString();

                if (Mouse[Input.MouseLeft]) {
                    Terrain.BreakTile((int)v.x, (int)v.y);
                }
                if (Mouse[Input.MouseRight]) {
                    int x = (int)v.x, y = (int)v.y;
                    ItemInteract.Interact(Hotbar.CurrentlySelectedItem(), x, y);
                    Terrain.TileAt(x, y).tileattribs.Interact(x, y);
                }
                if (Mouse[Input.MouseMiddle]) {
                    int x = (int)v.x, y = (int)v.y;
                }
            }
            if (Keys['a']) {
                Instance.MoveLeft();
            }
            if (Keys['d']) {
                Instance.MoveRight();
            }
            if (Keys['w']) {
                Instance.Jump();
            }
            if (playerdata.flying) {
                Instance.data.useGravity = false;
                if (Keys['s']) Instance.Fall();
            } else {
                Instance.data.useGravity = true;
            }
            if (Keys['f']) {
                playerdata.flying.Toggle();
            }

            Instance.UpdatePosition();
            if (Instance.data.pos != prevpos) {
                Terrain.UpdateMesh = true;
            }
            if (playerdata.flying) {
                Instance.data.vel.y = 0;
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

            if (dir < 0) Hotbar.IncrSlot();
            if (dir > 0) Hotbar.DecrSlot();

            if (Input.SpecialKeys[Glut.GLUT_KEY_F1]) {
                GameGuiRenderer.RenderDebugText.Toggle();
            }


            if (IsStuck()) {
                Debug.WriteLine("Player stuck! Position: " + data.pos + " Velocity: " + data.vel);
                CorrectTerrainCollision();
            }




        }

        public static float DistToPlayer(Vector2 pos) {
            float x = Instance.data.pos.x - pos.x;
            float y = Instance.data.pos.y - pos.y;
            return (float)Math.Sqrt(x * x + y * y);
        }
        public static Vector2 ToPlayer(Vector2 pos) { return new Vector2(Instance.data.pos.x - pos.x, Instance.data.pos.y - pos.y).Normalize(); }

        public static bool InRange(Entity entity, float maxDist) {
            float x = entity.data.pos.x, y = entity.data.pos.y;
            return (Instance.data.pos.x - x) * (Instance.data.pos.x - x) + (Instance.data.pos.y - y) * (Instance.data.pos.y - y) <= maxDist;
        }

        public static bool Intersecting(Entity entity) {
            return Instance.hitbox.Intersecting(entity.hitbox);
        }

        public override void OnDeath() {
            Instance.HealFull();
        }
    }
}
