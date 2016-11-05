using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;
using Game.Util;
using Game.Core;

namespace Game {

    [Serializable]
    class PlayerData : EntityData {
        public BoolSwitch flying = new BoolSwitch(false, 10);
        public Tuple<Item, uint>[,] items = null;
        public int slot = 0;
    }

    class Player : Entity {
        public const float StartX = TerrainGen.size/2, StartY = 200;

        public static Player Instance { get; private set; }

        private Player(Hitbox hitbox, Vector2 position) : base(EntityID.PlayerSimple, hitbox, position) {
            base.data = new PlayerData { };
            base.data.Position.val = position;
            base.data.speed = 0.08f;
            base.data.jumppower = 0.5f;
            base.data.life = new BoundedFloat(20, 0, 20);
        }

        private static Player DefaultPlayer() {

            Texture texture = TextureUtil.CreateTexture(new Vector3[,] {
              {new Vector3(1, 0, 0), new Vector3(0, 1, 0)},
              {new Vector3(0, 0, 1), new Vector3(1, 0, 1)}
            }, TextureUtil.TextureInterp.Linear);

            Vector2 position = new Vector2(StartX, StartY);
            Hitbox hitbox = new RectangularHitbox(position, new Vector2(1, 2));
            Player player = new Player(hitbox, position);
            return player;
        }

        public static void CreateNew() {
            Instance = DefaultPlayer();
        }

        public static void LoadPlayer(PlayerData data) {
            Instance = DefaultPlayer();
            Instance.data = data;
            data.flying.AddTimer();
        }

        public static new void CleanUp() {
            PlayerData playerdata = (PlayerData)Instance.data;
            playerdata.items = Inventory.Items;
            playerdata.slot = Hotbar.CurSelectedSlot;
            //Serialization.SavePlayer((PlayerData)Instance.data);
        }

        public override void Update() {
            Vector2 prevpos = Instance.data.Position;
            if (Instance.data.life <= 0) {
                Instance.data.life.Fill();
            }
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
                Instance.data.UseGravity = false;
                if (Keys['s']) Instance.Fall();
            } else {
                Instance.data.UseGravity = true;
            }
            if (Keys['f']) {
                playerdata.flying.Toggle();
            }

            Instance.UpdatePosition();
            if (Instance.data.Position != prevpos) {
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

            Hitbox.Position = data.Position.val;

        }

        public static float DistToPlayer(Vector2 pos) {
            float x = Instance.data.Position.x - pos.x;
            float y = Instance.data.Position.y - pos.y;
            return (float)Math.Sqrt(x * x + y * y);
        }
        public static Vector2 ToPlayer(Vector2 pos) { return new Vector2(Instance.data.Position.x - pos.x, Instance.data.Position.y - pos.y).Normalize(); }

        public static bool InRange(Entity entity, float maxDist) {
            float x = entity.data.Position.x, y = entity.data.Position.y;
            return (Instance.data.Position.x - x) * (Instance.data.Position.x - x) + (Instance.data.Position.y - y) * (Instance.data.Position.y - y) <= maxDist;
        }

        public static bool Intersecting(Entity entity) {
            return Instance.Hitbox.Intersecting(entity.Hitbox);
        }


    }
}
