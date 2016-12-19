using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;
using Game.Interaction;
using Game.Terrains;
using Game.Util;
using Game.Terrains.Gen;
using Game.Guis;
using Game.Items;

namespace Game.Core {

    class Player : Entity {

        public const float StartX = TerrainGen.size / 2, StartY = 200;

        public static Player Instance { get; private set; }

        private Player(Vector2 position) : base(EntityID.PlayerSimple, position) {
            data = new EntityData { };
            data.pos.val = position;
            data.speed = 0.08f;
            data.jumppower = 0.5f;
            data.life = new BoundedFloat(20, 0, 20);
        }

        public override void InitTimers() {

        }

        public static void CreateNew() {
            Instance = new Player(new Vector2(StartX, StartY));
        }

        public static void LoadPlayer(EntityData data) {
            CreateNew();
            Instance.data = data;
        }

        public override void Update() {

            Heal(0.002f * GameTime.DeltaTime);

            Vector2 prevpos = Instance.data.pos;
            int MouseX = Input.MouseX, MouseY = Input.MouseY;
            if (!PlayerInventory.Instance.InventoryOpen) {
                Vector2 v = Input.TerrainIntersect();
                Vector2i vi = new Vector2i((int)v.x, (int)v.y);

                Tile tile = Terrain.TileAt(vi);
                GameLogic.AdditionalDebugText = tile.ToString() + Environment.NewLine + tile.tileattribs.ToString();

                if (Input.Mouse[Input.MouseLeft]) {
                    PlayerInventory.Instance.CurrentlySelectedItem().rawitem.attribs.BreakTile(PlayerInventory.Instance, vi);
                }
                if (Input.Mouse[Input.MouseRight]) {
                    PlayerInventory.Instance.CurrentlySelectedItem().rawitem.attribs.Use(PlayerInventory.Instance, new Vector2i(PlayerInventory.Instance.CurSelectedSlot, 0), new Vector2(vi.x, vi.y), Input.RayCast());
                    Terrain.TileAt(vi.x, vi.y).tileattribs.Interact(vi.x, vi.y);
                }
                if (Input.Mouse[Input.MouseMiddle]) {

                }
            }
            if (Input.Keys['a']) Instance.MoveLeft();
            if (Input.Keys['d']) Instance.MoveRight();
            if (Input.Keys['w']) Instance.Jump();
            if (Input.Keys['s']) Instance.Fall();

            Instance.UpdatePosition();
            if (Instance.data.pos != prevpos) {
                Terrain.UpdateMesh = true;
            }

            if (Input.Keys['1']) PlayerInventory.Instance.CurSelectedSlot = 0;
            if (Input.Keys['2']) PlayerInventory.Instance.CurSelectedSlot = 1;
            if (Input.Keys['3']) PlayerInventory.Instance.CurSelectedSlot = 2;
            if (Input.Keys['4']) PlayerInventory.Instance.CurSelectedSlot = 3;
            if (Input.Keys['5']) PlayerInventory.Instance.CurSelectedSlot = 4;
            if (Input.Keys['6']) PlayerInventory.Instance.CurSelectedSlot = 5;
            if (Input.Keys['7']) PlayerInventory.Instance.CurSelectedSlot = 6;
            if (Input.Keys['8']) PlayerInventory.Instance.CurSelectedSlot = 7;
            if (Input.Keys['9']) PlayerInventory.Instance.CurSelectedSlot = 8;

            if (Input.MouseScroll < 0)
                PlayerInventory.Instance.IncreaseHotbarSelection();
            if (Input.MouseScroll > 0)
                PlayerInventory.Instance.DecreaseHotbarSelection();



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
