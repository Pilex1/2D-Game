using System;
using OpenGL;
using Game.Entities;

namespace Game {
    abstract class Entity {
        public Model Model { get; protected set; }
        public Hitbox Hitbox { get; protected set; }

        public float Speed { get; protected set; }

        protected bool CorrectCollisions;
        protected bool UseGravity;

        private float JumpPowerMax;
        private float JumpPower = 0;
        private float dy = 0;
        private const float gravity = 0.02f;

        private Vector2 position = new Vector2(0, 0);
        public Vector2 Position {
            get { return position; }
            set {
                if (value.x < 0) value.x = 0;
                if (value.x > Terrain.MaxWidth) value.x = Terrain.MaxWidth;
                position = value;
            }
        }

        protected Entity(Vector2 position, Model model, Hitbox hitbox, float speed, float jumpPowerMax, bool correctCollisions = true, bool useGravity = true) {
            Model = model;
            Hitbox = hitbox;
            Speed = speed;
            CorrectCollisions = correctCollisions;
            UseGravity = useGravity;
            Position = position;
            JumpPowerMax = jumpPowerMax;
            if (CorrectCollisions) CorrectTerrainCollision();
        }

        public virtual Matrix4 ModelMatrix() => Matrix4.CreateTranslation(new Vector3(Position.x, Position.y, 0));

        public void MoveLeft() {
            if (Terrain.WillCollide(this, new Vector2(-1,0))) Position = new Vector2((int)Position.x, Position.y);
            else Position += new Vector2(-Speed, 0);
        }

        public void MoveRight() {
            if (Terrain.WillCollide(this, new Vector2(1, 0))) Position = new Vector2((int)Position.x, Position.y);
            else Position += new Vector2(Speed, 0); 
        }

        public void MoveUp() {
            if (UseGravity) {

            }else {
                dy = 0;
                if (Terrain.WillCollide(this, new Vector2(0, 1))) Position = new Vector2(Position.x, (int)Position.y);
                else Position += new Vector2(0, Speed);
            }
           
        }

        public void MoveDown() {
            Vector2 offset;
            if (UseGravity) {
                dy += gravity;
                offset = new Vector2(0, -dy);
            }else {
                offset = new Vector2(0, -Speed);
            }

            if (Terrain.WillCollide(this, offset)) {
                dy = 0;
                Position = new Vector2(Position.x, (int)(Position.y));
            } else {
                Position += offset;
            }
        }

        public bool InAir() => !Terrain.WillCollide(this, new Vector2(0, -Speed));

        protected void CorrectTerrainCollision() {
            Position = Terrain.CorrectTerrainCollision(this);
        }

        public abstract void Update();
    }
}
