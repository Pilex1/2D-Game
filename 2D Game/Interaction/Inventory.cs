using Game.Assets;
using Game.Entities;
using Game.Util;
using OpenGL;
using System;
using System.Drawing;

namespace Game.Interaction {
    static class Hotbar {
        public static TexturedModel Frame;
        public static TexturedModel TexturedItems;
        public static TexturedModel CurSelected;

        public static int CurSelectedSlot { get; private set; }

        public const float SizeX = 2 * 64f / Program.Width;
        public const float SizeY = 2 * 64f / Program.Height;
        public const float ItemTextureOffset = 0.01f;

        private const int ItemTextureSize = 16;

        internal static void Init() {
            CurSelectedSlot = 0;
            InitFrame();
            InitTexturedItems();
            CurSelected = TexturedModel.CreateWireframeRectangle(new Vector2(SizeX, SizeY), new Vector4(0, 0, 1, 1));
        }

        private static void InitFrame() {
            Vector2[] verticesArr = new Vector2[2 * (Inventory.InvColumns + 1)];
            for (int i = 0; i < Inventory.InvColumns + 1; i++) {
                verticesArr[i * 2] = new Vector2(i * SizeX, SizeY);
                verticesArr[i * 2 + 1] = new Vector2(i * SizeX, 0);
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

            Texture texture = ColourUtil.TexFromColour(new Vector4(0,0,0.1,1));
            Vector2[] uvs = new Vector2[verticesArr.Length];
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new Vector2(0, 0);
            }

            Frame = new TexturedModel(vertices, elements, new VBO<Vector2>(uvs), texture, BeginMode.Lines, PolygonMode.Line);
        }

        private static void InitTexturedItems() {
            Vector2[] verticesArr = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                //topleft, bottomleft, topright, bottomright
                verticesArr[i * 4] = new Vector2(i * SizeX + ItemTextureOffset, SizeY - ItemTextureOffset);
                verticesArr[i * 4 + 1] = new Vector2(i * SizeX + ItemTextureOffset, ItemTextureOffset);
                verticesArr[i * 4 + 2] = new Vector2((i + 1) * SizeX - ItemTextureOffset, SizeY - ItemTextureOffset);
                verticesArr[i * 4 + 3] = new Vector2((i + 1) * SizeX - ItemTextureOffset, ItemTextureOffset);
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

            VBO<Vector2> uvs = CalcTexturedItemsUV();

            Texture texture = new Texture(Asset.ItemFile);

            TexturedItems = new TexturedModel(vertices, elements, uvs, texture, BeginMode.Triangles, PolygonMode.Fill);
        }

        private static VBO<Vector2> CalcTexturedItemsUV() {
            Vector2[] uvsArr = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                Item t = Inventory.Items[i, 0].Item1;
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
            return uvs;
        }

        public static void IncrSlot() {
            CurSelectedSlot++;
            if (CurSelectedSlot >= Inventory.InvColumns) CurSelectedSlot = 0;
        }

        public static void DecrSlot() {
            CurSelectedSlot--;
            if (CurSelectedSlot < 0) CurSelectedSlot = Inventory.InvColumns - 1;
        }

        public static void Update() {
            VBO<Vector2> uvs = CalcTexturedItemsUV();
            TexturedItems.UVs = uvs;
        }

        public static Item CurrentlySelectedItem() {
            return Inventory.Items[CurSelectedSlot, 0].Item1;
        }
    }
    static class Inventory {
        internal const int InvColumns = 9, InvRows = 6;
        internal static Tuple<Item, uint>[,] Items = new Tuple<Item, uint>[InvColumns, InvRows];

        public static void Init() {
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<Item, uint>(Item.None, 0);
                }
            }

            Hotbar.Init();

            for (int i = 0; i < 5; i++) {
                Items[i, 0] = new Tuple<Item, uint>((Item)i, 1);
            }
            Items[5, 0] = new Tuple<Item, uint>(Item.Tnt, 1);
            Items[6, 0] = new Tuple<Item, uint>(Item.Sapling, 1);

            Hotbar.Update();
        }
    }
}
