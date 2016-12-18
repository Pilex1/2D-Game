using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Template {
    class Entity {
        public Model model;

        public Vector3 pos;
        public Vector3 scale;
        public Vector3 rot;

        public Entity(Model model, Vector3 pos, Vector3 scale, Vector3 rot) {
            this.model = model;
            this.pos = pos;
            this.scale = scale;
            this.rot = rot;
        }

        public Matrix GetModelMatrix() {
            return Matrix.CreateScale(scale) * Matrix.CreateRotationX(rot.X) * Matrix.CreateRotationY(rot.Y) * Matrix.CreateRotationZ(rot.Z) * Matrix.CreateTranslation(pos);
        }



    }
}
