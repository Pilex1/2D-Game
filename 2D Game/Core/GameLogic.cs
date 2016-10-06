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

namespace Game {
    static class GameLogic {
        private static Stopwatch Watch = new Stopwatch();
        public static float DeltaTime { get; private set; }
        private static float ActualDeltaTime;
        private static float PrevTime;
        private static float CountTime;
        private static int Count;

        public static void Init() {

            Entity.Init();
            Player.Init();
            GameRenderer.Init();

            for (int i = 1; i <= 1; i++) {
              new Shooter(new Vector2(475, 0), 50, 150);
            }
        }

        public static void Update() {
            Watch.Stop();
            ActualDeltaTime = (Watch.ElapsedMilliseconds - PrevTime) / 1000;
            PrevTime = Watch.ElapsedMilliseconds;
            Watch.Start();

            DeltaTime = ActualDeltaTime * 60;
            CountTime += ActualDeltaTime;
            Count++;
            int interval = 1;
            if (CountTime > 1f / interval) {
                Glut.glutSetWindowTitle("Plexico 2D Game - Copyright Alex Tan 2016 FPS: " + (Count * interval).ToString() + " Pos: " + (int)Player.Instance.data.Position.x + ", " + (int)Player.Instance.data.Position.y);
                CountTime = 0;
                Count = 0;
            }

            CooldownTimer.Update();

            //update the player movement
            Player.Instance.Update();
            Player.Heal(0.0005f * DeltaTime);
            //Debug.WriteLine(Player.Instance.Position);



            //update terrain
            Terrain.Update();


        }

        public static void Render() {
            GameRenderer.Render();
        }

        public static void Dispose() {
            if (Program.Mode != ProgramMode.Game) return;

            Update();
            Terrain.CleanUp();
            Entity.CleanUp();
            Player.CleanUp();

        }
    }
}
