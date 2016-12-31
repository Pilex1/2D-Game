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

using Game.Terrains.Logics;
using Game.Terrains.Fluids;
using Game.Terrains.Lighting;
using Game.Terrains.Terrain_Generation;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;

namespace Game {


    static class GameLogic {

        internal enum GameState {
            Normal, Paused, Inventory, Text
        }

        public static Switch<LightingManager.LightingOption> LightingOption { get; private set; }
        public static BoolSwitch RenderDebugText { get; private set; }
        public static string AdditionalDebugText = "";

        public static CancellationTokenSource cancelWorldLoading;
        public static Task taskWorldLoading;
        public static bool saving = false;

        public static GameState State = GameState.Normal;
        private static bool StateChanged_Escape;
        private static bool StateChanged_Enter;

        public static BoolSwitch RenderHitboxes { get; private set; }


        private static void InitBefore() {
            RenderDebugText = new BoolSwitch(false, 30);
            RenderHitboxes = new BoolSwitch(false, 30);
            LightingOption = new Switch<LightingManager.LightingOption>(LightingManager.LightingOption.Smooth, 30);
            cancelWorldLoading = new CancellationTokenSource();

            FluidManager.Init();
            LogicManager.Init();
        }


        public static void InitNew(int seed) {
            InitBefore();

            #region Terrain

            Terrain.Init();

            Terrain.CreateNew(seed);

            LightingManager.CalcFromNew();

            #endregion

            #region Entities

            EntityManager.Init();
            Player.CreateNew();
            EntityManager.AddEntity(Player.Instance);
            Player.Instance.CorrectTerrainCollision();
            PlayerInventory.Init();
            PlayerInventory.Instance.LoadDefaultItems();

            for (int i = 0; i <= 300; i++) {
                Shooter s = new Shooter(new Vector2(MathUtil.RandFloat(Program.Rand, 0, TerrainGen.SizeX - 1), 0), 100, 250);
                EntityManager.AddEntity(s);
                s.CorrectTerrainCollision();

                Squisher sq = new Squisher(new Vector2(MathUtil.RandFloat(Program.Rand, 0, TerrainGen.SizeX - 1), 0));
                EntityManager.AddEntity(sq);
                sq.CorrectTerrainCollision();
            }

            #endregion


        }



        public static void InitLoad(string world) {
            InitBefore();

            #region Terrain

            Terrain.Init();


            #endregion

            #region Entities
            var entitiesdata = Serialization.LoadEntities(world);

            EntityManager.Init();

            //load other entities
            foreach (var e in entitiesdata.entities) {
                EntityManager.AddEntity(e);
                e.InitTimers();
            }

            //load player data
            Player.LoadPlayer(entitiesdata.player);
            EntityManager.AddEntity(Player.Instance);

            //load player inventory
            PlayerInventory.Init();
            PlayerInventory.Instance.LoadItems(entitiesdata.playerItems);
            #endregion


            //load chunk where the player is in and adjacent chunks
            float playerx = Player.Instance.data.pos.x;
            int playerchunk = Terrain.GetChunkAt(playerx);
            Terrain.LoadChunk(Serialization.LoadChunk(world, playerchunk));
            if (playerchunk - 1 >= 0)
                Terrain.LoadChunk(Serialization.LoadChunk(world, playerchunk - 1));
            if (playerchunk + 1 < TerrainGen.ChunksPerWorld)
                Terrain.LoadChunk(Serialization.LoadChunk(world, playerchunk + 1));

            LoadChunks(world);
        }

        private static async void LoadChunks(string world) {
            taskWorldLoading = Task.Factory.StartNew(() => {
                float playerx = Player.Instance.data.pos.x;
                int playerchunk = Terrain.GetChunkAt(playerx);
                int ptr1 = playerchunk - 2;
                int ptr2 = playerchunk + 2;
                while (ptr1 >= 0 || ptr2 < TerrainGen.ChunksPerWorld) {
                    if (cancelWorldLoading.Token.IsCancellationRequested) {
                        Debug.WriteLine("World loading cancelled");
                        return;
                    }

                    if (ptr1 >= 0)
                        Terrain.LoadChunk(Serialization.LoadChunk(world, ptr1));
                    if (ptr2 < TerrainGen.ChunksPerWorld)
                        Terrain.LoadChunk(Serialization.LoadChunk(world, ptr2));
                    ptr1--;
                    ptr2++;
                }
            }, cancelWorldLoading.Token);
            await taskWorldLoading;
        }

