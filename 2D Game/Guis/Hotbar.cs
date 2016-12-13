using Game.Assets;
using Game.Core;
using Game.Util;
using OpenGL;

namespace Game.Interaction {
    static class Hotbar {
        public static GuiModel Frame;
        public static GuiModel ItemDisplay;
        public static GuiModel SelectedDisplay;

        public static int CurSelectedSlot { get; set; }

        internal const float SizeX = 0.1f;
        internal const float SizeY = 0.1f;
        internal const float ItemTextureOffset = 0.01f;

        internal const int ItemTextureSize = 16;

        internal static void Init() {
            CurSelectedSlot = 0;

            Frame = new GuiModel(FrameVao(), TextureUtil.CreateTexture(new Vector3(0, 0, 0.1)), BeginMode.Lines, new Vector2(1,1));
            ItemDisplay = new GuiModel(ItemDisplayVao(), Textures.ItemTexture, BeginMode.Triangles, new Vector2(1,1));
            SelectedDisplay = GuiModel.CreateWireRectangleTopLeft(new Vector2(SizeX, SizeY), ColourUtil.ColourFromVec3(new Vector3(0, 0, 1)));
        }

        private static GuiVAO FrameVao() {
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

        private static GuiVAO ItemDisplayVao() {
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

            return new GuiVAO(vertices, elements, uvs,verticeshint:BufferUsageHint.DynamicDraw,uvhint:BufferUsageHint.DynamicDraw);
        }

        private static Vector2[] CalcTexturedItemsUV() {
            Vector2[] uvs = new Vector2[4 * Inventory.InvColumns];
            for (int i = 0; i < Inventory.InvColumns; i++) {
                Item t = Inventory.Items[0, i].Item1;
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
            ItemDisplay.vao.UpdateUVs(uvs);
        }

        public static Item CurrentlySelectedItem() {
            return Inventory.Items[0, CurSelectedSlot].Item1;
        }

        public static void Dispose() {
        //    Frame?.Dispose();
        //    ItemDisplay?.Dispose();
        //    SelectedDisplay?.Dispose();
            if (Frame != null)
                Frame.DisposeAll();
            if (ItemDisplay != null)
                ItemDisplay.DisposeAll();
            if (SelectedDisplay != null)
                SelectedDisplay.DisposeAll();
        }
    }
}
