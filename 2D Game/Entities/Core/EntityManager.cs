using Game.Core;
using Game.Entities.Particles;
using Game.Main.GLConstructs;
using Game.Terrains;
using Game.Terrains.Lighting;
using Game.Terrains.Terrain_Generation;
using Game.Util;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Game.Entities {
    static class EntityManager {

        #region Fields
        internal const float maxHorzSpeed = 1f;
        internal const float maxVertSpeed = 1f;

        private const int GridX = 4;
        private const int GridY = 4;

        public static int LoadedEntities { get; private set; }
        internal static HashSet<Entity>[,] EntityGrid;
        public static ShaderProgram Shader;
        #endregion

        #region Initialisation
        public static void Load(Entity[] entities) {
            foreach (Entity e in entities) {
                AddEntity(e);
            }
        }

        public static void LoadShaders() {
            Shader = new ShaderProgram(Assets.Shaders.EntityVert, Assets.Shaders.EntityFrag);
            Shader.AddUniform("viewMatrix");
            Shader.AddUniform("projectionMatrix");
            Shader.AddUniform("modelMatrix");
            Shader.AddUniform("clr");
        }

        public static void Init() {
            Particle.Init();
            EntityGrid = new HashSet<Entity>[(int)Math.Ceiling((float)TerrainGen.SizeX / GridX), (int)Math.Ceiling((float)TerrainGen.SizeY / GridY)];
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j] = new HashSet<Entity>();
                }
            }
        }
        #endregion

        #region Matrices

        public static void UpdateViewMatrix(Matrix mat) {
            GL.UseProgram(Shader.ID);
            Shader.SetUniform4m("viewMatrix", mat);
            GL.UseProgram(0);
        }


        public static void SetProjectionMatrix(Matrix mat) {
            GL.UseProgram(Shader.ID);
            Shader.SetUniform4m("projectionMatrix", mat);
            GL.UseProgram(0);
        }

        #endregion

        #region Entity Grid Array

        public static Entity[] GetEntitiesAt(Vector2 pos, Vector2 size) {
            return GetEntitiesAt(pos, size, x => true);
        }

        public static Entity[] GetEntitiesAt(Vector2 pos, Vector2 size, Predicate<Entity> predicate) {
            var g = GetGridArray(pos);
            var search = new HashSet<Entity>();
            for (int i = 0; i < Math.Ceiling(size.x / GridX) + 1; i++) {
                for (float j = 0; j < Math.Ceiling(size.y / GridY) + 1; j++) {
                    var grid = GetGridArray(pos + new Vector2(i * GridX, j * GridY));
                    var entities = EntityGrid[grid.x, grid.y];
                    foreach (var e in entities) {
                        search.Add(e);
                    }
                }
            }
            var r = new List<Entity>();
            foreach (var e in search) {
                if (e.hitbox.Intersecting(new RectangularHitbox(pos, size)) && predicate(e))
                    r.Add(e);
            }
            return r.ToArray();
        }
        public static Entity[] GetEntitiesAt(Vector2 pos) {
            return GetEntitiesAt(pos, new Vector2(1, 1));
        }

        internal static Vector2i GetGridArray(Vector2 pos) {
            int gx = (int)Math.Floor(pos.x / GridX);
            int gy = (int)Math.Floor(pos.y / GridY);
            gx = MathUtil.Clamp(gx, 0, EntityGrid.GetLength(0) - 1);
            gy = MathUtil.Clamp(gy, 0, EntityGrid.GetLength(1) - 1);
            return new Vector2i(gx, gy);
        }

        internal static Vector2i GetGridArray(Entity e) {
            return GetGridArray(e.data.pos);
        }

        public static void AddEntity(Entity e) {
            Vector2i v = GetGridArray(e);
            EntityGrid[v.x, v.y].Add(e);
        }

        public static void RemoveEntity(Entity e) {
            Vector2i v = GetGridArray(e);
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

        public static void UpdateAll() {
            if (Player.Instance == null) return;
            GameTime.EntityUpdatesTimer.Start();
            int minx, maxx, miny, maxy;
            Terrain.Range(out minx, out maxx, out miny, out maxy);

            int mingx = (int)Math.Floor((float)minx / GridX);
            int mingy = (int)Math.Floor((float)miny / GridY);
            int maxgx = (int)Math.Ceiling((float)maxx / GridX);
            int maxgy = (int)Math.Ceiling((float)maxy / GridY);
            mingx = MathUtil.Clamp(mingx, 0, EntityGrid.GetLength(0) - 1);
            mingy = MathUtil.Clamp(mingy, 0, EntityGrid.GetLength(1) - 1);
            maxgx = MathUtil.Clamp(maxgx, 0, EntityGrid.GetLength(0) - 1);
            maxgy = MathUtil.Clamp(maxgy, 0, EntityGrid.GetLength(1) - 1);

            for (int i = mingx; i <= maxgx; i++) {
                for (int j = mingy; j <= maxgy; j++) {
                    HashSet<Entity> set = EntityGrid[i, j];
                    foreach (Entity e in new List<Entity>(set)) {
                        e.data.recentDmg -= GameTime.DeltaTime;
                        e.data.recentDmg = Math.Max(e.data.recentDmg, 0);

                        e.UpdateHitbox();
                        e.Update();

                        if (e.data.life.IsEmpty() && !e.data.invulnerable) {
                            e.OnDeath();
                        }
                    }
                }
            }
            GameTime.EntityUpdatesTimer.Pause();
        }

        public static void UpdateLightEmittingBefore() {
            if (Player.Instance == null) return;
            GameTime.EntityUpdatesTimer.Start();
            int minx, maxx, miny, maxy;
            Terrain.Range(out minx, out maxx, out miny, out maxy);

            int mingx = (int)Math.Floor((float)minx / GridX);
            int mingy = (int)Math.Floor((float)miny / GridY);
            int maxgx = (int)Math.Ceiling((float)maxx / GridX);
            int maxgy = (int)Math.Ceiling((float)maxy / GridY);
            mingx = MathUtil.Clamp(mingx, 0, EntityGrid.GetLength(0) - 1);
            mingy = MathUtil.Clamp(mingy, 0, EntityGrid.GetLength(1) - 1);
            maxgx = MathUtil.Clamp(maxgx, 0, EntityGrid.GetLength(0) - 1);
            maxgy = MathUtil.Clamp(maxgy, 0, EntityGrid.GetLength(1) - 1);

            for (int i = mingx; i <= maxgx; i++) {
                for (int j = mingy; j <= maxgy; j++) {
                    HashSet<Entity> set = EntityGrid[i, j];
                    foreach (Entity e in new List<Entity>(set)) {
                        ILight l = e as ILight;
                        if (l != null) {
                            LightingManager.AddLight((int)e.data.pos.x, (int)e.data.pos.y, l.Radius(), l.Strength(), l.Colour());
                        }
                    }
                }
            }
            GameTime.EntityUpdatesTimer.Pause();
        }

        public static void UpdateLightEmittingAfter() {
            if (Player.Instance == null) return;
            GameTime.EntityUpdatesTimer.Start();
            int minx, maxx, miny, maxy;
            Terrain.Range(out minx, out maxx, out miny, out maxy);

            int mingx = (int)Math.Floor((float)minx / GridX);
            int mingy = (int)Math.Floor((float)miny / GridY);
            int maxgx = (int)Math.Ceiling((float)maxx / GridX);
            int maxgy = (int)Math.Ceiling((float)maxy / GridY);
            mingx = MathUtil.Clamp(mingx, 0, EntityGrid.GetLength(0) - 1);
            mingy = MathUtil.Clamp(mingy, 0, EntityGrid.GetLength(1) - 1);
            maxgx = MathUtil.Clamp(maxgx, 0, EntityGrid.GetLength(0) - 1);
            maxgy = MathUtil.Clamp(maxgy, 0, EntityGrid.GetLength(1) - 1);

            for (int i = mingx; i <= maxgx; i++) {
                for (int j = mingy; j <= maxgy; j++) {
                    HashSet<Entity> set = EntityGrid[i, j];
                    foreach (Entity e in new List<Entity>(set)) {
                        ILight l = e as ILight;
                        if (l != null) {
                            LightingManager.RemoveLight((int)e.data.pos.x, (int)e.data.pos.y, l.Radius(), l.Strength(), l.Colour());
                        }
                    }
                }
            }
            GameTime.EntityUpdatesTimer.Pause();
        }

        public static void Render() {
            if (Player.Instance == null) return;
            GameTime.EntityRenderTimer.Start();
            GL.UseProgram(Shader.ID);

            int minx, maxx, miny, maxy;
            Terrain.Range(out minx, out maxx, out miny, out maxy);
            int mingx = (int)Math.Floor((float)minx / GridX);
            int mingy = (int)Math.Floor((float)miny / GridY);
            int maxgx = (int)Math.Ceiling((float)maxx / GridX);
            int maxgy = (int)Math.Ceiling((float)maxy / GridY);
            mingx = MathUtil.Clamp(mingx, 0, EntityGrid.GetLength(0) - 1);
            mingy = MathUtil.Clamp(mingy, 0, EntityGrid.GetLength(1) - 1);
            maxgx = MathUtil.Clamp(maxgx, 0, EntityGrid.GetLength(0) - 1);
            maxgy = MathUtil.Clamp(maxgy, 0, EntityGrid.GetLength(1) - 1);

            Dictionary<EntityID, HashSet<Entity>> EntitiesMap = new Dictionary<EntityID, HashSet<Entity>>();
            for (int i = mingx; i <= maxgx; i++) {
                for (int j = mingy; j <= maxgy; j++) {
                    HashSet<Entity> set = EntityGrid[i, j];
                    foreach (Entity e in set) {
                        HashSet<Entity> setbatch;
                        if (EntitiesMap.TryGetValue(e.entityId, out setbatch)) {
                            setbatch.Add(e);
                        } else {
                            setbatch = new HashSet<Entity>();
                            setbatch.Add(e);
                            EntitiesMap.Add(e.entityId, setbatch);
                        }
                    }
                }
            }

            LoadedEntities = 0;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BindTexture(Assets.Textures.EntityTexture.TextureTarget, Assets.Textures.EntityTexture.TextureID);
            foreach (EntityID entityId in EntitiesMap.Keys) {
                EntityModel model = Assets.Models.GetModel(entityId);
                GL.BindVertexArray(model.VAO.ID);


                foreach (Entity e in EntitiesMap[entityId]) {
                    LoadedEntities++;
                    Shader.SetUniform4m("modelMatrix", e.ModelMatrix());
                    if (e.data.recentDmg > 0) {
                        float offsetval = 1 - e.data.recentDmg / EntityData.maxRecentDmgTime;
                        offsetval /= 2;
                        Vector4 colouroffset = Vector4.Multiply(TextureUtil.ToVec4(Color.DarkGoldenrod), new Vector4(offsetval, offsetval, offsetval, 1));
                        colouroffset += e.data.colour;
                        Shader.SetUniform4f("clr", colouroffset);
                    } else {
                        Shader.SetUniform4f("clr", e.data.colour);
                    }

                    GL.DrawElements(model.Drawmode, model.VAO.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }


            }

            if (GameLogic.RenderHitboxes) {
                var model = Assets.Models.GetModel(EntityID.BlackOutline);
                GL.BindVertexArray(model.VAO.ID);
                foreach (EntityID entityId in EntitiesMap.Keys) {
                    foreach (Entity e in EntitiesMap[entityId]) {
                        var h = e.hitbox;
                        Shader.SetUniform4m("modelMatrix", MathUtil.ModelMatrix(h.Size, 0, h.Position));
                        GL.DrawElements(model.Drawmode, model.VAO.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
                GL.BindVertexArray(0);
            }

            GL.BindTexture(Assets.Textures.EntityTexture.TextureTarget, 0);
            GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            GameTime.EntityRenderTimer.Pause();
        }

        public static void ClearEntities() {
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j].Clear();
                }
            }
            AddEntity(Player.Instance);
        }

        public static void CleanUp() {
            GL.DeleteShader(Shader.FragID);
            GL.DeleteShader(Shader.VertID);
            GL.DeleteProgram(Shader.ID);
        }

        #endregion
    }
}