        public static void Update() {
            #region Debug
            if (Input.SpecialKeys[Glut.GLUT_KEY_F1]) {
                RenderDebugText.Toggle();
            }

            if (Input.SpecialKeys[Glut.GLUT_KEY_F2]) {
                RenderHitboxes.Toggle();
            }

            if (Input.SpecialKeys[Glut.GLUT_KEY_F3]) {
                switch (LightingOption.Get()) {
                    case LightingManager.LightingOption.None:
                        LightingOption.Set(LightingManager.LightingOption.Jagged);
                        break;
                    case LightingManager.LightingOption.Jagged:
                        LightingOption.Set(LightingManager.LightingOption.Averaged);
                        break;
                    case LightingManager.LightingOption.Averaged:
                        LightingOption.Set(LightingManager.LightingOption.Smooth);
                        break;
                    case LightingManager.LightingOption.Smooth:
                        LightingOption.Set(LightingManager.LightingOption.None);
                        break;
                }
            }
            GameGuiRenderer.SetDebugText(DebugText());
            #endregion

            if (Input.Keys['e']) {
                if (State == GameState.Normal) {
                    PlayerInventory.Instance.InventoryOpen.ForceSet(true);
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
                            PlayerInventory.Instance.InventoryOpen.ForceSet(false);
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


            switch (State) {
                case GameState.Inventory:
                    EntityManager.UpdateAll();
                    Terrain.Update();
                    GameTime.GuiTimer.Start();
                    PlayerInventory.Instance.UpdateHotbar();
                    PlayerInventory.Instance.UpdateInventory();
                   GameTime.GuiTimer.Pause();
                    break;
                case GameState.Normal:
                    EntityManager.UpdateAll();
                    Terrain.Update();
                   // GameTime.GuiTimer.Start();
                    PlayerInventory.Instance.UpdateHotbar();
                  //  GameTime.GuiTimer.Pause();
                    break;
                case GameState.Paused:
                case GameState.Text:
                    break;
            }
        }

        private static string DebugText() {
            string brk = "--------------";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Terrain Rendering: " + StringUtil.TruncateTo(GameTime.TerrainTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("Lighting Calculations: " + StringUtil.TruncateTo(GameTime.LightingsTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("Entity Updates: " + StringUtil.TruncateTo(GameTime.EntityUpdatesTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("Entity Rendering: " + StringUtil.TruncateTo(GameTime.EntityUpdatesTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("Fluid Updates: " + StringUtil.TruncateTo(GameTime.FluidsTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("Logic Updates: " + StringUtil.TruncateTo(GameTime.LogicTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("GUI Updates & Rendering: " + StringUtil.TruncateTo(GameTime.GuiTimer.ElaspedTime, 4) + " ms");
            sb.AppendLine("Total: " + StringUtil.TruncateTo(1000f / GameTime.FPS, 4) + " ms / " + GameTime.FPS + " FPS");
            sb.AppendLine(brk);

            sb.AppendLine("Lighting: " + LightingOption.Get());
            sb.AppendLine("Loaded Entities: " + EntityManager.LoadedEntities);
            sb.AppendLine("Logic tiles: " + LogicManager.Instance.GetCount());
            sb.AppendLine("Fluid tiles: " + FluidManager.Instance.GetCount());
            sb.AppendLine(brk);

            Vector2 playerpos = Player.Instance.data.pos.val;
            Vector2 playervel = Player.Instance.data.vel.val;
            MovementState playermvt = Player.Instance.data.mvtState;
            sb.AppendLine("Position: " + StringUtil.TruncateTo(playerpos.x, 4) + ", " + StringUtil.TruncateTo(playerpos.y, 4));
            sb.AppendLine("Velocity: " + StringUtil.TruncateTo(playervel.x, 4) + ", " + StringUtil.TruncateTo(playervel.y, 4));
            sb.AppendLine("Movement state: " + playermvt.ToString());
            sb.AppendLine(brk);

            sb.AppendLine(AdditionalDebugText);
            return sb.ToString();
        }




        public static async void SaveWorld() {
            saving = true;

            //cancel world loading and wait until it finishes before saving world
            cancelWorldLoading.Cancel();
            try {
                taskWorldLoading.Wait();
            } catch (Exception) { } finally {
                cancelWorldLoading.Dispose();
            }
            await Task.Factory.StartNew(() => {
                ChunkData[] chunks = Terrain.GetChunkData();
                EntitiesData entitydata = new EntitiesData(Player.Instance.data, PlayerInventory.Instance.Items, EntityManager.GetAllEntities());
                Serialization.SaveWorld(Program.worldname, chunks, entitydata, FluidManager.Instance.GetDict(), LogicManager.Instance.GetDict());
                saving = false;
            });
        }

        public static void CleanUp() {
            if (Program.Mode != ProgramMode.Game) return;
            SaveWorld();
            Terrain.CleanUp();
            EntityManager.CleanUp();
            FluidManager.Instance.CleanUp();
            LogicManager.Instance.CleanUp();
        }


        internal static void Reset() {
            if (Program.Mode != ProgramMode.Game) return;
            StateChanged_Enter = StateChanged_Escape = false;
            State = GameState.Normal;
            RenderHitboxes.ForceSet(false);
            RenderDebugText.ForceSet(false);
            AdditionalDebugText = "";
        }

    }
}
