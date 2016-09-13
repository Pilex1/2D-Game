using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;
using Game.Assets;

namespace Game.Terrains {
    static class Terrain {

        public static LightingTexturedModel Model { get; private set; }

        internal static Tile[,] Tiles;
        internal static int[,] Lightings;
        internal static List<Light> Lights = new List<Light>();

        public static int MaxHeight { get; private set; }
        public static int MaxWidth { get; private set; }

        private static bool RecalculateMesh = false;

        private const int TerrainTextureSize = 16;

        public static void Init() {
            TerrainGen.Generate(314159);

            MaxWidth = Tiles.GetLength(0) - 1;
            MaxHeight = Tiles.GetLength(1) - 1;

            VBO<Vector2> vertices;
            VBO<int> elements;
            VBO<Vector2> uvs;
            VBO<float> lightings;
            CalculateMesh(Player.StartX,out vertices, out elements, out uvs, out lightings);


            Texture texture = new Texture(Asset.TextureFile);
            Model = new LightingTexturedModel(vertices, elements, BeginMode.TriangleStrip,PolygonMode.Fill, texture, uvs, lightings);

        }

        public static void CalculateMesh(int posX, out VBO<Vector2> vertices, out VBO<int> elements, out VBO<Vector2> uvs, out VBO<float> lightings) {
            List<Vector2> verticesList = new List<Vector2>();
            List<Vector2> uvList = new List<Vector2>();
            Lighting.CalculateLighting(posX);
            List<float> lightingsList = new List<float>();
            for (int i = (int)(posX-Renderer.zoom/2); i < (int)(posX+Renderer.zoom/2); i++) {

                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    if (Tiles[i, j] != Tile.Air) {
                        Tile t = Tiles[i, j];

                        float x = ((float)((int)t % TerrainTextureSize)) / TerrainTextureSize;
                        float y = ((float)((int)t / TerrainTextureSize)) / TerrainTextureSize;
                        float s = 1f / TerrainTextureSize;
                        float h = 1f / (TerrainTextureSize * TerrainTextureSize * 2);

                        //top left, bottom left, top right, bottom right

                        verticesList.AddRange(new Vector2[] {
                            new Vector2(i,j+1),
                            new Vector2(i,j),
                            new Vector2(i+1,j+1),
                            new Vector2(i+1,j)
                        });
                        uvList.AddRange(new Vector2[] {
                            new Vector2(x+h,y+s-h),
                            new Vector2(x+h,y+h),
                            new Vector2(x+s-h,y+s-h),
                            new Vector2(x+s-h,y+h)
                        });
                        float val = (float)Lightings[i, j] / Light.MaxLightLevel;
                        lightingsList.AddRange(new float[] {
                            val,val,val,val
                        });
                    }
                }
            }
            vertices = new VBO<Vector2>(verticesList.ToArray(), Hint: BufferUsageHint.DynamicDraw);
            uvs = new VBO<Vector2>(uvList.ToArray());
            lightings = new VBO<float>(lightingsList.ToArray());

            int[] elementsArr = new int[verticesList.Count];
            for (int i = 0; i < elementsArr.Length; i++) {
                elementsArr[i] = i;
            }
            elements = new VBO<int>(elementsArr, BufferTarget.ElementArrayBuffer);

        }

        public static void AddLight(Light l) {
            Lights.Add(l);
            RecalculateMesh = true;
        }

        public static bool WillCollide(Entity entity, Vector2 offset) {

            int x1 = (int)(entity.Position.x + offset.x);
            int x2 = (int)(entity.Position.x + entity.Hitbox.Width + offset.x) - 1;

            int y1 = (int)(entity.Position.y + offset.y);
            int y2 = (int)(entity.Position.y + entity.Hitbox.Height + offset.y) - 1;

            if (y1 <= 0 || y2 <= 0 || y1 > MaxHeight || y2 > MaxHeight || x1 < 0 || x2 < 0 || x1 > MaxWidth || x2 > MaxWidth) return true;

            for (int i = x1; i <= x2; i++) {
                if (Tiles[i, y1] != Tile.Air) {
                    return true;
                }
                if (Tiles[i, y2] != Tile.Air) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsColliding(Entity entity) { return WillCollide(entity, Vector2.Zero); }

        public static Vector2 CorrectTerrainCollision(Entity entity) {
            int height = -1;
            //find highest intersecting terrain
            for (int i = (int)entity.Position.x; i < entity.Position.x + entity.Hitbox.Width; i++) {
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
                if (Tiles[x, y + i] != Tile.Air) return false;
            }
            return true;
        }

        internal static int HighestPoint(int x) {
            for (int i = Tiles.GetLength(1) - 1; i > 0; i--) {
                if (Tiles[x, i] != Tile.Air) return i + 1;
            }
            return 1;
        }


        public static Tile TileAt(int x, int y) { return x < 0 || x > MaxWidth || y < 0 || y > MaxHeight ? Tile.Air : Tiles[x, y]; }
        public static Tile TileAt(float x, float y) { return TileAt((int)x, (int)y); }

        public static void BreakTile(int x, int y) {
            if (x < 0 || x > MaxWidth || y < 0 || y > MaxHeight) return;
            if (Tiles[x, y] == Tile.Bedrock) return;
            Tiles[x, y] = Tile.Air;
            RecalculateMesh = true;
        }

        public static void SetTile(int x, int y, Tile tile) {
            if (x < 0 || x > MaxWidth || y < 0 || y > MaxHeight) return;
            Tiles[x, y] = tile;
            RecalculateMesh = true;
        }

        public static void Update() {
            if (RecalculateMesh) {
                VBO<Vector2> vertices;
                VBO<int> elements;
                VBO<Vector2> uvs;
                VBO<float> lightings;
                CalculateMesh((int)Player.Instance.Position.x,out vertices, out elements, out uvs, out lightings);
                Model.Vertices = vertices;
                Model.Elements = elements;
                Model.Lightings = lightings;
                Model.UVs = uvs;
            }
            RecalculateMesh = false;
        }

    }
}
