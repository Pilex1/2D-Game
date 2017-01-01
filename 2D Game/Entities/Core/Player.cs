using System;
using Pencil.Gaming.MathUtils;
using Game.Entities;
using Game.Terrains;
using Game.Util;
using Game.Terrains.Terrain_Generation;
using Game.Items;
using Game.Terrains.Lighting;
using Pencil.Gaming;

namespace Game.Core {

    class Player : Entity {

        public static Player Instance { get; private set; }

        private Player() : base(EntityID.PlayerSimple, new Vector2(TerrainGen.SizeX / 2, 0)) {
            data.speed = 0.10f;
            data.jumppower = 0.5f;
            data.life = new BoundedFloat(20, 0, 20);
        }

        public static void CreateNew() {
            Instance = new Player();
        }

        public static void LoadPlayer(EntityData data) {
            CreateNew();
            Instance.data = data;
        }

        public override void Update() {

            Heal(0.002f * GameTime.DeltaTime);

            if (!PlayerInventory.Instance.InventoryOpen) {
                Vector2 v = Input.TerrainIntersect();
                Vector2i vi = new Vector2i((int)v.x, (int)v.y);

                Tile tile = Terrain.TileAt(vi);
                if (tile != null) {
                    var lighting = LightingManager.GetLighting(vi.x, vi.y);
                    GameLogic.AdditionalDebugText = tile.ToString() + Environment.NewLine + tile.tileattribs.ToString() + "Position: " + vi.x + ", " + vi.y + Environment.NewLine + "Lighting: Red " + StringUtil.TruncateTo(lighting.x, 4) + " Blue " + StringUtil.TruncateTo(lighting.y, 4) + " Green " + StringUtil.TruncateTo(lighting.z, 4);

                    if (Input.MouseDown(MouseButton.LeftButton)) {
                        tile.tileattribs.Destroy(vi.x, vi.y, PlayerInventory.Instance);
                    }
                    if (Input.MouseDown(MouseButton.RightButton)) {
                        PlayerInventory.Instance.CurrentlySelectedItem().rawitem.attribs.Use(PlayerInventory.Instance, new Vector2i(PlayerInventory.Instance.CurSelectedSlot, 0), new Vector2(vi.x, vi.y), Input.RayCast());
                        Terrain.TileAt(vi.x, vi.y).tileattribs.OnInteract(vi.x, vi.y);
                    }
                    if (Input.MouseDown(MouseButton.MiddleButton)) {

                    }
                }

            }
            if (Input.KeyDown(Key.A)) Instance.MoveLeft();
            if (Input.KeyDown(Key.D)) Instance.MoveRight();
            if (Input.KeyDown(Key.W)) Instance.Jump();
            if (Input.KeyDown(Key.S)) Instance.Fall();

            Instance.UpdatePosition();
        }

        public static float DistToPlayer(Vector2 pos) {
            float x = Instance.data.pos.x - pos.x;
            float y = Instance.data.pos.y - pos.y;
            return (float)Math.Sqrt(x * x + y * y);
        }
        public static Vector2 ToPlayer(Vector2 pos) {
            Vector2 v = new Vector2(Instance.data.pos.x - pos.x, Instance.data.pos.y - pos.y);
            v.Normalize();
            return v;
        }

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
