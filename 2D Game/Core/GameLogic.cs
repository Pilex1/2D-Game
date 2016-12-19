using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using Game.Core;
using Game.Util;
using Game.Interaction;
using System.Text;
using Game.Core.World_Serialization;
using Game.Guis;
using Game.Items;
using System.Diagnostics;
using Tao.FreeGlut;

namespace Game {
    static class GameLogic {

        public static BoolSwitch RenderDebugText { get; private set; }
        public static string AdditionalDebugText = "";

        public static BoolSwitch Paused;
        private static bool PausedFlag;

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


            for (int i = 1; i <= 300; i++) {
                Shooter s = new Shooter(new Vector2(i * 20 * MathUtil.RandFloat(Program.Rand, 0.8f, 1.2f), 0), 100, 250);
                EntityManager.AddEntity(s);
                s.CorrectTerrainCollision();

                Squisher sq = new Squisher(new Vector2(i * 20 * MathUtil.RandFloat(Program.Rand, 0.8f, 1.2f), 0));
                EntityManager.AddEntity(sq);
                sq.CorrectTerrainCollision();
            }




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
            Paused = new BoolSwitch(false);

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


            if (Input.Keys[27] && !PlayerInventory.Instance.InventoryOpen) {
                if (!PausedFlag) {
                    Paused.Toggle();
                    PausedFlag = true;
                }
            } else {
                PausedFlag = false;
            }
            if (Paused) return;



            EntityManager.UpdateAll();
            Terrain.Update();
            PlayerInventory.Instance.Update();
            GameGuiRenderer.SetDebugText(DebugText());
        }

        private static string DebugText() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Debug - ");
            sb.AppendLine("FPS: " + GameTime.FPS);
            Vector2 playerpos = Player.Instance.data.pos.val;
            Vector2 playervel = Player.Instance.data.vel.val;
            sb.AppendLine("Position: " + string.Format("{0:0.0000}, {1:0.0000}", playerpos.x, playerpos.y));
            sb.AppendLine("Velocity: " + string.Format("{0:0.0000}, {1:0.0000}", playervel.x, playervel.y));
            sb.AppendLine("Loaded Entities: " + EntityManager.LoadedEntities);
            sb.AppendLine("--------------");
            sb.AppendLine(AdditionalDebugText);
            return sb.ToString();
        }

    }
}
