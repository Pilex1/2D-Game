using Game.Entities;
using Game.Items;
using Game.Terrains;
using Game.Terrains.Lighting;
using Game.Terrains.Terrain_Generation;
using Game.Util;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Core {

    class Player : Entity {

        public static Player Instance { get; private set; }

        public Vector2i SelectedTilePos { get; private set; }
        public bool CanPlaceTile { get; private set; }

        private Player() : base(EntityID.PlayerSimple, new Vector2(TerrainGen.SizeX / 2, 0), new Vector2(1, 2)) {
            data.speed = 0.10f;
            data.jumppower = 0.5f;
            data.life = new BoundedFloat(20, 0, 20);
        }
        private Player(EntityData data) : base(EntityID.PlayerSimple, data) { }

        public static void CreateNewPlayer() {
            Instance = new Player();
        }

        public static void LoadPlayer(EntityData data) {
            Instance = new Player(data);
        }

        private void HandleItemUsage() {
            if (!PlayerInventory.Instance.InventoryOpen) {
                Vector2 v = Input.TerrainIntersect();
                int ix = (int)v.x;
                int iy = (int)v.y;
                SelectedTilePos = new Vector2i((int)v.x, (int)v.y);
                CanPlaceTile = Terrain.HasNeighbouringSolidTiles(SelectedTilePos) && EntityManager.GetEntitiesAt(SelectedTilePos.ToVector2()).Length == 0;

                Tile tile = Terrain.TileAt(SelectedTilePos);
                if (tile == null) return;
                if (EntityManager.GetEntitiesAt(new Vector2(ix, iy)).Length == 0) {

                    if (Input.MouseDown(MouseButton.RightButton)) {
                        Terrain.TileAt(SelectedTilePos.x, SelectedTilePos.y).tileattribs.OnInteract(SelectedTilePos.x, SelectedTilePos.y);
                        PlayerInventory.Instance.CurrentlySelectedItem().rawitem.attribs.Use(PlayerInventory.Instance, new Vector2i(PlayerInventory.Instance.CurSelectedSlot, 0), new Vector2(SelectedTilePos.x, SelectedTilePos.y), Input.RayCast());
                    }
                    if (Input.MouseDown(MouseButton.LeftButton)) {
                        tile.tileattribs.Destroy(SelectedTilePos.x, SelectedTilePos.y, PlayerInventory.Instance);
                    }
                }

                var lighting = LightingManager.GetLighting(SelectedTilePos.x, SelectedTilePos.y);
                GameLogic.AdditionalDebugText = tile.ToString() + Environment.NewLine + tile.tileattribs.ToString() + "Position: " + SelectedTilePos.x + ", " + SelectedTilePos.y + Environment.NewLine + "Lighting: Hue " + StringUtil.TruncateTo(lighting.Hue, 4) + " Saturation " + StringUtil.TruncateTo(lighting.Saturation, 4) + " Brightness " + StringUtil.TruncateTo(lighting.Brightness, 4);

            }
        }

        private void HandleKeys() {
            if (Input.KeyDown(Key.A)) Instance.MoveLeft();
            if (Input.KeyDown(Key.D)) Instance.MoveRight();
            if (Input.KeyDown(Key.W)) Instance.Jump();
            if (Input.KeyDown(Key.S)) Instance.Fall();
        }

        public override void Update() {
            Heal(0.002f * GameTime.DeltaTime);

            HandleItemUsage();
            HandleKeys();

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
