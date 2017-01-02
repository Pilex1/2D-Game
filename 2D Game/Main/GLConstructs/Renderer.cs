using Game.Assets;
using Game.Core;
using Game.Entities;
using Game.Terrains;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace Game {
    static class GameRenderer {

        private const float FOV = 0.45f, Near = 0.1f, Far = 1000f;
        public const float zoom = -100;
        public static Matrix projectionMatrix { get; private set; }
        public static Matrix viewMatrix { get; private set; }

        public static void Init() {
            EntityManager.LoadShaders();
            Terrain.LoadShaders();

            AssetsManager.InitInGame();
        }

        private static void UpdateProjectionMatrix() {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(FOV, Program.AspectRatio, Near, Far);
            Terrain.SetProjectionMatrix(projectionMatrix);
            EntityManager.SetProjectionMatrix(projectionMatrix);
        }

        private static void UpdateViewMatrix() {
            viewMatrix = Matrix.CreateTranslation(new Vector3(-Player.Instance.data.pos.x, -Player.Instance.data.pos.y, zoom));

            Terrain.UpdateViewMatrix(viewMatrix);
            EntityManager.UpdateViewMatrix(viewMatrix);
        }

        public static void Render() {
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            UpdateViewMatrix();
            UpdateProjectionMatrix();

            Terrain.Render();

            EntityManager.Render();
        }



    }
}
