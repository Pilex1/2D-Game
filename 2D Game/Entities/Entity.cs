using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using System.Diagnostics;
using Game.Assets;
using System.Collections.Generic;
using Game.Core;
using Game.Util;
using Game.Particles;

namespace Game {

    [Serializable]
    class EntityData {
        public float speed = 0;
        public float rot = 0;
        public bool CorrectCollisions = true;
        public bool UseGravity = true;
        public bool UseAirResis = true;
        public float jumppower = 0;
        public bool InAir = false;
        public BoundedVector2 vel = new BoundedVector2(new BoundedFloat(0, -Entity.maxHorzSpeed, Entity.maxHorzSpeed), new BoundedFloat(0, -Entity.maxVertSpeed, Entity.maxVertSpeed));
        public BoundedVector2 Position = new BoundedVector2(new BoundedFloat(0, 0, Terrain.Tiles.GetLength(0) - 1), new BoundedFloat(0, 0, Int32.MaxValue));
        public Vector4 colour = new Vector4(1, 1, 1, 1);
    }

    abstract class Entity {
        #region fields
        internal const float gravity = 0.02f;
        internal const float maxHorzSpeed = 0.8f;
        internal const float maxVertSpeed = 1f;

        private static Dictionary<EntityModel, HashSet<Entity>> Entities = new Dictionary<EntityModel, HashSet<Entity>>();
        public static ShaderProgram shader;

        public EntityModel model;
        public Hitbox Hitbox { get; protected set; }

        public EntityData data = new EntityData { };
        #endregion fields

        #region constructors
        protected Entity(EntityModel model, Hitbox hitbox, EntityData data) {
            this.data = data;
            this.model = model;
            Hitbox = hitbox;
            if (data.CorrectCollisions) CorrectTerrainCollision();
            AddEntity(this);
        }

        protected Entity(EntityModel model) : this(model, Vector2.Zero) { }

        protected Entity(EntityModel model, Vector2 position) : this(model, new RectangularHitbox(position, model.size), position) { }

        protected Entity(EntityModel model, Hitbox hitbox, Vector2 position) {
            data.Position.val = position;
            this.model = model;
            Hitbox = hitbox;
            AddEntity(this);
        }
        #endregion constructors

        #region instance methods

        public void MoveLeft() {
            data.vel.x -= data.speed * GameTime.DeltaTime;
        }
        public void MoveRight() {
            data.vel.x += data.speed * GameTime.DeltaTime;
        }
        public void Jump() {
            if (data.UseGravity) {
                if (!data.InAir) {
                    data.vel.y = data.jumppower;
                }
            } else {
                data.vel.y = data.jumppower;
            }
        }
        public void Fall() {
            if (!data.UseGravity) {
                data.vel.y = -data.jumppower;
            }
        }

        public bool UpdatePosition() {

            bool moved = false;

            if (data.UseGravity)
                data.vel.y -= gravity * GameTime.DeltaTime;

            if (data.UseAirResis)
                data.vel.x *= 0.9f;

            {
                TileID col;
                Vector2 offset = new Vector2(0, data.vel.y * GameTime.DeltaTime);
                if (Terrain.WillCollide(this, offset, out col)) {
                    if (data.vel.y > 0)
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Up, this);
                    else
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Down, this);
                } else {
                    data.Position.val += offset;
                    if (offset.y != 0) moved = true;
                    data.InAir = true;
                }
                if (!data.UseGravity) data.vel.y = 0;
            }

            {
                TileID col;
                Vector2 offset = new Vector2(data.vel.x, 0);
                if (Terrain.WillCollide(this, offset, out col)) {
                    if (data.vel.x > 0)
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Right, this);
                    else
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Left, this);
                } else {
                    if (offset.x != 0) moved = true;
                    data.Position.val += offset;
                }

            }

            return moved;
        }

        public abstract void Update();

        protected void CorrectTerrainCollision() {
            data.Position.val = Terrain.CorrectTerrainCollision(this);
        }

        #endregion instance methods

        #region Matrices
        public Matrix4 ModelMatrix() { return Matrix4.CreateScaling(new Vector3(model.size.x, model.size.y, 0)) * Matrix4.CreateRotationZ(data.rot) * Matrix4.CreateTranslation(new Vector3(data.Position.x, data.Position.y, 0)); }

        public static void UpdateViewMatrix(Matrix4 mat) {
            if (shader == null)
                throw new NullReferenceException("Entity shader null");
            Gl.UseProgram(shader.ProgramID);
            shader["viewMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }


        public static void SetProjectionMatrix(Matrix4 mat) {
            if (shader == null)
                throw new NullReferenceException("Entity shader null");
            Gl.UseProgram(shader.ProgramID);
            shader["projectionMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }
        #endregion Matrices

        #region static methods
        public static void Init() {
            shader = new ShaderProgram(FileUtil.LoadShader("EntityVertex"), FileUtil.LoadShader("EntityFragment"));
            Console.WriteLine("Entity Shader Log: ");
            Console.WriteLine(shader.ProgramLog);
            Particle.Init();
        }

        private static void AddEntity(Entity e) {
            HashSet<Entity> set;
            Entities.TryGetValue(e.model, out set);
            if (set == null) {
                set = new HashSet<Entity>();
                set.Add(e);
                Entities[e.model] = set;
            } else {
                set.Add(e);
            }
        }

        public static void RemoveEntity(Entity e) {
            HashSet<Entity> set = Entities[e.model];
            set.Remove(e);
            if (set.Count == 0) Entities.Remove(e.model);
        }

        public static void RemoveAllEntities() {
            Entities.Clear();
            AddEntity(Player.Instance);
        }


        public static void UpdateAll() {
            Vector2i min = Vector2i.Zero, max = Vector2i.Zero;
            Terrain.Range(out min.x, out max.x, out min.y, out max.y);
            foreach (HashSet<Entity> set in new List<HashSet<Entity>>(Entities.Values)) {
                foreach (Entity e in new List<Entity>(set)) {
                    if (MathUtil.Bounded(e.data.Position.val, min, max))
                        e.Update();
                }
            }
        }

        public static void Render() {
            Gl.UseProgram(shader.ProgramID);
            foreach (EntityModel model in Entities.Keys) {
                Gl.BindVertexArray(model.vao.ID);
                if (model.blend) {
                    Gl.Enable(EnableCap.Blend);
                    Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                }
                Gl.BindTexture(model.texture.TextureTarget, model.texture.TextureID);
                foreach (Entity e in Entities[model]) {
                    shader["modelMatrix"].SetValue(e.ModelMatrix());
                    shader["clr"].SetValue(e.data.colour);
                    Gl.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }
                Gl.BindTexture(model.texture.TextureTarget, 0);
                Gl.Disable(EnableCap.Blend);
                Gl.BindVertexArray(0);
            }
            Gl.UseProgram(0);
        }

        public static void CleanUp() {
            foreach (EntityModel model in Entities.Keys) {
                model.Dispose();
            }
            Entities.Clear();
            Gl.DeleteShader(shader.FragmentShader.ShaderID);
            Gl.DeleteShader(shader.VertexShader.ShaderID);
            Gl.DeleteProgram(shader.ProgramID);
        }
        #endregion static methods

    }
}
