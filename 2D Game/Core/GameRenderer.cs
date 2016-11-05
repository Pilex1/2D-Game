using Game.Interaction;
using Game.Particles;
using Game.Terrains;
using OpenGL;

namespace Game {
    static class GameRenderer {

        private const float FOV = 0.45f, Near = 0.1f, Far = 1000f;
        public const float zoom = -100;
        public static Matrix4 projectionMatrix { get; private set; }
        public static Matrix4 viewMatrix { get; private set; }

        public static void InitNew(int seed) {
            Terrain.CreateNew(seed);
            Terrain.Init();

            Entity.Init();
            Player.CreateNew();
            Entity.AddEntity(Player.Instance);
            Player.Instance.CorrectTerrainCollision();

            InitMatrices();
        }

        public static void InitLoad(WorldData worlddata) {

            //terrain
            Terrain.Load(worlddata.terrain);
            Terrain.Init();

            //entities
            Entity.Init();
            Player.LoadPlayer(worlddata.playerdata);
            // Entity.Load(worlddata.entities);
            Entity.AddEntity(Player.Instance);

            //Load matrices  
            InitMatrices();

        }

        private static void InitMatrices() {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, Program.AspectRatio, Near, Far);
            LoadProjectionMatrix();
            UpdateViewMatrix();
        }

        private static void LoadProjectionMatrix() {
            Terrain.SetProjectionMatrix(projectionMatrix);
            Entity.SetProjectionMatrix(projectionMatrix);
        }

        private static void UpdateViewMatrix() {
            viewMatrix = Matrix4.CreateTranslation(new Vector3(-Player.Instance.data.Position.x, -Player.Instance.data.Position.y, zoom));

            Terrain.UpdateViewMatrix(viewMatrix);
            Entity.UpdateViewMatrix(viewMatrix);
        }

        public static void Render() {
            //Gl.Enable(EnableCap.DepthTest);
            //Gl.DepthFunc(DepthFunction.Less);

            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);

            UpdateViewMatrix();

            Entity.Render();
            Terrain.Render();
        }


        public static void CleanUp() {
            if (Program.Mode != ProgramMode.Game) return;
            Terrain.CleanUp();
            Entity.CleanUp();
            Player.CleanUp();
        }
    }
}
