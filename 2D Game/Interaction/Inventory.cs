using Game.Assets;
using Game.Core;
using Game.Entities;
using Game.TitleScreen;
using Game.Util;
using OpenGL;
using System;
using System.Drawing;

namespace Game.Interaction {

    static class Hotbar {
        internal static GuiVAO Frame;
        internal static Texture FrameTexture;

        internal static GuiVAO TexturedItems;
        internal static Texture TexturedItemsTexture;

        internal static GuiVAO CurSelected;
        internal static Texture CurSelectedTexture;

        public static int CurSelectedSlot { get; set; }

        public const float SizeX = 0.1f;
        public const float SizeY = 0.1f;
        public const float ItemTextureOffset = 0.01f;

        private const int ItemTextureSize = 16;

        internal static void Init() {
            CurSelectedSlot = 0;

            Frame = InitFrame();
            FrameTexture = TextureUtil.CreateTexture(new Vector3(0, 0, 0.1));

            TexturedItems = InitTexturedItems();
            TexturedItemsTexture = new Texture(Asset.ItemTexture);

            CurSelected = GuiVAO.CreateWireRectangle(new Vector2(SizeX, SizeY), uvhint: BufferUsageHint.DynamicDraw);
            CurSelectedTexture = TextureUtil.CreateTexture(new Vector3(0, 0, 1));
        }

        private static GuiVAO InitFrame() {
            Vector2[] vertices = new Vector2[2 * (Inventory.InvColumns + 1)];
            for (int i = 0; i < Inventory.InvColumns + 1; i++) {
                vertices[i * 2] = new Vector2(i * SizeX, SizeY);
                vertices[i * 2 + 1] = new Vector2(i * SizeX, 0);
            }

            int[] elements = new int[2 * (Inventory.InvColumns + 1) + 4];
            for (int i = 0; i < 2 * (Inventory.InvColumns + 1); i++) {
                elements[i] = i;
            }
            elements[2 * (Inventory.InvColumns + 1)] = 0;
            elements[2 * (Inventory.InvColumns + 1) + 1] = 2 * (Inventory.InvColumns + 1) - 2;
            elements[2 * (Inventory.InvColumns + 1) + 2] = 1;
            elements[2 * (Inventory.InvColumns + 1) + 3] = 2 * (Inventory.InvColumns + 1) - 1;

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new Vector2(0, 0);
            }

            return new GuiVAO(vertices, elements, uvs);
        }

        private static GuiVAO InitTexturedItems() {
            Vector2[] vertices = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                //topleft, bottomleft, topright, bottomright
                vertices[i * 4] = new Vector2(i * SizeX + ItemTextureOffset, SizeY - ItemTextureOffset);
                vertices[i * 4 + 1] = new Vector2(i * SizeX + ItemTextureOffset, ItemTextureOffset);
                vertices[i * 4 + 2] = new Vector2((i + 1) * SizeX - ItemTextureOffset, SizeY - ItemTextureOffset);
                vertices[i * 4 + 3] = new Vector2((i + 1) * SizeX - ItemTextureOffset, ItemTextureOffset);
            }

            int[] elements = new int[(vertices.GetLength(0) / 4) * 6];
            for (int i = 0; i < (vertices.GetLength(0) / 4); i++) {
                elements[i * 6] = (i * 4);
                elements[i * 6 + 1] = (i * 4 + 1);
                elements[i * 6 + 2] = (i * 4 + 2);
                elements[i * 6 + 3] = (i * 4 + 2);
                elements[i * 6 + 4] = (i * 4 + 1);
                elements[i * 6 + 5] = (i * 4 + 3);
            }

            Vector2[] uvs = CalcTexturedItemsUV();

            return new GuiVAO(vertices, elements, uvs);
        }

        private static Vector2[] CalcTexturedItemsUV() {
            Vector2[] uvs = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                ItemId t = Inventory.Items[0, i].Item1;
                float x = ((float)((int)t % ItemTextureSize)) / ItemTextureSize;
                float y = ((float)((int)t / ItemTextureSize)) / ItemTextureSize;
                float s = 1f / ItemTextureSize;
                float h = 1f / (ItemTextureSize * ItemTextureSize * 2);
                uvs[i * 4] = new Vector2(x + h, y + s - h);
                uvs[i * 4 + 1] = new Vector2(x + h, y + h);
                uvs[i * 4 + 2] = new Vector2(x + s - h, y + s - h);
                uvs[i * 4 + 3] = new Vector2(x + s - h, y + h);
            }
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
            Vector2[] uvs = CalcTexturedItemsUV();
            TexturedItems.UpdateUVs(uvs);
        }

        public static ItemId CurrentlySelectedItem() {
            return Inventory.Items[0, CurSelectedSlot].Item1;
        }

        public static void Dispose() {
            if (Frame != null)
                Frame.Dispose();
            if (FrameTexture != null)
                FrameTexture.Dispose();
            Frame = null;
            FrameTexture = null;

            if (TexturedItems != null)
                TexturedItems.Dispose();
            if (TexturedItemsTexture != null)
                TexturedItemsTexture.Dispose();
            TexturedItems = null;
            TexturedItemsTexture = null;

            if (CurSelected != null)
                CurSelected.Dispose();
            if (CurSelectedTexture != null)
                CurSelectedTexture.Dispose();
            CurSelected = null;
            CurSelectedTexture = null;
        }
    }
    static class Inventory {

        public static GuiVAO Frame;
        public static Texture FrameTexture;

        public static GuiVAO TexturedItems;
        public static Texture TexturedItemsTexture;

        public static GuiVAO CurSelected;
        public static Texture CurSelectedTexture;

        internal const int InvColumns = 9, InvRows = 6;
        public static Tuple<ItemId, uint>[,] Items = new Tuple<ItemId, uint>[InvRows, InvColumns];

        public static void Init() {
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<ItemId, uint>(ItemId.None, 0);
                }
            }


            Items[0, 0] = new Tuple<ItemId, uint>(ItemId.Switch, 1);
            Items[0, 1] = new Tuple<ItemId, uint>(ItemId.Wire, 1);
            Items[0, 2] = new Tuple<ItemId, uint>(ItemId.LogicLamp, 1);
            Items[0, 3] = new Tuple<ItemId, uint>(ItemId.GateAnd, 1);
            Items[0, 4] = new Tuple<ItemId, uint>(ItemId.GateOr, 1);
            Items[0, 5] = new Tuple<ItemId, uint>(ItemId.GateNot, 1);
            Items[0, 6] = new Tuple<ItemId, uint>(ItemId.LogicBridge, 1);
            Items[0, 7] = new Tuple<ItemId, uint>(ItemId.StickyTilePusher, 1);
            Items[0, 8] = new Tuple<ItemId, uint>(ItemId.StickyTilePuller, 1);

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

        public static void Dispose() {
            //Frame.Dispose();
            //FrameTexture.Dispose();

            //TexturedItems.Dispose();
            //TexturedItemsTexture.Dispose();

            //CurSelected.Dispose();
            //CurSelectedTexture.Dispose();
        }

    }
}
