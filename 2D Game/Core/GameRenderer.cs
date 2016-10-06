using Game.Core;
using Game.Entities;
using Game.Fluids;
using Game.Interaction;
using Game.Terrains;
using Game.Util;
using OpenGL;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Game {
    static class GameRenderer {

        private const float FOV = 0.45f, Near = 0.1f, Far = 1000f;
        public const float zoom = -100;
        public static Matrix4 projectionMatrix { get; private set; }
        public static Matrix4 viewMatrix { get; private set; }

        public static void Init() {
            //GL Stuff
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);
            Gl.ClearColor(0.1f, 0.2f, 0.6f, 1);

            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, (float)Program.Width / Program.Height, Near, Far);

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
            Entity.UpdateViewMatrix(viewMatrix);
            Terrain.UpdateViewMatrix(viewMatrix);
        }

        public static void Render() {
            UpdateViewMatrix();

            Entity.Render();
            Terrain.Render();
        }


        public static void CleanUp() {
            Terrain.CleanUp();
            Entity.CleanUp();
        }
    }
}
