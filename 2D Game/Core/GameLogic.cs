using System;
using System.Collections.Generic;
using OpenGL;
using Tao.FreeGlut;
using System.Diagnostics;
using Game.Entities;
using Game.Terrains;
using Game.Core;
using System.Threading;
using Game.Util;
using Game.TitleScreen;
using Game.Interaction;
using Game.Particles;
using System.Text;

namespace Game {
    static class GameLogic {

       
        public static void Init() {

            //new Shooter(new Vector2(500, 0), 50, 150);
            //for (int i = 1; i <= 1000; i++) {
            //    Shooter s = new Shooter(new Vector2(i * 10 * MathUtil.RandFloat(Program.Rand, 0.8f, 1.2f), 0), 100, 250);
            //    Entity.AddEntity(s);
            //}
        }


        public static void Update() {

            Entity.UpdateAll();
            Player.Instance.Heal(0.002f * GameTime.DeltaTime);

            Terrain.Update();

            GameGuiRenderer.SetDebugText(DebugText());
        }

        private static string DebugText() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Debug - ");
            sb.AppendLine("FPS: " + GameTime.FPS);
            Vector2 playerpos = Player.Instance.data.Position.val;
            Vector2 playervel = Player.Instance.data.vel.val;
            sb.AppendLine("Position: " + String.Format("{0:0.0000}, {1:0.0000}", playerpos.x, playerpos.y));
            sb.AppendLine("Velocity: " + String.Format("{0:0.0000}, {1:0.0000}", playervel.x, playervel.y));
            sb.AppendLine("Loaded Entities: " + Entity.LoadedEntities);
            return sb.ToString();
        }
    }
}
