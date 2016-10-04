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

        public static int CurSelectedSlot { get; set; }

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

            Texture texture = ColourUtil.TexFromColour(new Vector4(0, 0, 0.1, 1));
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

            Texture texture = new Texture(Asset.ItemTexture);

            TexturedItems = new TexturedModel(vertices, elements, uvs, texture, BeginMode.Triangles, PolygonMode.Fill);
        }

        private static VBO<Vector2> CalcTexturedItemsUV() {
            Vector2[] uvsArr = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                ItemId t = Inventory.Items[0, i].Item1;
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

        public static ItemId CurrentlySelectedItem() {
            return Inventory.Items[0, CurSelectedSlot].Item1;
        }
    }
    static class Inventory {

        public static TexturedModel Frame;
        public static TexturedModel TexturedItems;
        public static TexturedModel CurSelected;

        internal const int InvColumns = 9, InvRows = 6;
        public static Tuple<ItemId, uint>[,] Items = new Tuple<ItemId, uint>[InvRows, InvColumns];

        public static void Init() {
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<ItemId, uint>(ItemId.None, 0);
                }
            }

            Hotbar.Init();
            
            Items[0, 0] = new Tuple<ItemId, uint>(ItemId.Switch, 1);
            Items[0, 1] = new Tuple<ItemId, uint>(ItemId.Wire, 1);
            Items[0, 2] = new Tuple<ItemId, uint>(ItemId.LogicLamp, 1);
            Items[0, 3] = new Tuple<ItemId, uint>(ItemId.GateAnd, 1);
            Items[0, 4] = new Tuple<ItemId, uint>(ItemId.GateOr, 1);
            Items[0, 5] = new Tuple<ItemId, uint>(ItemId.GateNot, 1);
            Items[0, 6] = new Tuple<ItemId, uint>(ItemId.LogicBridge, 1);
            Items[0, 7] = new Tuple<ItemId, uint>(ItemId.StickyTilePusher, 1);
           Items[0, 8] = new Tuple<ItemId, uint>(ItemId.StickyTilePuller, 1);

            Hotbar.Update();
        }

        private static void InitFrame() {
            Vector2[] verticesArr = new Vector2[(InvColumns + 1) * (InvRows + 1)];
            int ptr = 0;
            for (int i = 0; i < InvColumns + 1; i++) {
                for (int j = 0; j < InvRows + 1; j++) {
                    verticesArr[i] = new Vector2(i * Hotbar.SizeX, j * Hotbar.SizeY);
                    ptr++;
                }
            }

            int[] elementsArr = new int[2 * (InvColumns + 1) + 2 * (InvRows + 1)];
            ptr = 0;

        }
    }
}
