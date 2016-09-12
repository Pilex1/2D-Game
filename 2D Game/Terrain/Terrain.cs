using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;

namespace Game {
    enum Tile {
        Air = -1, PurpleStone, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock
    }
    static class Terrain {

        public static LightingTexturedModel Model { get; private set; }

        internal static Tile[,] Tiles;
        internal static int[,] Lightings;
        public static int MaxHeight { get; private set; }
        public static int MaxWidth { get; private set; }

        private static bool TerrainChanged = false;

        private const int TerrainTextureSize = 16;

        public static void Init() {
            TerrainGen.Generate(314159);

            MaxWidth = Tiles.GetLength(0) - 1;
            MaxHeight = Tiles.GetLength(1) - 1;

            VBO<Vector2> vertices;
            VBO<int> elements;
            VBO<Vector2> uvs;
            VBO<float> lightings;
            CalculateMesh(out vertices, out elements, out uvs, out lightings);


            Texture texture = new Texture("Terrain/TerrainTextures.png");
            Model = new LightingTexturedModel(vertices, elements, BeginMode.TriangleStrip, texture, uvs, lightings);

        }

        public static void CalculateMesh(out VBO<Vector2> vertices, out VBO<int> elements, out VBO<Vector2> uvs, out VBO<float> lightings) {
            List<Vector2> verticesList = new List<Vector2>();
            List<Vector2> uvList = new List<Vector2>();
            TerrainGen.CalculateLighting();
            List<float> lightingsList = new List<float>();
            for (int i = 0; i < Tiles.GetLength(0); i++) {

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
                VBO<float> lightings;
                CalculateMesh(out vertices, out elements, out uvs, out lightings);
                Model.Vertices = vertices;
                Model.Elements = elements;
                Model.Lightings = lightings;
                Model.UVs = uvs;
            }
            TerrainChanged = false;
        }

    }

    internal static class TerrainGen {

        internal static Random Rand;

        private static void SetTile(int x, int y, Tile tile) {
            if (x < 0 || x >= Terrain.Tiles.GetLength(0) || y < 0 || y >= Terrain.Tiles.GetLength(1)) return;
            Terrain.Tiles[x, y] = tile;
        }

        private static void SetLighting(int x, int y, int lighting) {
            if (x < 0 || x >= Terrain.Lightings.GetLength(0) || y < 0 || y >= Terrain.Lightings.GetLength(1)) return;
            if (lighting > Terrain.Lightings[x, y]) Terrain.Lightings[x, y] = lighting;
        }

        internal static void Generate(int seed) {
            Rand = new Random(seed);

            GenerateTerrain();
        }

        #region Terrain
        private static void GenerateTerrain() {
            int size = 1000;
            int maxHeight = 20;
            int freq = 100;
            int wavelength = size / freq;
            Terrain.Tiles = new Tile[size, 256];

            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    SetTile(i, j, Tile.Air);
                }
            }

            float v1 = (float)(maxHeight * Rand.NextDouble()), v2 = (float)(maxHeight * Rand.NextDouble()), v3 = (float)(maxHeight * Rand.NextDouble()), v4 = (float)(maxHeight * Rand.NextDouble());

            for (int i = 0; i < freq; i++) {
                for (int j = 0; j < wavelength; j++) {
                    int y = 64 + CatmullRomCubicInterpolate(v1, v2, v3, v4, (float)j / wavelength);
                    if (y < 1) y = 1;
                    int x = i * wavelength + j;

                    for (int k = 0; k <= y; k++) {
                        if (k <= y - 10 + 3 * Rand.NextDouble()) SetTile(x, k, Tile.Stone);
                        else if (k <= y - 3 + 2 * Rand.NextDouble()) SetTile(x, k, Tile.Dirt);
                        else SetTile(x, k, Tile.Grass);
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
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int y = (int)(1 + 5 * Rand.NextDouble());
                for (int j = 0; j < y; j++) {
                    SetTile(i, j, Tile.Bedrock);
                }
            }
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
        #endregion Terrain

        #region Lighting
        internal static void CalculateLighting() {

            Terrain.Lightings = new int[Terrain.Tiles.GetLength(0), Terrain.Tiles.GetLength(1)];

            //calculate lightings for each tile
            //sun lighting
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int highest = Terrain.HighestPoint(i);
                SpreadLighting(i, highest, Light.MaxLightLevel);
            }
            //artificial lighting

        }


        private static void SpreadLighting(int x, int y, int strength) {
            for (int i = 0; i < strength; i++) {
                DiamondLighting(x, y, i, strength - i);
            }
        }

        private static void DiamondLighting(int x, int y, int radius, int strength) {
            int i = 0;
            for (int j = -radius; j <= radius; j++) {

                SetLighting(x - i, y + j, strength);
                SetLighting(x + i, y + j, strength);

                if (j < 0) i++;
                else i--;
            }
        }
        #endregion Lighting


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
