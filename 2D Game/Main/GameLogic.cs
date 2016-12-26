using OpenGL;
using Game.Entities;
using Game.Terrains;
using Game.Core;
using Game.Util;
using Game.Interaction;
using System.Text;
using Game.Core.World_Serialization;
using Game.Items;
using Tao.FreeGlut;
using Game.Fluids;
using System;

namespace Game {


    static class GameLogic {

        internal enum GameState {
            Normal, Paused, Inventory, Text
        }

        public static BoolSwitch RenderDebugText { get; private set; }
        public static string AdditionalDebugText = "";

        public static GameState State = GameState.Normal;
        private static bool StateChanged_Escape;
        private static bool StateChanged_Enter;

        public static BoolSwitch RenderHitboxes { get; private set; }



        public static void InitNew(int seed) {

            Terrain.CreateNew(seed);
            Terrain.Init();

            EntityManager.Init();
            Player.CreateNew();
            EntityManager.AddEntity(Player.Instance);
            Player.Instance.CorrectTerrainCollision();
            PlayerInventory.Init();
            PlayerInventory.Instance.LoadDefaultItems();


            //for (int i = 1; i <= 300; i++) {
            //    Shooter s = new Shooter(new Vector2(i * 20 * MathUtil.RandFloat(Program.Rand, 0.8f, 1.2f), 0), 100, 250);
            //    EntityManager.AddEntity(s);
            //    s.CorrectTerrainCollision();

            //    Squisher sq = new Squisher(new Vector2(i * 20 * MathUtil.RandFloat(Program.Rand, 0.8f, 1.2f), 0));
            //    EntityManager.AddEntity(sq);
            //    sq.CorrectTerrainCollision();
            //}




            Init();
        }



        public static void InitLoad(TerrainData worlddata, EntitiesData entitiesdata) {

            #region Terrain
            Terrain.Load(worlddata.terrain);
            Terrain.Init();
            #endregion

            #region Entities
            EntityManager.Init();

            //load other entities
            foreach (var e in entitiesdata.entities) {
                EntityManager.AddEntity(e);
                e.InitTimers();
            }
            #endregion

            #region Player
            //load player data
            Player.LoadPlayer(entitiesdata.player);
            EntityManager.AddEntity(Player.Instance);

            //load player inventory
            PlayerInventory.Init();
            PlayerInventory.Instance.LoadItems(entitiesdata.playerItems);
            #endregion


            Init();
        }

        private static void Init() {
            RenderDebugText = new BoolSwitch(false, 30);
            RenderHitboxes = new BoolSwitch(false, 30);
        }

        public static void Update() {

            if (Input.SpecialKeys[Glut.GLUT_KEY_F1]) {
                RenderDebugText.Toggle();
            }

            if (Input.SpecialKeys[Glut.GLUT_KEY_F2]) {
                RenderHitboxes.Toggle();
            }

            if (Input.Keys['e']) {
                if (State == GameState.Normal) {
                    PlayerInventory.Instance.InventoryOpen.val = true;
                    State = GameState.Inventory;
                }
            }
            if (Input.Keys[13]) {
                if (!StateChanged_Enter) {
                    if (State == GameState.Normal) {
                        GameGuiRenderer.TxtInput.disabled = false;
                        State = GameState.Text;
                    } else if (State == GameState.Text) {
                        GameGuiRenderer.TxtInput.Execute();
                        GameGuiRenderer.TxtInput.disabled = true;
                        State = GameState.Normal;
                    }
                    StateChanged_Enter = true;
                }
            } else {
                StateChanged_Enter = false;
            }

            if (Input.Keys[27]) {
                if (!StateChanged_Escape) {
                    switch (State) {
                        case GameState.Normal:
                            State = GameState.Paused;
                            break;
                        case GameState.Paused:
                            State = GameState.Normal;
                            break;
                        case GameState.Inventory:
                            PlayerInventory.Instance.InventoryOpen.val = false;
                            State = GameState.Normal;
                            break;
                        case GameState.Text:
                            GameGuiRenderer.TxtInput.disabled = true;
                            State = GameState.Normal;
                            break;
                    }
                    StateChanged_Escape = true;
                }
            } else {
                StateChanged_Escape = false;
            }

            GameGuiRenderer.SetDebugText(DebugText());

            switch (State) {
                case GameState.Inventory:
                    EntityManager.UpdateAll();
                    Terrain.Update();
                    PlayerInventory.Instance.UpdateHotbar();
                    PlayerInventory.Instance.UpdateInventory();
                    break;
                case GameState.Normal:
                    EntityManager.UpdateAll();
                    Terrain.Update();
                    PlayerInventory.Instance.UpdateHotbar();
                    PlayerInventory.Instance.UpdateInventory();
                    break;
                case GameState.Paused:
                case GameState.Text:
                    break;
            }
        }

        private static string DebugText() {
            string brk = "--------------";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Debug");
            sb.AppendLine(brk);
            sb.AppendLine(GameTime.FPS + " FPS / " + string.Format("{0:0.0000}", 1000f / GameTime.FPS) + " ms");
            sb.AppendLine("Loaded Entities: " + EntityManager.LoadedEntities);
            sb.AppendLine("Logic tiles: " + Terrain.LogicDict.Keys.Count);
            sb.AppendLine("Fluid tiles: " + FluidManager.GetFluidCount());
            sb.AppendLine(brk);
            Vector2 playerpos = Player.Instance.data.pos.val;
            Vector2 playervel = Player.Instance.data.vel.val;
            sb.AppendLine("Position: " + string.Format("{0:0.0000}, {1:0.0000}", playerpos.x, playerpos.y));
            sb.AppendLine("Velocity: " + string.Format("{0:0.0000}, {1:0.0000}", playervel.x, playervel.y));
            sb.AppendLine(brk);
            sb.AppendLine(AdditionalDebugText);
            return sb.ToString();
        }

        public static void CleanUp() {
            if (Program.Mode != ProgramMode.Game) return;
            Terrain.CleanUp();
            EntityManager.CleanUp();
            FluidManager.CleanUp();
        }


        internal static void Reset() {
            if (Program.Mode != ProgramMode.Game) return;
            StateChanged_Enter = StateChanged_Escape = false;
            State = GameState.Normal;
            RenderHitboxes.val = false;
            RenderDebugText.val = false;
            AdditionalDebugText = "";
        }

    }
}
