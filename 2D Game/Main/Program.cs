using System;
using OpenGL;
using Tao.FreeGlut;
using System.Diagnostics;
using Game.Util;
using Game.Core;
using Game.TitleScreen;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;
using Game.Core.World_Serialization;
using Game.Entities;
using Game.Items;

using Game.Terrains.Logics;
using Game.Terrains.Fluids;
using Game.Terrains.Lighting;

namespace Game {

    enum ProgramMode {
        None, TitleScreen, Game
    }
    static class Program {

        public static Random Rand = new Random();

        public static int ScreenWidth, ScreenHeight;
        public static int Width = 1280, Height = 720;
        public static float AspectRatio = (float)Width / Height;
        public static ProgramMode Mode { get; private set; }

        private static string worldname;

        static void Main() {
            Init();
            Glut.glutMainLoop();
            Dispose();
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
            Mode = ProgramMode.None;

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

        public static void SwitchToTitleScreen() {
            GameLogic.Reset();
            Mode = ProgramMode.TitleScreen;
            TitleScreenRenderer.Reset();
        }


        public static void SwitchToGame(string worldname, int seed) {
            Mode = ProgramMode.Game;
            Program.worldname = worldname;
            GameRenderer.Init();
            GameLogic.InitNew(seed);
            GameTime.Update();
        }

        public static void SwitchToGame(string worldname, WorldData world) {
            Mode = ProgramMode.Game;
            Program.worldname = worldname;
            GameRenderer.Init();
            GameLogic.InitLoad(world.terrain, world.entities);
            GameTime.Update();
        }


        private static void MainGameLoop() {
            Gl.ClearColor(1, 1, 1, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ErrorCode error = Gl.GetError();
            Debug.Assert(error == ErrorCode.NoError);

            GameTime.Update();
            CooldownTimer.Update();

            if (Mode == ProgramMode.Game) {
                GameGuiRenderer.RenderBackground();
                GameRenderer.Render();
                GameLogic.Update();
            }
            Gui.Render();
            Gui.Update();

            Input.Update();
            Glut.glutSwapBuffers();
        }

        public static void SaveWorld() {
            TerrainData worlddata = new TerrainData { terrain = Terrain.Tiles, terrainbiomes = Terrain.TerrainBiomes, fluidDict = FluidManager.Instance.GetDict(), logicDict = LogicManager.Instance.GetDict() , lightings = LightingManager.Lightings};
            EntitiesData entitydata = new EntitiesData(Player.Instance.data, PlayerInventory.Instance.Items, EntityManager.GetAllEntities());

            Serialization.SaveWorld(worldname, worlddata, entitydata);
        }

        private static void Dispose() {
            GameLogic.CleanUp();
            Gui.Dispose();


            if (Mode == ProgramMode.Game)
                SaveWorld();
        }
    }
}
