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
using Game.Fluids;
using Game.Particles;
using Game.Guis;

namespace Game.Core {

    [Serializable]
    class PlayerData : EntityData {
        public BoolSwitch flying = new BoolSwitch(false, 10);
        public Item[,] items = null;
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
            playerdata.items = PlayerInventory.Instance.Items;
            playerdata.slot = PlayerInventory.Instance.CurSelectedSlot;
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
            if (!PlayerInventory.Instance.InventoryOpen) {
                Vector2 v = Input.TerrainIntersect();
                Vector2i vi = new Vector2i((int)v.x, (int)v.y);

                Tile tile = Terrain.TileAt(vi);
                GameLogic.AdditionalDebugText = tile.ToString() + Environment.NewLine + tile.tileattribs.ToString();


                if (Mouse[Input.MouseLeft]) {
                    PlayerInventory.Instance.CurrentlySelectedItem().rawitem.attribs.BreakTile(PlayerInventory.Instance,vi);
                }
                if (Mouse[Input.MouseRight]) {
                    PlayerInventory.Instance.CurrentlySelectedItem().rawitem.attribs.Use(PlayerInventory.Instance, new Vector2i(PlayerInventory.Instance.CurSelectedSlot, 0), new Vector2(vi.x, vi.y));
                    Terrain.TileAt(vi.x, vi.y).tileattribs.Interact(vi.x, vi.y);
                }
                if (Mouse[Input.MouseMiddle]) {

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

            if (Keys['1']) PlayerInventory.Instance.CurSelectedSlot = 0;
            if (Keys['2']) PlayerInventory.Instance.CurSelectedSlot = 1;
            if (Keys['3']) PlayerInventory.Instance.CurSelectedSlot = 2;
            if (Keys['4']) PlayerInventory.Instance.CurSelectedSlot = 3;
            if (Keys['5']) PlayerInventory.Instance.CurSelectedSlot = 4;
            if (Keys['6']) PlayerInventory.Instance.CurSelectedSlot = 5;
            if (Keys['7']) PlayerInventory.Instance.CurSelectedSlot = 6;
            if (Keys['8']) PlayerInventory.Instance.CurSelectedSlot = 7;
            if (Keys['9']) PlayerInventory.Instance.CurSelectedSlot = 8;

            if (dir < 0) PlayerInventory.Instance.IncreaseHotbarSelection();
            if (dir > 0) PlayerInventory.Instance.DecreaseHotbarSelection();

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
