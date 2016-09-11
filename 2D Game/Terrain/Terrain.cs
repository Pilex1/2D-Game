using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;

namespace Game {
    enum Tile {
        Air = -1, PurpleStone, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock
    }
    static class Terrain {

        public static TexturedModel Model { get; private set; }

        private static Tile[,] Tiles;
        public static int MaxHeight { get; private set; }
        public static int MaxWidth { get; private set; }

        private static bool TerrainChanged = false;

        private const int TerrainTextureSize = 16;

        public static void Init() {
            Tiles = TerrainGen.Generate(314159);
            MaxWidth = Tiles.GetLength(0) - 1;
            MaxHeight = Tiles.GetLength(1) - 1;

            VBO<Vector2> vertices;
            VBO<int> elements;
            VBO<Vector2> uvs;
            CalculateMesh(out vertices, out elements, out uvs);

            Texture texture = new Texture("Terrain/TerrainTextures.png");
            Model = new TexturedModel(vertices, elements, BeginMode.TriangleStrip, texture, uvs);

        }

        public static void CalculateMesh(out VBO<Vector2> vertices, out VBO<int> elements, out VBO<Vector2> uvs) {
            List<Vector2> verticesList = new List<Vector2>();
            List<Vector2> uvList = new List<Vector2>();
            for (int i = 0; i < Tiles.GetLength(0); i++) {

                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    if (Tiles[i, j] != Tile.Air) {
                        Tile t = Tiles[i, j];

                        float x = ((float)((int)t % TerrainTextureSize)) / TerrainTextureSize;
                        float y = ((float)((int)t / TerrainTextureSize)) / TerrainTextureSize;
                        float s = 1f / TerrainTextureSize;
                        float h = 1f / (TerrainTextureSize * TerrainTextureSize * 2);
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
                    }
                }
            }
            vertices = new VBO<Vector2>(verticesList.ToArray(), Hint: BufferUsageHint.DynamicDraw);
            uvs = new VBO<Vector2>(uvList.ToArray());

            int[] elementsArr = new int[verticesList.Count];
            for (int i = 0; i < elementsArr.Length; i++) {
                elementsArr[i] = i;
            }
            elements = new VBO<int>(elementsArr, BufferTarget.ElementArrayBuffer);

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

        public static bool IsColliding(Entity entity) => WillCollide(entity, Vector2.Zero);

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


        public static Tile TileAt(int x, int y) => Tiles[x, y];
        public static Tile TileAt(float x, float y) => TileAt((int)x, (int)y);

        public static void BreakTile(int x, int y) {
            if (x < 0 || x > MaxWidth || y < 0 || y > MaxHeight) return;
            if (Tiles[x, y] == Tile.Bedrock) return;
            Tiles[x, y] = Tile.Air;
            TerrainChanged = true;
        }

        public static void SetTile(int x, int y, Tile tile) {
            if (x < 0 || x > MaxWidth || y < 0 || y > MaxHeight) return;
            Tiles[x, y] = tile;
            TerrainChanged = true;
        }

        public static void Update() {
            if (TerrainChanged) {
                VBO<Vector2> vertices;
                VBO<int> elements;
                VBO<Vector2> uvs;
                CalculateMesh(out vertices, out elements, out uvs);
                Model.Vertices = vertices;
                Model.Elements = elements;
                Model.UVs = uvs;
            }
            TerrainChanged = false;
        }

    }

    static class TerrainGen {

        private static Tile[,] Tiles;
        private static Random Rand;

        private static void SetTile(int x, int y, Tile tile) {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1)) return;
            Tiles[x, y] = tile;
        }

        internal static Tile[,] Generate(int seed) {
            Rand = new Random(seed);
            int size = 1000;
            int maxHeight = 20;
            int freq = 100;
            int wavelength = size / freq;
            Tiles = new Tile[size, 256];

            for (int i = 0; i < Tiles.GetLength(0); i++) {
                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    Tiles[i, j] = Tile.Air;
                }
            }

            float v1 = (float)(maxHeight * Rand.NextDouble()), v2 = (float)(maxHeight * Rand.NextDouble()), v3 = (float)(maxHeight * Rand.NextDouble()), v4 = (float)(maxHeight * Rand.NextDouble());

            for (int i = 0; i < freq; i++) {
                for (int j = 0; j < wavelength; j++) {
                    int y = 64 + CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / wavelength);
                    if (y < 1) y = 1;
                    int x = i * wavelength + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 10 + 3 * Rand.NextDouble()) Tiles[x, k] = Tile.Stone;
                        else if (k <= y - 3 + 2 * Rand.NextDouble()) Tiles[x, k] = Tile.Dirt;
                        else Tiles[x, k] = Tile.Grass;
                    }

                    if (x == 0) continue;
                    if (x % 20 == 0) Tree(x, y);
                }
                v1 = v2;
                v2 = v3;
                v3 = v4;
                v4 = (float)(maxHeight * Rand.NextDouble());
            }

            //generate bedrock
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                int y = (int)(1 + 5 * Rand.NextDouble());
                for (int j = 0; j < y; j++) {
                    Tiles[i, j] = Tile.Bedrock;
                }
            }

            return Tiles;
        }

        private static void Tree(int x, int y) {
            //generate wood
            int trunkHeight = (int)(5 + 3 * Rand.NextDouble());
            for (int i = y; i < y + trunkHeight; i++) {
                SetTile(x, i, Tile.Wood);
            }

            //generate leaves
            int r = (int)(4 + 2 * Rand.NextDouble());
            int yPointer = y + trunkHeight;

            for (int i = 0; i < 2 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    SetTile(x - j, yPointer, Tile.Leaf);
                    SetTile(x + j, yPointer, Tile.Leaf);
                }
                yPointer++;
            }

            r /= 2;
            for (int i = 0; i < 1 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    SetTile(x - j, yPointer, Tile.Leaf);
                    SetTile(x + j, yPointer, Tile.Leaf);
                }
                yPointer++;
            }

            r /= 2;
            if (r < 2) r = 2;
            for (int i = 0; i < 1 + 2 * Rand.NextDouble(); i++) {
                for (int j = 0; j < r; j++) {
                    SetTile(x - j, yPointer, Tile.Leaf);
                    SetTile(x + j, yPointer, Tile.Leaf);
                }
                yPointer++;
            }
        }

        private static int CatmullRomCubicInterpolate(float y0, float y1, float y2, float y3, float mu) {
            float a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = (float)(-0.5 * y0 + 1.5 * y1 - 1.5 * y2 + 0.5 * y3);
            a1 = (float)(y0 - 2.5 * y1 + 2 * y2 - 0.5 * y3);
            a2 = (float)(-0.5 * y0 + 0.5 * y2);
            a3 = y1;

            return (int)(a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }
    }
}
