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

namespace Game {
    static class GameLogic {

        public static void Init() {
            //new Shooter(new Vector2(500, 0), 50, 150);
            for (int i = 1; i <= 1000; i++) {
                Shooter s = new Shooter(new Vector2(i * 10 * MathUtil.RandFloat(Program.Rand, 0.8f, 1.2f), 0), 100, 250);
                Entity.AddEntity(s);
            }
        }

        public static void Update() {

            Entity.UpdateAll();
            Player.Heal(0.002f * GameTime.DeltaTime);

            Terrain.Update();

            GameGuiRenderer.SetDebugText("Position: " + Player.Instance.data.Position.val);
        }
    }
}
