using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using Game.Core;
using Game.Util;
using Game.Interaction;
using System.Text;
using Game.Core.World_Serialization;

namespace Game {
    static class GameLogic {

        public static string AdditionalDebugText = "";

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
            PlayerData playerdata = (PlayerData)Player.Instance.data;
            PlayerInventory.Init();
            PlayerInventory.Instance.LoadItems(playerdata.items);
            #endregion

        }

        public static void Update() {

            EntityManager.UpdateAll();
            Player.Instance.Heal(0.002f * GameTime.DeltaTime);

            Terrain.Update();

            GameGuiRenderer.SetDebugText(DebugText());
        }

        private static string DebugText() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Debug - ");
            sb.AppendLine("FPS: " + GameTime.FPS);
            Vector2 playerpos = Player.Instance.data.pos.val;
            Vector2 playervel = Player.Instance.data.vel.val;
            sb.AppendLine("Position: " + String.Format("{0:0.0000}, {1:0.0000}", playerpos.x, playerpos.y));
            sb.AppendLine("Velocity: " + String.Format("{0:0.0000}, {1:0.0000}", playervel.x, playervel.y));
            sb.AppendLine("Loaded Entities: " + EntityManager.LoadedEntities);
            sb.AppendLine("--------------");
            sb.AppendLine(AdditionalDebugText);
            return sb.ToString();
        }

    }
}
