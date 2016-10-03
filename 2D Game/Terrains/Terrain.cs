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

        public static bool UpdateMesh = true;

        public static LightingTexturedModel Model { get; private set; }

        internal static Tile[,] Tiles;
        internal static float[,] Lightings;

        public static int MaxHeight { get; private set; }
        public static int MaxWidth { get; private set; }

        private const int TerrainTextureSize = 16;

        private const float Epsilon = 0.001f;

        //temporary until i get more efficient lighting algorithms
        public static BoolSwitch UpdateLighting = true;


        public static void Init() {
            //TODO: deserialsie terrrain;
            TerrainGen.Generate(4584);

            MaxWidth = Tiles.GetLength(0) - 1;
            MaxHeight = Tiles.GetLength(1) - 1;

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
                    if (Tiles[i, j].id != TileID.Air) {
                        float val = Lightings[i, j] / Light.MaxLightLevel;
                        lightingsSet.AddRange(new float[] {
                            val,val,val,val
                        });
                    }
                }
            }
            lightings = new VBO<float>(lightingsSet.ToArray());
        }

        public static void CleanUp() {
            //serialise terrain
            //  Serialization.SaveTerrain(Tiles);
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
                    if (Tiles[i, j].id != TileID.Air) {
                        Tile t = Tiles[i, j];

                        float x = ((float)((int)t.id % TerrainTextureSize)) / TerrainTextureSize;
                        float y = ((float)((int)t.id / TerrainTextureSize)) / TerrainTextureSize;
                        float s = 1f / TerrainTextureSize;
                        //half pixel correction
                        float h = 1f / (TerrainTextureSize * TerrainTextureSize * 2);

                        float height = t is Fluid ? ((Fluid)t).Height : 1f;

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

        public static bool WillCollide(Entity entity, Vector2 offset, out Tile collidedTile) {
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
                    if (TileAt(i, j) is ISolid) {
                        collidedTile = TileAt(i, j);
                        return true;
                    }
                }
            }
            collidedTile = new Air(0, 0);
            return false;
        }

        public static bool IsColliding(Entity entity) {
            Tile col;
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
                if (Tiles[x, y + i].id != TileID.Air) return false;
            }
            return true;
        }

        internal static int HighestPoint(int x) {
            for (int i = Tiles.GetLength(1) - 1; i > 0; i--) {
                if (Tiles[x, i].id != TileID.Air) return i + 1;
            }
            return 1;
        }

        public static Tile TileAt(int x, int y) { return x < 0 || x > MaxWidth || y < 0 || y > MaxHeight ? new Air(0, 0) : Tiles[x, y]; }
        public static Tile TileAt(float x, float y) { return TileAt((int)x, (int)y); }

        public static Tile BreakTile(Tile t) {
            return BreakTile(t.x, t.y);
        }
        public static Tile BreakTile(int x, int y) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return new Invalid();
            Tile res = TileAt(x, y);
            if (res.id == TileID.Bedrock) return new Invalid();
            if (res is Fluid) FluidsManager.RemoveFluid((Fluid)res);
            if (res is Logic) LogicManager.RemoveLogic((Logic)res);
            Tiles[x, y] = new Air(x, y);
            UpdateMesh = true;
            return res;
        }

        internal static Tile ReplaceTile(int x, int y, TileID id) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return new Invalid();
            if (id == TileID.Bedrock) return new Invalid();
            Tile res = TileAt(x, y);
            Tiles[x, y].id = id;
            return res;
        }

        public static void Update() {


            FluidsManager.Update();
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

    }
}
