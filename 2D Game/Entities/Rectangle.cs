using System;
using OpenGL;
using Game.Entities;

namespace Game {
    abstract class Rectangle : Entity {

        public Rectangle(Vector2 size, Vector2 position, Vector4[] colours, Hitbox hitbox, float jumpPowerMax) : base(position, CalculateModel(size, colours), hitbox, 0, jumpPowerMax) {
        }

        public Rectangle(Vector2 size, Vector2 position, Vector4[] colours) : this(size, position, colours, new RectangularHitbox(position, size ),0) { }

        private static Model CalculateModel(Vector2 size, Vector4[] colours) {
            Model model = new ColouredModel(CalculateVertices(size), new VBO<int>(new int[] {
                0,1,2,3
            },BufferTarget.ElementArrayBuffer), new VBO<Vector4>(colours), BeginMode.TriangleStrip);
            return model;
        }

        private static VBO<Vector2> CalculateVertices(Vector2 size) { return new VBO<Vector2>(new Vector2[] { new Vector2(0, size.y), new Vector2(0, 0), new Vector2(size.x, size.y), new Vector2(size.x, 0) }); }


        public void Resize(Vector2 size) {
            Model.Vertices = CalculateVertices(size);
            Hitbox.Width = size.x;
            Hitbox.Height = size.y;
        }
    }
}
