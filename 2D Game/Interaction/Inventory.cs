using System;
using OpenGL;
using Game.Entities;
using Game.Assets;

namespace Game.Interaction {
    static class Hotbar {
        public static ColouredModel Frame;
        public static ColouredModel Background;
        public static TexturedModel TexturedItems;

        public const float Size = 0.15f;
        public const float ItemTextureOffset = 0.01f;

        private const int ItemTextureSize = 16;

        internal static void Init() {
            InitFrame();
            Background = ColouredModel.CreateRectangle(new Vector2(Size * Inventory.InvColumns, Size), new Vector4(0.9f, 0.85f, 1, 0), PolygonMode.Fill);
            InitTexturedItems();
        }

        private static void InitFrame() {
            Vector2[] verticesArr = new Vector2[2 * (Inventory.InvColumns + 1)];
            for (int i = 0; i < Inventory.InvColumns + 1; i++) {
                verticesArr[i * 2] = new Vector2(i * Size, Size);
                verticesArr[i * 2 + 1] = new Vector2(i * Size, 0);
            }
            VBO<Vector2> vertices = new VBO<Vector2>(verticesArr);

            int[] elementsArr = new int[2 * (Inventory.InvColumns + 1) + 4];
            for (int i = 0; i < 2 * (Inventory.InvColumns + 1); i++) {
                elementsArr[i] = i;
            }
            elementsArr[2 * (Inventory.InvColumns + 1)] = 0;
            elementsArr[2 * (Inventory.InvColumns + 1) + 1] = 2 * (Inventory.InvColumns + 1) - 2;
            elementsArr[2 * (Inventory.InvColumns + 1) + 2] = 1;
            elementsArr[2 * (Inventory.InvColumns + 1) + 3] = 2 * (Inventory.InvColumns + 1) - 1;
            VBO<int> elements = new VBO<int>(elementsArr, BufferTarget.ElementArrayBuffer);

            Vector4[] coloursArr = new Vector4[verticesArr.GetLength(0)];
            for (int i = 0; i < coloursArr.GetLength(0); i++) {
                coloursArr[i] = new Vector4(0, 0, 0.1, 0);
            }
            VBO<Vector4> colours = new VBO<Vector4>(coloursArr);

            Frame = new ColouredModel(vertices, elements, colours, BeginMode.Lines, PolygonMode.Line);
        }

        private static void InitTexturedItems() {
            Vector2[] verticesArr = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                //topleft, bottomleft, topright, bottomright
                verticesArr[i * 4] = new Vector2(i * Size + ItemTextureOffset, Size - ItemTextureOffset);
                verticesArr[i * 4 + 1] = new Vector2(i * Size + ItemTextureOffset, ItemTextureOffset);
                verticesArr[i * 4 + 2] = new Vector2((i + 1) * Size - ItemTextureOffset, Size - ItemTextureOffset);
                verticesArr[i * 4 + 3] = new Vector2((i + 1) * Size - ItemTextureOffset, ItemTextureOffset);
            }
            VBO<Vector2> vertices = new VBO<Vector2>(verticesArr);

            int[] elementsArr = new int[(verticesArr.GetLength(0) / 4) * 6];
            for (int i = 0; i < (verticesArr.GetLength(0) / 4); i++) {
                elementsArr[i * 6] = (i * 4);
                elementsArr[i * 6 + 1] = (i * 4 + 1);
                elementsArr[i * 6 + 2] = (i * 4 + 2);
                elementsArr[i * 6 + 3] = (i * 4 + 2);
                elementsArr[i * 6 + 4] = (i * 4 + 1);
                elementsArr[i * 6 + 5] = (i * 4 + 3);
            }
            VBO<int> elements = new VBO<int>(elementsArr, BufferTarget.ElementArrayBuffer);

            Vector2[] uvsArr = new Vector2[verticesArr.GetLength(0)];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                Item t = (Item)i;
                float x = ((float)((int)t % ItemTextureSize)) / ItemTextureSize;
                float y = ((float)((int)t / ItemTextureSize)) / ItemTextureSize;
                float s = 1f / ItemTextureSize;
                float h = 1f / (ItemTextureSize * ItemTextureSize * 2);
                uvsArr[i * 4] = new Vector2(x + h, y + s - h);
                uvsArr[i * 4 + 1] = new Vector2(x + h, y + h);
                uvsArr[i * 4 + 2] = new Vector2(x + s - h, y + s - h);
                uvsArr[i * 4 + 3] = new Vector2(x + s - h, y + h);
            }
            VBO<Vector2> uvs = new VBO<Vector2>(uvsArr);

            Texture texture = new Texture(Asset.ItemFile);

            TexturedItems = new TexturedModel(vertices, elements, uvs, texture, BeginMode.Triangles, PolygonMode.Fill);
        }

    }
    static class Inventory {
        internal const int InvColumns = 9, InvRows = 6;
        internal static Tuple<Item, uint>[,] items = new Tuple<Item, uint>[InvColumns, InvRows];

        public static void Init() {
            Hotbar.Init();
        }
    }
    enum Item {
        PurpleStone, Grass, Sand, Dirt, Wood, Leaf, Stone, Bedrock, Sword
    }
}
