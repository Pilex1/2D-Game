using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;
using Game.Assets;
using Game.Util;
using System.Diagnostics;
using Game.Terrains.Logics;
using Game.Core;
using Game.Terrains.Terrain_Generation;
using Game.Core.World_Serialization;
using Game.Terrains.Fluids;
using Game.Terrains.Lighting;

namespace Game.Terrains {

    static class Terrain {

        #region Fields
        internal static Tile[,] Tiles;
        internal static Biome[] TerrainBiomes;

        public static bool generating { get; private set; }

        public static TerrainVAO vao;


        private const int TerrainTextureSize = 16;

        public static ShaderProgram Shader { get; private set; }
        #endregion

        #region Init
        public static void CreateNew(int seed) {
            generating = true;
            TerrainGen.Generate(seed);
            generating = false;
        }

        public static void Load(TerrainData data) {
            Tiles = data.terrain;

            FluidManager.Instance.LoadDict(data.fluidDict);
            LogicManager.Instance.LoadDict(data.logicDict);
            LightingManager.Lightings = data.lightings;

        }

        public static void LoadShaders() {
            Shader = new ShaderProgram(Shaders.TerrainVert, Shaders.TerrainFrag);
            Console.WriteLine("Terrain Shader Log: ");
            Console.WriteLine(Shader.ProgramLog);
        }

        public static void Init() {
            LightingManager.Init();
            vao = new TerrainVAO(new Vector2[] { }, new int[] { }, new Vector2[] { }, new Vector4[] { });
        }

        #endregion

        #region Matrices
        public static void UpdateViewMatrix(Matrix4 mat) {
            Debug.Assert(Shader != null);
            Gl.UseProgram(Shader.ProgramID);
            Shader["viewMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }

        public static void SetProjectionMatrix(Matrix4 mat) {
            Debug.Assert(Shader != null);
            Gl.UseProgram(Shader.ProgramID);
            Shader["projectionMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }
        #endregion

        #region Mesh
        internal static void Range(out int minx, out int maxx, out int miny, out int maxy) {
            float posX = (int)Player.Instance.data.pos.x;
            float posY = (int)Player.Instance.data.pos.y;

            minx = (int)(posX + GameRenderer.zoom / 2);
            maxx = (int)(posX - GameRenderer.zoom / 2);
            miny = (int)(posY + GameRenderer.zoom / 2 / Program.AspectRatio);
            maxy = (int)(posY - GameRenderer.zoom / 2 / Program.AspectRatio);
            MathUtil.Clamp(ref minx, 0, Tiles.GetLength(0) - 1);
            MathUtil.Clamp(ref miny, 0, Tiles.GetLength(1) - 1);
            MathUtil.Clamp(ref maxx, 0, Tiles.GetLength(0) - 1);
            MathUtil.Clamp(ref maxy, 0, Tiles.GetLength(1) - 1);
        }

        public static void CalculateMesh(out Vector2[] vertices, out int[] elements, out Vector2[] uvs) {
            int startX, endX, startY, endY;
            Range(out startX, out endX, out startY, out endY);
            List<Vector2> verticesList = new List<Vector2>((endX - startX + 1) * (endY - startY + 1));
            List<Vector2> uvList = new List<Vector2>((endX - startX + 1) * (endY - startY + 1));
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    if (Tiles[i, j].enumId != TileID.Air) {
                        TileID t = Tiles[i, j].enumId;

                        float x = ((float)((int)t % TerrainTextureSize)) / TerrainTextureSize;
                        float y = ((float)((int)t / TerrainTextureSize)) / TerrainTextureSize;
                        float s = 1f / TerrainTextureSize;
                        //half pixel correction
                        float h = 1f / (TerrainTextureSize * TerrainTextureSize * 2);

                        // float height = 1;
                        //TODO
                        //something like
                        float height = 1;
                        FluidAttribs fattribs = Tiles[i, j].tileattribs as FluidAttribs;
                        if (fattribs != null) {
                            height = fattribs.GetHeight();
                        }

                        //top left, bottom left, bottom right, top right
                        verticesList.AddRange(new Vector2[] {
                            new Vector2(i,j+height),
                            new Vector2(i,j),
                            new Vector2(i+1,j),
                            new Vector2(i+1,j+height)
                        });

                        switch (Tiles[i, j].tileattribs.rotation) {
                            case Direction.Up:
                                //top left, bottom left, bottom right, top right
                                uvList.AddRange(new Vector2[] {
                                    new Vector2(x+h,y+s-h),
                                    new Vector2(x+h,y+h),
                                    new Vector2(x+s-h,y+h),
                                    new Vector2(x+s-h,y+s-h)
                                });
                                break;
                            case Direction.Right:
                                uvList.AddRange(new Vector2[] {
                                    new Vector2(x+h,y+h),
                                    new Vector2(x+s-h,y+h),
                                    new Vector2(x+s-h,y+s-h),
                                    new Vector2(x+h,y+s-h)
                                });
                                break;
                            case Direction.Down:
                                uvList.AddRange(new Vector2[] {
                                    new Vector2(x+s-h,y+h),
                                    new Vector2(x+s-h,y+s-h),
                                    new Vector2(x+h,y+s-h),
                                    new Vector2(x+h,y+h),
                                });
                                break;
                            case Direction.Left:
                                uvList.AddRange(new Vector2[] {
                                    new Vector2(x+s-h,y+s-h),
                                    new Vector2(x+h,y+s-h),
                                    new Vector2(x+h,y+h),
                                    new Vector2(x+s-h,y+h)
                                });
                                break;
                        }
                    }
                }
            }
            vertices = verticesList.ToArray();
            uvs = uvList.ToArray();

            elements = new int[verticesList.Count / 4 * 6];
            for (int i = 0; i < verticesList.Count / 4; i++) {
                elements[6 * i] = 4 * i;
                elements[6 * i + 1] = 4 * i + 1;
                elements[6 * i + 2] = 4 * i + 2;
                elements[6 * i + 3] = 4 * i + 2;
                elements[6 * i + 4] = 4 * i + 3;
                elements[6 * i + 5] = 4 * i + 0;
            }
        }

        internal static Tile[,] ShallowCopyTileData() {
            Tile[,] result = new Tile[Tiles.GetLength(0), Tiles.GetLength(1)];
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    result[i, j] = Tiles[i, j];
                }
            }
            return result;
        }

