using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;

namespace Game.Core {
   static class GameTime {
        private static Stopwatch Watch = new Stopwatch();
        public static float DeltaTime { get; private set; }
        private static float ActualDeltaTime;
        private static float PrevTime;
        private static float CountTime;
        private static int Count;

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
                Glut.glutSetWindowTitle("Plexico 2D Game - Copyright Alex Tan 2016 FPS: " + (Count * interval).ToString());
                CountTime = 0;
                Count = 0;
            }
        }
    }
}
