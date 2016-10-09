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
            //for (int i = 1; i <= 500; i++) {
            //    new Shooter(new Vector2(i * 2, 0), 50, 150);
            //}
        }

        public static void Update() {

            Entity.UpdateAll();
            Player.Heal(0.0005f * GameTime.DeltaTime);

            Terrain.Update();
        }
    }
}
