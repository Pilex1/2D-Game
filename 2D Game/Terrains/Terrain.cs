using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;
using Game.Assets;
using Game.Util;
using System.Diagnostics;
using System.Linq;
using Game.Fluids;
using Game.Logics;
using Game.Core;

namespace Game.Terrains {

    static class Terrain {

        private static Random Rand = new Random();

        internal static bool terrainGenerated = false;

        public static bool UpdateMesh = true;
        public static TerrainVAO vao;
        public static Texture texture;

        public static Dictionary<Vector2i, LogicData> LogicDict = new Dictionary<Vector2i, LogicData>();

        internal static TileID[,] Tiles;

        internal static int[] Heights;

        private const int TerrainTextureSize = 16;

        private const float Epsilon = 0.001f;

        public static ShaderProgram TerrainShader { get; private set; }

        public static void CreateNew() {
            Console.WriteLine("Generating terrain...");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            TerrainGen.Generate(MathUtil.RandInt(new Random(), 0, Int32.MaxValue >> 1));
            watch.Stop();
            Console.WriteLine("Terrain generation finished in " + watch.ElapsedMilliseconds + " ms");
            Init();
        }

        public static bool Load() {
            try {
                Tiles = Serialization.LoadTerrain();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            Init();
            return true;
        }


        public static void UpdateViewMatrix(Matrix4 mat) {
            if (TerrainShader == null)
                throw new ArgumentNullException("Terrain shader null");
            Gl.UseProgram(TerrainShader.ProgramID);
            TerrainShader["viewMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }

        private static void Init() {
            TerrainShader = new ShaderProgram(Asset.TerrainVert, Asset.TerrainFrag);
            Console.WriteLine("Terrain Shader Log: ");
            Console.WriteLine(TerrainShader.ProgramLog);

            InitHeights();
            Lighting.Init();
            InitMesh();
            terrainGenerated = true;
        }

        private static void InitHeights() {
            Heights = new int[Tiles.GetLength(0)];
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                for (int j = Tiles.GetLength(1) - 1; j > 0; j--) {
                    if (!Tiles[i, j].tileattribs.transparent) {
                        Heights[i] = j;
                        break;
                    }
                }
            }
        }

        public static void SetProjectionMatrix(Matrix4 mat) {
            if (TerrainShader == null)
                throw new ArgumentNullException("Terrain shader null");
            Gl.UseProgram(TerrainShader.ProgramID);
            TerrainShader["projectionMatrix"].SetValue(mat);
            Gl.UseProgram(0);
        }

        public static void Render() {
            Gl.UseProgram(TerrainShader.ProgramID);
            Gl.BindVertexArray(vao.ID);
            Gl.BindTexture(texture.TextureTarget, texture.TextureID);
            Gl.DrawElements(BeginMode.Triangles, vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(texture.TextureTarget, 0);
            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        #region Mesh

        private static void InitMesh() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            CalculateMesh(out vertices, out elements, out uvs);

            Lighting.Init();
            float[] lightings = Lighting.CalcMesh();

            texture = Asset.TerrainTexture;

            vao = new TerrainVAO(vertices, elements, uvs, lightings);
        }

        public static void Range(out int minx, out int maxx, out int miny, out int maxy) {
            float posX, posY;
            if (Player.Instance != null) {
                posX = (int)Player.Instance.data.Position.x;
                posY = (int)Player.Instance.data.Position.y;
            } else {
                posX = Player.StartX;
                posY = Player.StartY;
            }

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
            List<Vector2> verticesList = new List<Vector2>((endX - startX) * (endY - startY));
            List<Vector2> uvList = new List<Vector2>((endX - startX) * (endY - startY));
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    if (Tiles[i, j].enumId != TileEnum.Air) {
                        TileEnum t = Tiles[i, j].enumId;

                        float x = ((float)((int)t % TerrainTextureSize)) / TerrainTextureSize;
                        float y = ((float)((int)t / TerrainTextureSize)) / TerrainTextureSize;
                        float s = 1f / TerrainTextureSize;
                        //half pixel correction
                        float h = 1f / (TerrainTextureSize * TerrainTextureSize * 2);

                        float height = 1;
                        //TODO
                        //something like
                        //float height = t.tiledata is Fluid ? ((Fluid)(t.tiledata)).height : 1;

                        //top left, bottom left, top right, bottom right
                        verticesList.AddRange(new Vector2[] {
                            new Vector2(i,j+height),
                            new Vector2(i,j),
                            new Vector2(i+1,j+height),
                            new Vector2(i+1,j)
                        });
                        uvList.AddRange(new Vector2[] {
                            new Vector2(x+h,y+s-h),
                            new Vector2(x+h,y+h),
                            new Vector2(x+s-h,y+s-h),
                            new Vector2(x+s-h,y+h)
                        });
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
                elements[6 * i + 4] = 4 * i + 1;
                elements[6 * i + 5] = 4 * i + 3;
            }
        }
        #endregion Mesh

        #region Collision
        public static bool WillCollide(Entity entity, Vector2 offset, out TileID collidedTile) { return WillCollide(entity.Hitbox, offset, out collidedTile); }
        public static bool WillCollide(Hitbox hitbox, Vector2 offset, out TileID collidedTile) {
            if (offset.x > 1) offset.x = 1;
            if (offset.x < -1) offset.x = -1;
            if (offset.y > 1) offset.y = 1;
            if (offset.y < -1) offset.y = -1;

            int x1 = (int)(hitbox.Position.x + offset.x);
            int x2 = (int)(hitbox.Position.x + hitbox.Width + offset.x - Epsilon);

            int y1 = (int)(hitbox.Position.y + offset.y);
            int y2 = (int)(hitbox.Position.y + hitbox.Height + offset.y - Epsilon);

            for (int i = x1; i <= x2; i++) {
                for (int j = y1; j <= y2; j++) {
                    if (TileAt(i, j).tileattribs.solid) {
                        collidedTile = TileAt(i, j);
                        return true;
                    }
                }
            }
            collidedTile = TileID.Air;
            return false;
        }

        public static bool IsColliding(Entity entity) {
            TileID col;
            return IsColliding(entity, out col);
        }

        public static bool IsColliding(Hitbox h) {
            TileID col;
            return IsColliding(h, out col);
        }

        public static bool IsColliding(Hitbox h, out TileID col) {
            return WillCollide(h, Vector2.Zero, out col);
        }

        public static bool IsColliding(Entity entity, out TileID col) {
            return IsColliding(entity.Hitbox, out col);
        }

        public static Vector2 CorrectTerrainCollision(Entity entity) {
            int height = -1;
            //find highest intersecting terrain
            for (int i = (int)entity.data.Position.x; i < entity.data.Position.x + entity.Hitbox.Width - Epsilon; i++) {
                int curHeight = NonCollisionAbove(i, (int)entity.data.Position.y, (int)entity.Hitbox.Height);
                if (curHeight > height) height = curHeight;
            }
            if (height == -1) return entity.data.Position.val;
            return new Vector2(entity.data.Position.x, height);
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
                if (y + i >= Tiles.GetLength(1)) return true;
                if (Tiles[x, y + i].enumId != TileEnum.Air) return false;
            }
            return true;
        }

        internal static int HighestPoint(int x) {
            for (int i = Tiles.GetLength(1) - 1; i > 0; i--) {
                if (Tiles[x, i].enumId != TileEnum.Air) return i + 1;
            }
            return 1;
        }
        #endregion Collision

        public static TileID TileAt(Vector2i v) { return TileAt(v.x, v.y); }
        public static TileID TileAt(int x, int y) { return x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1) ? TileID.Invalid : Tiles[x, y]; }
        public static TileID TileAt(float x, float y) { return TileAt((int)x, (int)y); }

        public static void Explode(float x, float y, float radius, float error) {
            for (float i = -radius + MathUtil.RandFloat(Rand, -error, error); i <= radius + MathUtil.RandFloat(Rand, -error, error); i++) {
                for (float j = -radius + MathUtil.RandFloat(Rand, -error, error); j <= radius + MathUtil.RandFloat(Rand, -error, error); j++) {
                    if (i * i + j * j <= radius * radius) {
                        if (BreakTileNoLightingUpdate((int)(x + i), (int)(j + y)).enumId == TileEnum.Tnt) Explode(x + i, j + y, radius, error);
                    }
                }
            }
            UpdateMesh = true;
            Lighting.RecalcAll();
        }

        public static void SetTile(int x, int y, TileID tile) { SetTile(x, y, tile, true); }
        private static void SetTileNoUpdate(int x, int y, TileID tile) { SetTile(x, y, tile, false); }
        private static void SetTile(int x, int y, TileID tile, bool update) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return;
            if (Tiles[x, y].enumId != TileEnum.Air) return;
            int xmin = (int)Math.Floor(Player.Instance.data.Position.x), xmax = (int)Math.Ceiling(Player.Instance.data.Position.x + Player.Instance.Hitbox.Size.x - 1);
            int ymin = (int)Math.Floor(Player.Instance.data.Position.y), ymax = (int)Math.Ceiling(Player.Instance.data.Position.y + Player.Instance.Hitbox.Size.y - 1);
            if (x >= xmin && x <= xmax && y >= ymin && y <= ymax) return;

            Tiles[x, y] = tile;
            LogicData logic = tile.tileattribs as LogicData;
            if (logic != null && update) {
                LogicDict.Add(new Vector2i(x, y), logic);
            }
            UpdateMesh = true;
            if (terrainGenerated) { }
                //Debug.WriteLine(String.Format("{0} tile placed at {{{1}, {2}}}", tile.enumId.ToString(), x, y));
            if (!tile.tileattribs.transparent && y > Heights[x]) Heights[x] = y;
            Lighting.UpdateAround(x, y);
        }

