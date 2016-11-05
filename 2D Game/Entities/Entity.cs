﻿using System;
using OpenGL;
using Game.Entities;
using Game.Terrains;
using System.Diagnostics;
using System.Collections.Generic;
using Game.Core;
using Game.Util;
using Game.Particles;
using System.Drawing;

namespace Game {

    enum EntityID {
        //16 x 16
        None = 0, ShooterProjectile, ParticlePurple, ParticleRed, ParticleGreen, ParticleBlue, Squisher,

        //16 x 32
        Shooter = 128, Player, PlayerSimple
    }

    [Serializable]
    class EntityData {

        public const float maxRecentDmgTime = 10;
        [NonSerialized]
        public float recentDmg = 0;

        public float speed = 0;
        public float rot = 0;
        public bool CorrectCollisions = true;
        public bool UseGravity = true;
        public float Grav = 0.02f;
        public float AirResis = 0.9f;
        public float jumppower = 0;
        public bool InAir = false;
        public BoundedFloat life = new BoundedFloat(0, 0, 0);
        public BoundedVector2 vel = new BoundedVector2(new BoundedFloat(0, -Entity.maxHorzSpeed, Entity.maxHorzSpeed), new BoundedFloat(0, -Entity.maxVertSpeed, Entity.maxVertSpeed));
        public BoundedVector2 Position = new BoundedVector2(new BoundedFloat(0, 0, Terrain.Tiles.GetLength(0) - 1), new BoundedFloat(0, 0, Terrain.Tiles.GetLength(1) - 1));
        public Vector4 colour = new Vector4(1, 1, 1, 1);

        public EntityID entityId = EntityID.None;
    }

    [Serializable]
    abstract class Entity {
        #region Fields
        internal const float maxHorzSpeed = 0.8f;
        internal const float maxVertSpeed = 1f;

        public static int LoadedEntities { get; private set; }
        private const int GridX = 4;
        private const int GridY = 4;
        private static HashSet<Entity>[,] EntityGrid;
        public static ShaderProgram shader;

        public EntityID entityId;
        public Hitbox Hitbox { get; protected set; }
        public static Color Colornew { get; private set; }

        public EntityData data = new EntityData { };
        #endregion

        #region Initialisation
        protected Entity(EntityID texid, Hitbox hitbox, EntityData data) {
            this.data = data;
            this.entityId = texid;
            Hitbox = hitbox;
            if (data.CorrectCollisions) CorrectTerrainCollision();
        }

        protected Entity(EntityID entityId) : this(entityId, Vector2.Zero) { }

        protected Entity(EntityID entityId, Vector2 position) : this(entityId, new RectangularHitbox(position, Assets.Models.GetModel(entityId).size), position) { }

        protected Entity(EntityID entityId, Hitbox hitbox, Vector2 position) {
            data.Position.val = position;
            this.entityId = entityId;
            Hitbox = hitbox;
        }

        public static void Load(Entity[] Entities) {
            Debug.Assert(Entities != null);
            foreach (Entity e in Entities) {
                AddEntity(e);
            }
        }

