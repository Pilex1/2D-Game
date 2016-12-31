using System.Diagnostics;

namespace Game.Core {
    static class GameTime {

        public static Timer TerrainTimer;
        public static Timer LightingsTimer;
        public static Timer EntityUpdatesTimer;
        public static Timer EntityRenderTimer;
        public static Timer LogicTimer;
        public static Timer FluidsTimer;
        public static Timer GuiTimer;

        public static float TotalTime { get; private set; }
        public static float DeltaTime { get { return deltaTime / 1000 * 60; } }
        public static int FPS { get; private set; }

        private static float deltaTime;
        private static float prevTime;
        private static float countTime;
        private static int count;
        private static Stopwatch watch;

        public static void Init() {
            watch = new Stopwatch();
            TerrainTimer = new Timer();
            LightingsTimer = new Timer();
            EntityUpdatesTimer = new Timer();
            EntityRenderTimer = new Timer();
            LogicTimer = new Timer();
            FluidsTimer = new Timer();
            GuiTimer = new Timer();
        }

        public static void Update() {
            watch.Stop();
            deltaTime = watch.ElapsedMilliseconds - prevTime;
            prevTime = watch.ElapsedMilliseconds;
            watch.Start();

            countTime += deltaTime;
            count++;
            int updateinterval = 1;
            if (countTime > 1000f / updateinterval) {
                FPS = count * updateinterval;
                countTime -= 1000f / updateinterval;
                count = 0;

                TerrainTimer.Calculate();
                LightingsTimer.Calculate();
                EntityUpdatesTimer.Calculate();
                EntityRenderTimer.Calculate();
                LogicTimer.Calculate();
                FluidsTimer.Calculate();
                GuiTimer.Calculate();
            }
            TotalTime += DeltaTime;
        }
    }

    class Timer {

        public float ElaspedTime { get; private set; }

        private Stopwatch watch;
        private float countTime;
        private int count;

        public Timer() {
            watch = new Stopwatch();
        }

        public void Start() {
            watch.Start();
        }

        public void Pause() {
            watch.Stop();
        }

        public void Stop() {
            watch.Stop();
            count++;
            countTime += watch.ElapsedMilliseconds;
            watch.Reset();
        }

        internal void Calculate() {
            ElaspedTime = countTime / count;
            countTime = 0;
            count = 0;
        }

    }
}