        #endregion Mesh

        #region Collision

        public static Tuple<Vector2i, Tile>[] CalcFutureCollision(Entity entity, Vector2 offset) {
            return CalcFutureCollision(entity.hitbox, offset);
        }
        public static Tuple<Vector2i, Tile>[] CalcFutureCollision(Hitbox hitbox, Vector2 offset) {
            int x1 = (int)Math.Floor(hitbox.Position.x + offset.x);
            int x2 = (int)Math.Floor(hitbox.Position.x + hitbox.Size.x + offset.x);

            int y1 = (int)Math.Floor(hitbox.Position.y + offset.y);
            int y2 = (int)Math.Floor(hitbox.Position.y + hitbox.Size.y + offset.y);

            var cols = new List<Tuple<Vector2i, Tile>>();

            for (int i = x1; i <= x2; i++) {
                for (int j = y1; j <= y2; j++) {
                    if (TileAt(i, j).enumId != TileID.Air) {
                        cols.Add(new Tuple<Vector2i, Tile>(new Vector2i(i, j), TileAt(i, j)));
                    }
                }
            }

            var array = cols.ToArray();
            return cols.ToArray();
        }

        public static Tuple<Vector2i, Tile>[] CalcCollision(Entity entity) {
            return CalcCollision(entity.hitbox);
        }
        public static Tuple<Vector2i, Tile>[] CalcCollision(Hitbox h) {
            return CalcFutureCollision(h, Vector2.Zero);
        }

        public static Vector2 CorrectTerrainCollision(Entity entity) {
            int height = -1;
            //find highest non intersecting terrain
            for (int i = (int)entity.data.pos.x; i < entity.data.pos.x + entity.hitbox.Size.x + MathUtil.Epsilon; i++) {
                int curHeight = NonCollisionAbove(i, (int)entity.data.pos.y, (int)(entity.hitbox.Size.y + MathUtil.Epsilon));
                if (curHeight > height) height = curHeight;
            }
            if (height == -1)
                return entity.data.pos.val;
            return new Vector2(entity.data.pos.x, height);
        }

        private static int NonCollisionAbove(int x, int y, int height) {
            int ry = y;
            while (true) {
                if (Valid(x, ry, height)) break;
                ry++;
            }
            return ry;
        }

        private static bool Valid(int x, int y, int height) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return true;
            for (int i = 0; i < height; i++) {
                if (y + i >= Tiles.GetLength(1)) continue;
                if (Tiles[x, y + i].enumId != TileID.Air) return false;
            }
            return true;
        }

