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

                Tile tile = Terrain.TileAt(v.x, v.y);
                GameLogic.AdditionalDebugText = tile.ToString() + Environment.NewLine + tile.tileattribs.ToString();

                var p = PlayerInventory.Instance;

                if (Mouse[Input.MouseLeft]) {
                    Tile t = Terrain.BreakTile((int)v.x, (int)v.y);
                    switch (t.enumId) {
                        case TileEnum.Invalid:
                        case TileEnum.Air:
                        case TileEnum.Bedrock:
                            break;

                        case TileEnum.Grass: p.AddItem(ItemID.Grass); break;
                        case TileEnum.Sand: p.AddItem(ItemID.Sand); break;
                        case TileEnum.Dirt: p.AddItem(ItemID.Dirt); break;
                        case TileEnum.Wood: p.AddItem(ItemID.Wood); break;
                        case TileEnum.Leaf: p.AddItem(ItemID.Leaf); break;
                        case TileEnum.Stone: p.AddItem(ItemID.Stone); break;
                        case TileEnum.Tnt: p.AddItem(ItemID.Tnt); break;
                        case TileEnum.Sandstone: p.AddItem(ItemID.Sandstone); break;
                        case TileEnum.Sapling: p.AddItem(ItemID.Sapling); break;
                        case TileEnum.Brick: p.AddItem(ItemID.Brick); break;
                        case TileEnum.Metal1: p.AddItem(ItemID.Metal1); break;
                        case TileEnum.SmoothSlab: p.AddItem(ItemID.SmoothSlab); break;
                        case TileEnum.WeatheredStone: p.AddItem(ItemID.WeatheredStone); break;
                        case TileEnum.FutureMetal: p.AddItem(ItemID.FutureMetal); break;
                        case TileEnum.Marble: p.AddItem(ItemID.Marble); break;
                        case TileEnum.PlexSpecial: p.AddItem(ItemID.PlexSpecial); break;
                        case TileEnum.PurpleStone: p.AddItem(ItemID.PurpleStone); break;
                        case TileEnum.Nuke: p.AddItem(ItemID.Nuke); break;
                        case TileEnum.Cactus: p.AddItem(ItemID.Cactus); break;
                        case TileEnum.Bounce: p.AddItem(ItemID.Bounce); break;
                        case TileEnum.Water: break;
                        case TileEnum.WireOn: case TileEnum.WireOff: p.AddItem(ItemID.Wire); break;
                        case TileEnum.SwitchOn: case TileEnum.SwitchOff: p.AddItem(ItemID.Switch); break;
                        case TileEnum.LogicLampUnlit: case TileEnum.LogicLampLit: p.AddItem(ItemID.LogicLamp); break;
                        case TileEnum.Snow: p.AddItem(ItemID.Snow); break;
                        case TileEnum.SnowWood: p.AddItem(ItemID.SnowWood); break;
                        case TileEnum.SnowLeaf: p.AddItem(ItemID.SnowLeaf); break;
                        case TileEnum.GrassDeco: p.AddItem(ItemID.GrassDeco); break;
                        case TileEnum.GateAnd: p.AddItem(ItemID.GateAnd); break;
                        case TileEnum.GateOr: p.AddItem(ItemID.GateOr); break;
                        case TileEnum.GateNot: p.AddItem(ItemID.GateNot); break;
                        case TileEnum.WireBridgeOff: case TileEnum.WireBridgeHorzVertOn: case TileEnum.WireBridgeHorzOn: case TileEnum.WireBridgeVertOn: p.AddItem(ItemID.WireBridge); break;
                        case TileEnum.TilePusherOff: case TileEnum.TilePusherOn: p.AddItem(ItemID.StickyTilePusher); break;
                        case TileEnum.TilePullerOn: case TileEnum.TilePullerOff: p.AddItem(ItemID.StickyTilePuller); break;
                        case TileEnum.Light: p.AddItem(ItemID.Light); break;
                        case TileEnum.Accelerator: p.AddItem(ItemID.Accelerator); break;
                        case TileEnum.SingleTilePusherOff: case TileEnum.SingleTilePusherOn: p.AddItem(ItemID.SingleTilePusher); break;
                    }
                }
                if (Mouse[Input.MouseRight]) {
                    int x = (int)v.x, y = (int)v.y;
                    ItemInteract.Interact(PlayerInventory.Instance.CurrentlySelectedItem(), x, y);
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
