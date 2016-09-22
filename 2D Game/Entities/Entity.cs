﻿using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using System.Diagnostics;
using Game.Assets;

namespace Game {
    abstract class Entity {
        public Model Model { get; protected set; }
        public Hitbox Hitbox { get; protected set; }

        public float Speed { get; protected set; }

        protected bool CorrectCollisions;
        protected bool UseGravity;

        protected float JumpPowerMax;
        protected bool InAir = false;
        private float _dy = 0;
        private float _dx = 0;
        private float dy {
            get { return _dy; }
            set {
                if (value < -1 * maxVertSpeed) _dy = -1 * maxVertSpeed;
                else if (value > maxVertSpeed) _dy = maxVertSpeed;
                else _dy = value;
                if (Math.Abs(_dy) < 0.00001) _dy = 0;
            }
        }
        private float dx {
            get { return _dx; }
            set {
                if (value < -1 * maxHorzSpeed) _dx = -1 * maxHorzSpeed;
                else if (value > maxHorzSpeed) _dx = maxHorzSpeed;
                else _dx = value;
                if (Math.Abs(_dx) < 0.00001) _dx = 0;
            }
        }
        private const float maxHorzSpeed = 0.8f;
        private const float maxVertSpeed = 1f;
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

        public virtual Matrix4 ModelMatrix() { return Matrix4.CreateTranslation(new Vector3(Position.x, Position.y, 0)); }

        public void MoveLeft() {
            dx -= Speed * GameLogic.DeltaTime;
        }
        public void MoveRight() {
            dx += Speed * GameLogic.DeltaTime;
        }
        public void Jump() {
            if (UseGravity) {
                if (!InAir) {
                    dy = JumpPowerMax;
                }
            } else {
                dy = JumpPowerMax;
            }
        }
        public void Fall() {
            if (!UseGravity) {
                dy = -JumpPowerMax;
            }
        }


        public bool UpdatePosition() {
            float bouncePower = -1.2f;
            float bouncePowerHorz = -5f;
            bool moved = false;

            if (UseGravity) {
                dy -= gravity * GameLogic.DeltaTime;
            }
            dx *= 0.9f;



            Tile col = new Air(0, 0);
            Vector2 offset = new Vector2(0, dy * GameLogic.DeltaTime);
            if (Terrain.WillCollide(this, offset, out col)) {
                if (dy > 0) {
                    //hit ceiling
                    Position = new Vector2(Position.x, (int)Math.Ceiling(Position.y));
                    moved = true;
                    if (col.id == TileID.Bounce) {
                        dy *= bouncePower;
                        Position += new Vector2(0, dy * GameLogic.DeltaTime);
                    } else dy = 0;

                } else {
                    //hit ground
                    Position = new Vector2(Position.x, (int)Math.Floor(Position.y));
                    if (col.id == TileID.Bounce) {
                        dy *= bouncePower;
                        Position += new Vector2(0, dy * GameLogic.DeltaTime);
                        moved = true;
                    } else {
                        dy = 0;
                    }

                    InAir = false;
                }

            } else {
                if (offset.y != 0) moved = true;
                Position += offset;
                InAir = true;
            }

            col = new Air(0, 0);
            offset = new Vector2(dx, 0);
            if (Terrain.WillCollide(this, offset, out col)) {
                if (dx > 0) Position = new Vector2((int)Math.Ceiling(Position.x), Position.y);
                else Position = new Vector2((int)Math.Floor(Position.x), Position.y);

                if (col.id == TileID.Bounce) {
                    dx *= bouncePowerHorz;
                    Position += new Vector2(dx * GameLogic.DeltaTime, 0);
                    moved = true;
                } else dx = 0;
            } else {
                if (offset.x != 0) moved = true;
                Position += offset;
            }

            if (!UseGravity) dy = 0;


            return moved;
        }

        public abstract void Update();

        protected void CorrectTerrainCollision() {
            Position = Terrain.CorrectTerrainCollision(this);
        }


    }
}
