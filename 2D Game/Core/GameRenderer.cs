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

        public static void Init(WorldData worlddata) {

            //terrain
            if (worlddata == null) {
                Terrain.CreateNew();
            } else {
                Terrain.Load(worlddata.terrain);
            }
            Terrain.Init();

            //entities
            Entity.Init();
            if (worlddata == null) {
                Player.CreateNew();
            } else {
                Player.LoadPlayer(worlddata.playerdata);
               // Entity.Load(worlddata.entities);
            }
            Entity.AddEntity(Player.Instance);

            if (worlddata==null)
                Player.Instance.CorrectTerrainCollision();

            PlayerData playerdata = (PlayerData)Player.Instance.data;
            Inventory.Init(playerdata.items);



            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, Program.AspectRatio, Near, Far);

            //Load matrices
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
