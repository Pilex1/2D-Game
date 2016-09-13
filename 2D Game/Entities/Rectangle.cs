using System;
using OpenGL;
using Game.Entities;

namespace Game {
    abstract class Rectangle : Entity {

        public Rectangle(Vector2 size, Vector2 position, Vector4[] colours, PolygonMode polyMode, Hitbox hitbox, float jumpPowerMax) : base(position, ColouredModel.CreateRectangle(size,colours,polyMode), hitbox, 0, jumpPowerMax) {
        }

        public Rectangle(Vector2 size, Vector2 position, Vector4[] colours, PolygonMode polyMode, float jumpPowerMax) : this(size, position, colours, polyMode, new RectangularHitbox(position, size), jumpPowerMax) { }

        public void Resize(Vector2 size) {
            Model.Vertices = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, size.y),
                new Vector2(0, 0),
                new Vector2(size.x, size.y),
                new Vector2(size.x, 0)
            });
            Hitbox.Width = size.x;
            Hitbox.Height = size.y;
        }
    }
}
