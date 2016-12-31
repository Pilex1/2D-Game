using Game.Assets;
using Game.Core;
using Game.Entities;
using Game.Terrains;
using OpenGL;

namespace Game {
    static class GameRenderer {

        private const float FOV = 0.45f, Near = 0.1f, Far = 1000f;
        public const float zoom = -100;
        public static Matrix4 projectionMatrix { get; private set; }
        public static Matrix4 viewMatrix { get; private set; }

        public static void Init() {
            EntityManager.LoadShaders();
            Terrain.LoadShaders();

            AssetsManager.InitInGame();
        }

        private static void UpdateProjectionMatrix() {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, Program.AspectRatio, Near, Far);
            Terrain.SetProjectionMatrix(projectionMatrix);
            EntityManager.SetProjectionMatrix(projectionMatrix);
        }

        private static void UpdateViewMatrix() {
            viewMatrix = Matrix4.CreateTranslation(new Vector3(-Player.Instance.data.pos.x, -Player.Instance.data.pos.y, zoom));

            Terrain.UpdateViewMatrix(viewMatrix);
            EntityManager.UpdateViewMatrix(viewMatrix);
        }

        public static void Render() {
            //Gl.Enable(EnableCap.DepthTest);
            //Gl.DepthFunc(DepthFunction.Less);

            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);

            UpdateViewMatrix();
            UpdateProjectionMatrix();

            Terrain.Render();

            EntityManager.Render();
        }



    }
}