        public static void Init() {
            shader = new ShaderProgram(Assets.Shaders.EntityVert, Assets.Shaders.EntityFrag);
            Console.WriteLine("Entity Shader Log: ");
            Console.WriteLine(shader.ProgramLog);
            Particle.Init();
            EntityGrid = new HashSet<Entity>[(int)Math.Ceiling((float)Terrain.Tiles.GetLength(0) / GridX), (int)Math.Ceiling((float)Terrain.Tiles.GetLength(1) / GridY)];
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j] = new HashSet<Entity>();
                }
            }
        }
        #endregion

        #region Movement

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
            }
            else {
                data.vel.y = data.jumppower;
            }
        }
        public void Fall() {
            if (!data.UseGravity) {
                data.vel.y = -data.jumppower;
            }
        }





        /*
        private void UpdateX(float x) {
            Tile col;
            Vector2 offset = new Vector2(x, 0);
            if (Terrain.WillCollide(this, offset, out col)) {
                if (x > 0)
                    col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Right, this);
                else
                    col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Left, this);
            }
            else {
                data.Position.val += offset;
            }
        }

        private void UpdateY(float y) {
            Tile col;
            Vector2 offset = new Vector2(0, y);
            if (Terrain.WillCollide(this, offset, out col)) {
                if (y > 0)
                    col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Up, this);
                else
                    col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Down, this);
            }
            else {
                data.Position.val += offset;
                if (y != 0)
                    data.InAir = true;
            }
        }

        public virtual void UpdatePosition() {

            Vector2i gridPrev = GridArray(this);

            //air resisstance & gravity
            if (data.UseGravity)
                data.vel.y -= data.Grav * GameTime.DeltaTime;
            data.vel.x *= (float)Math.Pow(data.AirResis, GameTime.DeltaTime);

            float delta = 0.02f;

            float xcopy = data.vel.x;
            float ycopy = data.vel.y;
            bool xneg = xcopy < 0;
            bool yneg = ycopy < 0;
            bool xflag = xcopy == 0;
            bool yflag = ycopy == 0;
            xcopy = Math.Abs(xcopy);
            ycopy = Math.Abs(ycopy);

            while (true) {

                if (xflag && yflag) break;

                //update x
                if (!xflag) {
                    float amt = (xcopy >= delta ? delta : xcopy) * (xneg ? -1 : 1);
                    xcopy -= delta;
                    UpdateX(amt);

                    if (xcopy <= 0) xflag = true;
                }

                //update y
                if (!yflag) {
                    float amt = (ycopy >= delta ? delta : ycopy) * (yneg ? -1 : 1);
                    ycopy -= delta;
                    UpdateY(amt);

                    if (ycopy <= 0) yflag = true;
                }
            }




            Vector2i gridNow = GridArray(this);
            if (gridPrev != gridNow) {
                //recalc grid array
                EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityGrid[gridNow.x, gridNow.y].Add(this);
            }
        }

        */



        public bool UpdatePosition()
        {

            Vector2i gridPrev = GridArray(this);
            bool moved = false;

            if (data.UseGravity)
                data.vel.y -= data.Grav * GameTime.DeltaTime;

            data.vel.x *= (float)Math.Pow(data.AirResis, GameTime.DeltaTime);

            {
                Tile col;
                Vector2 offset = new Vector2(0, data.vel.y * GameTime.DeltaTime);
                if (Terrain.WillCollide(this, offset, out col))
                {
                    if (data.vel.y > 0)
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Up, this);
                    else
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Down, this);
                }
                else
                {
                    data.Position.val += offset;
                    if (offset.y != 0) moved = true;
                    data.InAir = true;
                }
            }

            {
                Tile col;
                Vector2 offset = new Vector2(data.vel.x * GameTime.DeltaTime, 0);
                if (Terrain.WillCollide(this, offset, out col))
                {
                    if (data.vel.x > 0)
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Right, this);
                    else
                        moved = col.tileattribs.OnTerrainIntersect((int)data.Position.x, (int)data.Position.y, Direction.Left, this);
                }
                else
                {
                    data.Position.val += offset;
                    if (offset.x != 0) moved = true;
                }

            }


            Vector2i gridNow = GridArray(this);
            if (gridPrev != gridNow)
            {
                //recalc grid array
                EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityGrid[gridNow.x, gridNow.y].Add(this);
            }

            return moved;
        }


        #endregion

        #region Collisions
        public void CorrectTerrainCollision() {
            Vector2i gridPrev = GridArray(this);
            data.Position.val = Terrain.CorrectTerrainCollision(this);
            Vector2i gridNow = GridArray(this);
            if (gridPrev != gridNow) {
                //recalc grid array
                EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityGrid[gridNow.x, gridNow.y].Add(this);
            }

        }

        protected List<Entity> EntityCollisions() {
            Vector2i grid = GridArray(this);
            HashSet<Entity> set = EntityGrid[grid.x, grid.y];
            List<Entity> list = new List<Entity>(set);
            List<Entity> colliding = new List<Entity>();
            foreach (Entity e in list) {
                if (e == this)
                    continue;
                if (Hitbox.Intersecting(e.Hitbox))
                    colliding.Add(e);
            }
            return colliding;
        }
        #endregion

        #region Health
        public void HealFull() {
            data.life.Fill();
        }

        public void Damage(float dmg) {
            data.life.val -= dmg;
            data.recentDmg = EntityData.maxRecentDmgTime;
        }

        public void Heal(float dmg) {
            data.life.val += dmg;
        }

        public void DecrMaxLife(float decr) {
            data.life.max -= decr;
            if (data.life.max < 0) data.life.max = 0;
            data.life.val += 0;
        }

        public void IncrMaxLife(float incr) {
            data.life.max += incr;
        }

        #endregion

        #region Matrices
        public virtual Matrix4 ModelMatrix() {
            Vector2 size = Assets.Models.GetModel(entityId).size;
            return Matrix4.CreateScaling(new Vector3(size.x, size.y, 0)) * Matrix4.CreateRotationZ(data.rot) * Matrix4.CreateTranslation(new Vector3(data.Position.x, data.Position.y, 0));
        }

        public static void UpdateViewMatrix(Matrix4 mat) {
            Debug.Assert(shader != null);
            Gl.UseProgram(shader.ProgramID);
            shader["viewMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }


        public static void SetProjectionMatrix(Matrix4 mat) {
            Debug.Assert(shader != null);
            Gl.UseProgram(shader.ProgramID);
            shader["projectionMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }
        #endregion Matrices

        #region Entity Grid Array

        private static Vector2i GridArray(Entity e) {
            int gx = (int)Math.Floor(e.data.Position.x / GridX);
            int gy = (int)Math.Floor(e.data.Position.y / GridY);
            return new Vector2i(gx, gy);
        }

        public static void AddEntity(Entity e) {
            Vector2i v = GridArray(e);
            EntityGrid[v.x, v.y].Add(e);
        }

        public static void RemoveEntity(Entity e) {
            Vector2i v = GridArray(e);
            EntityGrid[v.x, v.y].Remove(e);
        }

        public static void RemoveAllEntities() {
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j].Clear();
                }
            }

            AddEntity(Player.Instance);
        }
        #endregion

        #region Update & Render

        public static Entity[] GetAllEntities() {
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    foreach (Entity e in EntityGrid[i, j]) {
                        if (e != Player.Instance)
                            list.Add(e);
                    }
                }
            }
            return list.ToArray();
        }

        public virtual void Update() {
            UpdatePosition();
            Hitbox.Position = data.Position;
        }

        public static void UpdateAll() {
            int minx, maxx, miny, maxy;
            Terrain.Range(out minx, out maxx, out miny, out maxy);

            int mingx = (int)Math.Floor((float)minx / GridX);
            int mingy = (int)Math.Floor((float)miny / GridY);
            int maxgx = (int)Math.Ceiling((float)maxx / GridX);
            int maxgy = (int)Math.Ceiling((float)maxy / GridY);
            MathUtil.Clamp(ref mingx, 0, EntityGrid.GetLength(0) - 1);

            MathUtil.Clamp(ref mingy, 0, EntityGrid.GetLength(1) - 1);
            MathUtil.Clamp(ref maxgx, 0, EntityGrid.GetLength(0) - 1);
            MathUtil.Clamp(ref maxgy, 0, EntityGrid.GetLength(1) - 1);

            for (int i = mingx; i <= maxgx; i++) {
                for (int j = mingy; j <= maxgy; j++) {
                    HashSet<Entity> set = EntityGrid[i, j];
                    foreach (Entity e in new List<Entity>(set)) {
                        e.data.recentDmg -= GameTime.DeltaTime;
                        MathUtil.ClampMin(ref e.data.recentDmg, 0);
                        e.Update();
                        if (e.data.life <= 0)
                            set.Remove(e);
                    }
                }
            }
        }

        public static void Render() {
            Gl.UseProgram(shader.ProgramID);

            int minx, maxx, miny, maxy;
            Terrain.Range(out minx, out maxx, out miny, out maxy);
            int mingx = (int)Math.Floor((float)minx / GridX);
            int mingy = (int)Math.Floor((float)miny / GridY);
            int maxgx = (int)Math.Ceiling((float)maxx / GridX);
            int maxgy = (int)Math.Ceiling((float)maxy / GridY);
            MathUtil.Clamp(ref mingx, 0, EntityGrid.GetLength(0) - 1);
            MathUtil.Clamp(ref mingy, 0, EntityGrid.GetLength(1) - 1);
            MathUtil.Clamp(ref maxgx, 0, EntityGrid.GetLength(0) - 1);
            MathUtil.Clamp(ref maxgy, 0, EntityGrid.GetLength(1) - 1);

            Dictionary<EntityID, HashSet<Entity>> EntitiesMap = new Dictionary<EntityID, HashSet<Entity>>();
            for (int i = mingx; i <= maxgx; i++) {
                for (int j = mingy; j <= maxgy; j++) {
                    HashSet<Entity> set = EntityGrid[i, j];
                    foreach (Entity e in set) {
                        HashSet<Entity> setbatch;
                        if (EntitiesMap.TryGetValue(e.entityId, out setbatch)) {
                            setbatch.Add(e);
                        }
                        else {
                            setbatch = new HashSet<Entity>();
                            setbatch.Add(e);
                            EntitiesMap.Add(e.entityId, setbatch);
                        }
                    }
                }
            }

            LoadedEntities = 0;
            Gl.BindTexture(Assets.Textures.EntityTexture.TextureTarget, Assets.Textures.EntityTexture.TextureID);
            foreach (EntityID entityId in EntitiesMap.Keys) {
                EntityModel model = Assets.Models.GetModel(entityId);
                Gl.BindVertexArray(model.vao.ID);
                if (model.blend) {
                    Gl.Enable(EnableCap.Blend);
                    Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                }

                foreach (Entity e in EntitiesMap[entityId]) {
                    LoadedEntities++;
                    shader["modelMatrix"].SetValue(e.ModelMatrix());
                    if (e.data.recentDmg > 0) {
                        float offsetval = 1 - e.data.recentDmg / EntityData.maxRecentDmgTime;
                        offsetval /= 2;
                        Vector4 colouroffset = TextureUtil.ToVec4(Color.DarkGoldenrod) * new Vector4(offsetval, offsetval, offsetval, 1);
                        colouroffset *= e.data.colour;
                        shader["clr"].SetValue(colouroffset);
                    }
                    else {
                        shader["clr"].SetValue(e.data.colour);
                    }

                    Gl.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }

                Gl.Disable(EnableCap.Blend);
                Gl.BindVertexArray(0);
            }
            Gl.BindTexture(Assets.Textures.EntityTexture.TextureTarget, 0);
            Gl.UseProgram(0);
        }

        public static void CleanUp() {
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j].Clear();
                }
            }

            Gl.DeleteShader(shader.FragmentShader.ShaderID);
            Gl.DeleteShader(shader.VertexShader.ShaderID);
            Gl.DeleteProgram(shader.ProgramID);
        }
        #endregion
    }
}
