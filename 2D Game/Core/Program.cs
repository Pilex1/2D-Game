using System;
using OpenGL;
using Tao.FreeGlut;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Game.Util;
using System.Linq;
using Game.Core;
using Game.Fonts;
using Game.TitleScreen;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;
using Game.Core.World_Serialization;
using Game.Entities;

namespace Game {

    enum ProgramMode {
        TitleScreen, Game
    }
    static class Program {

        public static Random Rand = new Random();

        public static int ScreenWidth, ScreenHeight;
        public static int Width = 1280, Height = 720;
        //public static int Width = 1600, Height = 900;
        public static float AspectRatio = (float)Width / Height;

        public static bool FullScreen { get; private set; }

        public static ProgramMode Mode { get; private set; }

        public static string worldname;

        private static bool ResetAgain;

        static void Main() {

            Core.World_Serialization.Serialization.GetWorlds();

            Init();

            Glut.glutMainLoop();

            Dispose();

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

        }

        private static void Init() {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(Width, Height);

            ScreenWidth = Glut.glutGet(Glut.GLUT_SCREEN_WIDTH);
            ScreenHeight = Glut.glutGet(Glut.GLUT_SCREEN_HEIGHT);

            Glut.glutInitWindowPosition((ScreenWidth - Width) / 2, (ScreenHeight - Height) / 2);
            Glut.glutCreateWindow("Plexico 2D Game - Copyright Alex Tan 2016");
            //Glut.glutGameModeString(Width+"x"+Height+":32@60");

            Glut.glutSetOption(Glut.GLUT_ACTION_ON_WINDOW_CLOSE, Glut.GLUT_ACTION_CONTINUE_EXECUTION);
            Gl.Viewport(0, 0, Width, Height);
            Glut.glutReshapeFunc(OnReshape);
            Glut.glutDisplayFunc(delegate () { });
            Glut.glutIdleFunc(MainGameLoop);

            //Console.SetWindowSize(Console.LargestWindowWidth / 4, Console.LargestWindowHeight / 4);
            //Console.SetWindowPosition(0, 0);
            Mode = ProgramMode.TitleScreen;

            AssetsManager.Init();
            Input.Init();
            Gui.Init();
            SwitchToTitleScreen();
        }

        private static void OnReshape(int width, int height) {
            Width = width;
            Height = height;
            AspectRatio = (float)Width / height;
        }

        //todo
        public static void EnterFullScreen() {
            Glut.glutEnterGameMode();
            FullScreen = true;
        }

        //todo
        public static void ExitFullScreen() {
            Glut.glutLeaveGameMode();
            FullScreen = false;
        }


        public static void SwitchToTitleScreen() {
            Mode = ProgramMode.TitleScreen;
            GameRenderer.CleanUp();
            Gui.SwitchToTitleScreen();
        }


        public static void SwitchToGame(string worldname, int seed) {
            Mode = ProgramMode.Game;
            Program.worldname = worldname;

            GameRenderer.InitNew(seed);
            AssetsManager.InitInGame();
            GameLogic.InitNew();
            Gui.SwitchToGame();
            ResetAgain = true;
        }

        public static void SwitchToGame(WorldData world) {
            Mode = ProgramMode.Game;

            GameRenderer.InitLoad(world.terrain, world.entities);
            AssetsManager.InitInGame();
            GameLogic.InitLoad();
            Gui.SwitchToGame();
            ResetAgain = true;
        }


        private static void MainGameLoop() {
            Gl.ClearColor(1, 1, 1, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ErrorCode error = Gl.GetError();
            Debug.Assert(error == ErrorCode.NoError);

            if (ResetAgain) {
                GameTime.Update();
                ResetAgain = false;
            }

            GameTime.Update();


            if (Mode == ProgramMode.Game)
                GameGuiRenderer.RenderBackground();


            if (Mode == ProgramMode.Game) {
                GameRenderer.Render();
                GameLogic.Update();
            }

            Gui.Render();
            Gui.Update();

            CooldownTimer.Update();


            Input.Update();

          //  Thread.Sleep((int)((float)1000 / 20));

            Glut.glutSwapBuffers();
        }

        private static void Dispose() {
            GameRenderer.CleanUp();
            if (Mode == ProgramMode.Game) {
                TerrainData worlddata = new TerrainData(Terrain.Tiles, Terrain.TerrainBiomes);
                EntitiesData entitydata = new EntitiesData((PlayerData)Player.Instance.data, Entity.GetAllEntities());

                Serialization.SaveWorld(worldname, worlddata, entitydata);
            }

            Gui.Dispose();
        }

    }
}