        internal static int HighestPoint(int x) {
            if (x < 0 || x >= Tiles.GetLength(0)) return 0;
            for (int i = Tiles.GetLength(1) - 1; i > 0; i--) {
                if (Tiles[x, i].enumId != TileID.Air) return i + 1;
            }
            return 1;
        }
        #endregion Collision

        #region Tile Interaction
        public static Tile TileAt(Vector2i v) { return TileAt(v.x, v.y); }
        public static Tile TileAt(int x, int y) { return x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1) ? Tile.Invalid : Tiles[x, y]; }
        public static Tile TileAt(float x, float y) { return TileAt((int)x, (int)y); }

        public static void SetTile(int x, int y, Tile tile, Vector2 v, bool overwrite = false) {
            Direction d = DirectionUtil.FromVector2(v);
            tile.tileattribs.rotation = d;
            SetTile(x, y, tile, overwrite);
        }
        public static void SetTile(int x, int y, Tile tile, bool overwrite = false) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return;
            if (!overwrite && Tiles[x, y].enumId != TileID.Air) return;

            Tiles[x, y] = tile;

            LogicAttribs logic = tile.tileattribs as LogicAttribs;
            if (logic != null) LogicManager.Instance.AddUpdate(x, y, logic);

            FluidAttribs fluid = tile.tileattribs as FluidAttribs;
            if (fluid != null) FluidManager.Instance.AddUpdate(x, y, fluid);
            FluidManager.Instance.UpdateAround(x, y);

            if (!generating) {
                ILight light = tile.tileattribs as ILight;
                if (light != null) LightingManager.AddLight(x, y, light.Radius(), light.Colour());

                LightingManager.AddTile(x, y);
            }
        }

        public static Tile BreakTile(int x, int y) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return Tile.Invalid;
            Tile tile = TileAt(x, y);
            if (tile.enumId == TileID.Air) return Tile.Air;
            if (tile.enumId == TileID.Bedrock) return Tile.Invalid;

            Tiles[x, y] = Tile.Air;

            LogicManager.Instance.RemoveUpdate(x, y);
            FluidManager.Instance.RemoveUpdate(x, y);
            FluidManager.Instance.UpdateAround(x, y);

            if (!generating) {
                ILight light = tile.tileattribs as ILight;
                if (light != null) {
                    LightingManager.RemoveLight(x, y, light.Radius(), light.Colour());
                }
                LightingManager.RemoveTile(x, y);
            }
            return tile;
        }
        public static Tile BreakTile(Vector2i v) {
            return BreakTile(v.x, v.y);
        }

        public static void MoveTile(int x, int y, Direction dir) {
            Vector2i v = new Vector2i(x, y);
            switch (dir) {
                case Direction.Left:
                    if (TileAt(x - 1, y).enumId == TileID.Air) {
                        SetTile(x - 1, y, TileAt(x, y));
                        BreakTile(x, y);
                    }
                    break;
                case Direction.Right:
                    if (TileAt(x + 1, y).enumId == TileID.Air) {
                        SetTile(x + 1, y, TileAt(x, y));
                        BreakTile(x, y);
                    }
                    break;
                case Direction.Up:
                    if (TileAt(x, y + 1).enumId == TileID.Air) {
                        SetTile(x, y + 1, TileAt(x, y));
                        BreakTile(x, y);
                    }
                    break;
                case Direction.Down:
                    if (TileAt(x, y - 1).enumId == TileID.Air) {
                        SetTile(x, y - 1, TileAt(x, y));
                        BreakTile(x, y);
                    }
                    break;
            }

        }
        #endregion

        #region Update & Render
        public static void Render() {
            Gl.UseProgram(Shader.ProgramID);
            Gl.BindVertexArray(vao.ID);
            Gl.BindTexture(Textures.TerrainTexture.TextureTarget, Textures.TerrainTexture.TextureID);
            Gl.DrawElements(BeginMode.Triangles, vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(Textures.TerrainTexture.TextureTarget, 0);
            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        public static void Update() {
            FluidManager.Instance.Update();
            LogicManager.Instance.Update();

            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            CalculateMesh(out vertices, out elements, out uvs);
            Vector4[] lightings = LightingManager.CalcMesh();

            vao.UpdateData(vertices, elements, uvs, lightings);

        }


        public static void CleanUp() {
            Shader.DisposeChildren = true;
            Shader.Dispose();
        }

        #endregion

    }
}