        public static TileID BreakTile(int x, int y) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return TileID.Invalid;
            TileID tile = TileAt(x, y);
            if (tile.enumId == TileEnum.Air) return TileID.Air;
            if (tile.enumId == TileEnum.Bedrock) return TileID.Invalid;
            LogicDict.Remove(new Vector2i(x, y));
            Tiles[x, y] = TileID.Air;
            UpdateMesh = true;
            if (terrainGenerated) { }
                //Debug.WriteLine(String.Format("{0} tile removed at {{{1}, {2}}}", tile.enumId.ToString(), x, y));
            if (Heights[x] == y) {
                for (int j = y - 1; j > 0; j--) {
                    if (!Tiles[x, j].tileattribs.transparent) {
                        Heights[x] = j;
                        break;
                    }
                }
            }
            Lighting.UpdateAround(x, y);
            return tile;
        }
        public static TileID BreakTile(float x, float y) {
            return BreakTile((int)x, (int)y);
        }
        private static TileID BreakTileNoLightingUpdate(int x, int y) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return TileID.Invalid;
            TileID tile = TileAt(x, y);
            if (tile.enumId == TileEnum.Air) return TileID.Air;
            if (tile.enumId == TileEnum.Bedrock) return TileID.Invalid;
            LogicDict.Remove(new Vector2i(x, y));
            Tiles[x, y] = TileID.Air;
            UpdateMesh = true;
            // if (terrainGenerated)
            // Console.WriteLine(String.Format("{0} tile removed at {{{1}, {2}}}", tile.enumId.ToString(), x, y));
            if (Heights[x] == y) {
                for (int j = y - 1; j > 0; j--) {
                    if (!Tiles[x, j].tileattribs.transparent) {
                        Heights[x] = j;
                        break;
                    }
                }
            }
            return tile;
        }

        public static void MoveTile(int x, int y, Direction dir) {
            Vector2i v = new Vector2i(x, y);
            switch (dir) {
                case Direction.Left:
                    if (TileAt(x - 1, y).enumId == TileEnum.Air) {
                        LogicData logic;
                        if (LogicDict.TryGetValue(v, out logic)) {
                            LogicDict.Remove(v);
                            LogicDict.Add(new Vector2i(x - 1, y), logic);
                        }
                        SetTileNoUpdate(x - 1, y, TileAt(x, y));
                        BreakTile(x, y);
                        UpdateMesh = true;
                        Lighting.UpdateAround(x - 1, y);
                    }
                    break;
                case Direction.Right:
                    if (TileAt(x + 1, y).enumId == TileEnum.Air) {
                        LogicData logic;
                        if (LogicDict.TryGetValue(v, out logic)) {
                            LogicDict.Remove(v);
                            LogicDict.Add(new Vector2i(x + 1, y), logic);
                        }
                        SetTileNoUpdate(x + 1, y, TileAt(x, y));
                        BreakTile(x, y);
                        UpdateMesh = true;
                        Lighting.UpdateAround(x + 1, y);
                    }
                    break;
                case Direction.Up:
                    if (TileAt(x, y + 1).enumId == TileEnum.Air) {
                        LogicData logic;
                        if (LogicDict.TryGetValue(v, out logic)) {
                            LogicDict.Remove(v);
                            LogicDict.Add(new Vector2i(x, y + 1), logic);
                        }
                        SetTileNoUpdate(x, y + 1, TileAt(x, y));
                        BreakTile(x, y);
                        UpdateMesh = true;
                        Lighting.UpdateAround(x, y + 1);
                    }
                    break;
                case Direction.Down:
                    if (TileAt(x, y - 1).enumId == TileEnum.Air) {
                        LogicData logic;
                        if (LogicDict.TryGetValue(v, out logic)) {
                            LogicDict.Remove(v);
                            LogicDict.Add(new Vector2i(x, y - 1), logic);
                        }
                        SetTileNoUpdate(x, y - 1, TileAt(x, y));
                        BreakTile(x, y);
                        UpdateMesh = true;
                        Lighting.UpdateAround(x, y - 1);
                    }
                    break;
            }
            Lighting.UpdateAround(x, y);

        }

        public static void Update() {

            // FluidsManager.Update();
            LogicManager.Update();

            if (UpdateMesh) {
                Vector2[] vertices;
                int[] elements;
                Vector2[] uvs;
                CalculateMesh(out vertices, out elements, out uvs);

                float[] lightings = Lighting.CalcMesh();

                vao.UpdateData(vertices, elements, uvs, lightings);
            }
            UpdateMesh = false;

        }


        public static void CleanUp() {

            TerrainShader.DisposeChildren = true;
            TerrainShader.Dispose();

            //serialise terrain
            Serialization.SaveTerrain(Tiles);
        }

    }
}
