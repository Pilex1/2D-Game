using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using System.Diagnostics;
using Game.Assets;
using System.Collections.Generic;
using Game.Core;
using Game.Util;

namespace Game {

    [Serializable]
    class EntityData {
        public float speed = 0;
        public bool CorrectCollisions = true;
        public bool UseGravity = true;
        public float jumppower = 0;
        public bool InAir = false;
        public BoundedFloat dx = new BoundedFloat(0, -Entity.maxHorzSpeed, Entity.maxHorzSpeed);
        public BoundedFloat dy = new BoundedFloat(0, -Entity.maxVertSpeed, Entity.maxVertSpeed);
        public BoundedVector2 Position = new BoundedVector2(new BoundedFloat(0, 0, Terrain.Tiles.GetLength(0) - 1), new BoundedFloat(0, 0, Terrain.Tiles.GetLength(1) - 1));
    }

    abstract class Entity {
        internal const float gravity = 0.02f;
        internal const float maxHorzSpeed = 0.8f;
        internal const float maxVertSpeed = 1f;
        private static HashSet<Entity> Entities = new HashSet<Entity>();
        public static ShaderProgram shader;

        public EntityVAO vao;
        private Texture texture;
        public Hitbox Hitbox { get; protected set; }

        public EntityData data = new EntityData { };

        public static void Init() {
            shader = new ShaderProgram(FileUtil.LoadShader("EntityVertex"), FileUtil.LoadShader("EntityFragment"));
            Console.WriteLine("Entity Shader Log: ");
            Console.WriteLine(shader.ProgramLog);
        }

        public static void UpdateViewMatrix(Matrix4 mat) {
            Gl.UseProgram(shader.ProgramID);
            shader["viewMatrix"].SetValue(mat);
        }

        protected Entity(EntityVAO vao, Texture texture, Hitbox hitbox, EntityData data) {
            this.data = data;
            this.vao = vao;
            Hitbox = hitbox;
            this.texture = texture;
            if (data.CorrectCollisions) CorrectTerrainCollision();
            Entities.Add(this);
        }

        protected Entity(EntityVAO vao, Texture texture, Hitbox hitbox, Vector2 position) {
            data.Position.val = position;
            this.vao = vao;
            Hitbox = hitbox;
            this.texture = texture;
            if (data.CorrectCollisions) CorrectTerrainCollision();
            Entities.Add(this);
        }

        public virtual Matrix4 ModelMatrix() { return Matrix4.CreateTranslation(new Vector3(data.Position.x, data.Position.y, 0)); }

        public void MoveLeft() {
            data.dx.val -= data.speed * GameLogic.DeltaTime;
        }
        public void MoveRight() {
            data.dx.val += data.speed * GameLogic.DeltaTime;
        }
        public void Jump() {
            if (data.UseGravity) {
                if (!data.InAir) {
                    data.dy.val = data.jumppower;
                }
            } else {
                data.dy.val = data.jumppower;
            }
        }
        public void Fall() {
            if (!data.UseGravity) {
                data.dy.val = -data.jumppower;
            }
        }

        public bool UpdatePosition() {
            float bouncePower = -1.2f;
            float bouncePowerHorz = -5f;
            bool moved = false;

            if (data.UseGravity) {
                data.dy.val -= gravity * GameLogic.DeltaTime;
            }
            data.dx.val *= 0.9f;



            TileID col;
            Vector2 offset = new Vector2(0, data.dy.val * GameLogic.DeltaTime);
            if (Terrain.WillCollide(this, offset, out col)) {
                if (data.dy.val > 0) {
                    //hit ceiling
                    data.Position.val = new Vector2(data.Position.x, (int)Math.Ceiling(data.Position.y));
                    moved = true;
                    if (col.enumId == TileEnum.Bounce) {
                        data.dy.val *= bouncePower;
                        data.Position.val += new Vector2(0, data.dy.val * GameLogic.DeltaTime);
                    } else data.dy.val = 0;

                } else {
                    //hit ground
                    data.Position.val = new Vector2(data.Position.x, (int)Math.Floor(data.Position.y));
                    if (col.enumId == TileEnum.Bounce) {
                        data.dy.val *= bouncePower;
                        data.Position.val += new Vector2(0, data.dy.val * GameLogic.DeltaTime);
                        moved = true;
                    } else {
                        data.dy.val = 0;
                    }

                    data.InAir = false;
                }

            } else {
                if (offset.y != 0) moved = true;
                data.Position.val += offset;
                data.InAir = true;
            }

            offset = new Vector2(data.dx.val, 0);
            if (Terrain.WillCollide(this, offset, out col)) {
                if (data.dx.val > 0) data.Position.val = new Vector2((int)Math.Ceiling(data.Position.x), data.Position.y);
                else data.Position.val = new Vector2((int)Math.Floor(data.Position.x), data.Position.y);

                if (col.enumId == TileEnum.Bounce) {
                    data.dx.val *= bouncePowerHorz;
                    data.Position.val += new Vector2(data.dx.val * GameLogic.DeltaTime, 0);
                    moved = true;
                } else data.dx.val = 0;
            } else {
                if (offset.x != 0) moved = true;
                data.Position.val += offset;
            }

            if (!data.UseGravity) data.dy.val = 0;


            return moved;
        }

        public abstract void Update();

        protected void CorrectTerrainCollision() {
            data.Position.val = Terrain.CorrectTerrainCollision(this);
        }

        public static void RemoveEntity(Entity e) {
            Entities.Remove(e);
        }

        public static void SetProjectionMatrix(Matrix4 mat) {
            Gl.UseProgram(shader.ProgramID);
            shader["projectionMatrix"].SetValue(mat);
        }

        public static void Render() {
            Gl.UseProgram(shader.ProgramID);
            shader["clr"].SetValue(new Vector3(1, 1, 1));
            foreach (var e in Entities) {
                shader["modelMatrix"].SetValue(e.ModelMatrix());
                Gl.BindVertexArray(e.vao.ID);
                Gl.BindTexture(e.texture.TextureTarget, e.texture.TextureID);
                Gl.DrawElements(BeginMode.Triangles, e.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(e.texture.TextureTarget, e.texture.TextureID);
                Gl.BindVertexArray(0);
            }
            Gl.UseProgram(0);
        }

        public static void CleanUp() {
            foreach (var e in Entities) {
                Gl.DeleteTextures(1, new uint[] { e.texture.TextureID });
                e.vao.Dispose();
            }
            Entities.Clear();
            Gl.DeleteShader(shader.FragmentShader.ShaderID);
            Gl.DeleteShader(shader.VertexShader.ShaderID);
            Gl.DeleteProgram(shader.ProgramID);
        }

    }
}
