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

        internal static bool terrainGenerated = false;

        public static bool UpdateMesh = true;
        public static LightingTexturedModel Model { get; private set; }

        public static Dictionary<Vector2i, LogicData> LogicDict = new Dictionary<Vector2i, LogicData>();

        internal static TileID[,] Tiles;
        internal static float[,] Lightings;

        public static int MaxHeight { get; private set; }
        public static int MaxWidth { get; private set; }

        private const int TerrainTextureSize = 16;

        private const float Epsilon = 0.001f;

        //temporary until i get more efficient lighting algorithms
        public static BoolSwitch UpdateLighting = true;

        internal static int[] Heights;

        public static void Init() {
            LoadTerrain();
            InitMesh();
            Heights = new int[Tiles.GetLength(0)];
            CalcHeights();
        }

        private static void LoadTerrain() {
            Tiles = Serialization.LoadTerrain();
            if (Tiles == null) {
                Console.WriteLine("Generating terrain...");
                Stopwatch watch = new Stopwatch();
                watch.Start();
                TerrainGen.Generate(4584);
                watch.Stop();
                Console.WriteLine("Terrain generation finished in " + watch.ElapsedMilliseconds + " ms");
            }
            terrainGenerated = true;
        }

        private static void CalcHeights() {
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                for (int j = Tiles.GetLength(1) - 1; j >= 0; j++) {
                    if (!TileAt(i, j).tileattribs.transparent) {
                        Heights[i] = j;
                        continue;
                    }
                }
            }
        }

        #region Mesh

        private static void InitMesh() {
            VBO<Vector2> vertices;
            VBO<int> elements;
            VBO<Vector2> uvs;
            CalculateMesh(out vertices, out elements, out uvs);

            Lighting.CalculateAllLighting();
            VBO<float> lightings;
            CalculateLightingMesh(out lightings);

            Texture texture = new Texture(Asset.TileTexture);
            Model = new LightingTexturedModel(vertices, elements, uvs, texture, lightings, BeginMode.Triangles, PolygonMode.Fill);
        }

        public static void CalculateLightingMesh(out VBO<float> lightings) {
            int posX, posY;
            if (Player.Instance != null) {
                posX = (int)Player.Instance.Position.x;
                posY = (int)Player.Instance.Position.y;
            } else {
                posX = Player.StartX;
                posY = Player.StartY;
            }

            int startX = (int)(posX + GameRenderer.zoom / 2), endX = (int)(posX - GameRenderer.zoom / 2);
            int startY = (int)(posY + GameRenderer.zoom / 2 / Program.AspectRatio), endY = (int)(posY - GameRenderer.zoom / 2 / Program.AspectRatio);
            List<float> lightingsSet = new List<float>();
            for (int i = startX >= 0 ? startX : 0; i <= (endX < Tiles.GetLength(0) ? endX : Tiles.GetLength(0) - 1); i++) {

                for (int j = startY >= 0 ? startY : 0; j <= (endY < Tiles.GetLength(1) ? endY : Tiles.GetLength(1) - 1); j++) {
                    if (Tiles[i, j].enumId != TileEnum.Air) {
                        float val = Lightings[i, j] / Light.MaxLightLevel;
                        lightingsSet.AddRange(new float[] {
                            val,val,val,val
                        });
                    }
                }
            }
            lightings = new VBO<float>(lightingsSet.ToArray());
        }


        public static void CalculateMesh(out VBO<Vector2> vertices, out VBO<int> elements, out VBO<Vector2> uvs) {
            int posX, posY;
            if (Player.Instance != null) {
                posX = (int)Player.Instance.Position.x;
                posY = (int)Player.Instance.Position.y;
            } else {
                posX = Player.StartX;
                posY = Player.StartY;
            }
            List<Vector2> verticesList = new List<Vector2>();
            List<Vector2> uvList = new List<Vector2>();

            int startX = (int)(posX + GameRenderer.zoom / 2), endX = (int)(posX - GameRenderer.zoom / 2);
            int startY = (int)(posY + GameRenderer.zoom / 2 / Program.AspectRatio), endY = (int)(posY - GameRenderer.zoom / 2 / Program.AspectRatio);
            for (int i = startX >= 0 ? startX : 0; i <= (endX < Tiles.GetLength(0) ? endX : Tiles.GetLength(0) - 1); i++) {
                for (int j = startY >= 0 ? startY : 0; j <= (endY < Tiles.GetLength(1) ? endY : Tiles.GetLength(1) - 1); j++) {
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
            vertices = new VBO<Vector2>(verticesList.ToArray(), Hint: BufferUsageHint.DynamicDraw);
            uvs = new VBO<Vector2>(uvList.ToArray(), Hint: BufferUsageHint.DynamicDraw);

            int[] elementsArr = new int[verticesList.Count / 4 * 6];
            for (int i = 0; i < verticesList.Count / 4; i++) {
                elementsArr[6 * i] = 4 * i;
                elementsArr[6 * i + 1] = 4 * i + 1;
                elementsArr[6 * i + 2] = 4 * i + 2;
                elementsArr[6 * i + 3] = 4 * i + 2;
                elementsArr[6 * i + 4] = 4 * i + 1;
                elementsArr[6 * i + 5] = 4 * i + 3;
            }
            elements = new VBO<int>(elementsArr, BufferTarget.ElementArrayBuffer, Hint: BufferUsageHint.DynamicDraw);
        }
        #endregion Mesh

        #region Collision
        public static bool WillCollide(Entity entity, Vector2 offset, out TileID collidedTile) {
            if (offset.x > 1) offset.x = 1;
            if (offset.x < -1) offset.x = -1;
            if (offset.y > 1) offset.y = 1;
            if (offset.y < -1) offset.y = -1;

            int x1 = (int)(entity.Position.x + offset.x);
            int x2 = (int)(entity.Position.x + entity.Hitbox.Width + offset.x - Epsilon);

            int y1 = (int)(entity.Position.y + offset.y);
            int y2 = (int)(entity.Position.y + entity.Hitbox.Height + offset.y - Epsilon);

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
            return WillCollide(entity, Vector2.Zero, out col);
        }

        public static Vector2 CorrectTerrainCollision(Entity entity) {
            int height = -1;
            //find highest intersecting terrain
            for (int i = (int)entity.Position.x; i < entity.Position.x + entity.Hitbox.Width - Epsilon; i++) {
                int curHeight = NonCollisionAbove(i, (int)entity.Position.y, (int)entity.Hitbox.Height);
                if (curHeight > height) height = curHeight;
            }
            if (height == -1) return entity.Position;
            return new Vector2(entity.Position.x, height);
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
            for (int i = 0; i < height; i++) {
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
        public static TileID TileAt(int x, int y) { return x < 0 || x > MaxWidth || y < 0 || y > MaxHeight ? TileID.Invalid : Tiles[x, y]; }
        public static TileID TileAt(float x, float y) { return TileAt((int)x, (int)y); }

        public static void SetTile(int x, int y, TileID tile) { SetTile(x, y, tile, true); }
        private static void SetTileNoUpdate(int x, int y, TileID tile) { SetTile(x, y, tile, false); }
        private static void SetTile(int x, int y, TileID tile, bool update) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return;
            if (Tiles[x, y].enumId != TileEnum.Air) return;
            Tiles[x, y] = tile;
            LogicData logic = tile.tileattribs as LogicData;
            if (logic != null && update) {
                LogicDict.Add(new Vector2i(x, y), logic);
            }
            UpdateMesh = true;
            if (terrainGenerated)
                Console.WriteLine(String.Format("{0} tile placed at {{{1}, {2}}}", tile.enumId.ToString(), x, y));
            if (y > Heights[x]) Heights[x] = y;
        }

        public static TileID BreakTile(int x, int y) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return TileID.Invalid;
            TileID tile = TileAt(x, y);
            if (tile.enumId == TileEnum.Air) return TileID.Air;
            if (tile.enumId == TileEnum.Bedrock) return TileID.Invalid;
            LogicDict.Remove(new Vector2i(x, y));
            Tiles[x, y] = TileID.Air;
            UpdateMesh = true;
            if (terrainGenerated)
                Console.WriteLine(String.Format("{0} tile removed at {{{1}, {2}}}", tile.enumId.ToString(), x, y));
            if (y == Heights[x]) {
                for (int j = y - 1; j >= 0; j--) {
                    if (!TileAt(x, j).tileattribs.transparent)
                        Heights[x] = j;
                }
            }
            return tile;
        }

        public static TileID BreakTile(float x, float y) {
            return BreakTile((int)x, (int)y);
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
                    }
                    break;
            }
        }

        public static void Update() {

            // FluidsManager.Update();
            LogicManager.Update();

            if (UpdateMesh) {
                VBO<Vector2> vertices;
                VBO<int> elements;
                VBO<Vector2> uvs;
                CalculateMesh(out vertices, out elements, out uvs);
                Model.Vertices = vertices;
                Model.Elements = elements;
                Model.UVs = uvs;

                if (UpdateLighting) {
                    Lighting.CalculateLighting();
                }
                VBO<float> lightings;
                CalculateLightingMesh(out lightings);
                Model.Lightings = lightings;

            }
            UpdateMesh = false;

        }


        public static void CleanUp() {
            //serialise terrain
            Serialization.SaveTerrain(Tiles);
        }

    }
}
