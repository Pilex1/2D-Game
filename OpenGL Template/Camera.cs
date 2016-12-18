using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    static class Camera {

        public const float FOV = 0.45f, Near = 0.1f, Far = 1000f;

        public static Vector3 pos;
        private static Vector3 _rot;
        public static Vector3 rot { get { return _rot; } set {
                _rot = value;
                if (_rot.X > Math.PI / 2) _rot.X = (float)Math.PI / 2;
                if (_rot.X < -Math.PI / 2) _rot.X = (float)-Math.PI / 2;
                if (_rot.Y > Math.PI / 2) _rot.Y = (float)Math.PI / 2;
                if (_rot.Y < -Math.PI / 2) _rot.Y = (float)-Math.PI / 2;
            } }

        private static Matrix projectionMatrix;

        public static void Init(Vector3 pos, Vector3 rot) {
            Camera.pos = pos;
            Camera.rot = rot;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(FOV, (float)Program.width / Program.height, Near, Far);
        }

        public static void MoveUp(float amt) {
            pos.Z -= amt;
        }

        public static void MoveDown(float amt) {
            pos.Z += amt;
        }

        public static void MoveForward(float amt) {
            pos.X += amt * (float)Math.Sin(rot.X);
            pos.Y -= amt * (float)Math.Cos(rot.X);
        }

        public static void MoveBackward(float amt) {
            pos.X += amt * (float)Math.Sin(rot.X);
            pos.Y -= amt * (float)Math.Cos(rot.X);
        }

        public static void MoveLeft(float amt) {
            pos.X -= amt * (float)Math.Sin(Math.PI / 2 + rot.X);
            pos.Y += amt * (float)Math.Cos(Math.PI / 2 + rot.X);
        }

        public static void MoveRight(float amt) {
            pos.X += amt * (float)Math.Sin(Math.PI / 2 + rot.X);
            pos.Y -= amt * (float)Math.Cos(Math.PI / 2 + rot.X);
        }

        public static Matrix GetProjectionMatrix() {
            return projectionMatrix;
        }

        public static Matrix GetViewMatrix() {
            return Matrix.CreateTranslation(pos) * Matrix.CreateRotationX(rot.X) * Matrix.CreateRotationY(rot.Z) ;
        }

    }
}
