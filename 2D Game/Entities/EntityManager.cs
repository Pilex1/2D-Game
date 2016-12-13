using Game.Core;
using Game.Particles;
using Game.Terrains;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Entities {
    static class EntityManager {

        #region Fields
        internal const float maxHorzSpeed = 0.8f;
        internal const float maxVertSpeed = 1f;

        public static int LoadedEntities { get; private set; }
        private const int GridX = 4;
        private const int GridY = 4;
        internal static HashSet<Entity>[,] EntityGrid;
        public static ShaderProgram shader;
        #endregion

        #region Initialisation
        public static void Load(Entity[] entities) {
            foreach (Entity e in entities) {
                AddEntity(e);
            }
        }

        public static void LoadShaders() {
            shader = new ShaderProgram(Assets.Shaders.EntityVert, Assets.Shaders.EntityFrag);
            Console.WriteLine("Entity Shader Log: ");
            Console.WriteLine(shader.ProgramLog);
        }

        public static void Init() {
            Particle.Init();
            EntityGrid = new HashSet<Entity>[(int)Math.Ceiling((float)Terrain.Tiles.GetLength(0) / GridX), (int)Math.Ceiling((float)Terrain.Tiles.GetLength(1) / GridY)];
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j] = new HashSet<Entity>();
                }
            }
        }
        #endregion

        #region Matrices

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

        #endregion

        #region Entity Grid Array

        internal static Vector2i GetGridArray(Entity e) {
            int gx = (int)Math.Floor(e.data.pos.x / GridX);
            int gy = (int)Math.Floor(e.data.pos.y / GridY);
            if (gx < 0) gx = 0;
            if (gx >= EntityGrid.GetLength(0)) gx = EntityGrid.GetLength(0) - 1;
            if (gy < 0) gy = 0;
            if (gy >= EntityGrid.GetLength(1)) gy = EntityGrid.GetLength(1) - 1;
            return new Vector2i(gx, gy);
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
                        e.UpdateHitbox();

                        if (e.data.life.IsEmpty()) {
                            e.OnDeath();
                        }
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
                        } else {
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
                    } else {
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

        public static void ClearEntities() {
            for (int i = 0; i < EntityGrid.GetLength(0); i++) {
                for (int j = 0; j < EntityGrid.GetLength(1); j++) {
                    EntityGrid[i, j].Clear();
                }
            }
            AddEntity(Player.Instance);
        }

        public static void CleanUp() {
            Gl.DeleteShader(shader.FragmentShader.ShaderID);
            Gl.DeleteShader(shader.VertexShader.ShaderID);
            Gl.DeleteProgram(shader.ProgramID);
        }

        #endregion
    }
}
