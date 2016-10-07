using Game.Assets;
using Game.Core;
using Game.Entities;
using Game.Fonts;
using Game.TitleScreen;
using Game.Util;
using OpenGL;
using System;
using System.Drawing;

namespace Game.Interaction {


    static class Inventory {

        public static readonly Vector2 Pos = new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -0.5);

        public static GuiModel Frame;
        public static GuiModel ItemDisplay;
        public static GuiModel SelectedDisplay;
        public static GuiModel Background;

        public static Text text;
        public static GuiModel textbg;

        public static Vector2i? Selected;

        public static BoolSwitch toggle = new BoolSwitch(false, 20);

        internal const int InvColumns = 9, InvRows = 6;
        public static Tuple<ItemId, uint>[,] Items = new Tuple<ItemId, uint>[InvRows, InvColumns];

        public static void Init() {

            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<ItemId, uint>(ItemId.None, 0);
                }
            }

            int ptr = 1;

            for (int i = 1; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<ItemId, uint>((ItemId)ptr, 0);
                    ptr++;
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


            Frame = new GuiModel(FrameVao(), TextureUtil.CreateTexture(new Vector3(0, 0, 0.1)), BeginMode.Lines);
            ItemDisplay = new GuiModel(ItemDisplayVao(), new Texture(Asset.ItemTexture), BeginMode.Triangles);
            SelectedDisplay = GuiModel.CreateWireRectangle(new Vector2(Hotbar.SizeX, Hotbar.SizeY), Color.Blue);
            Background = GuiModel.CreateRectangle(new Vector2(InvColumns * Hotbar.SizeX, (InvRows-1) * Hotbar.SizeY), Color.DimGray);

            text = new Text("Inventory", TextFont.Chiller, 0.5f, Pos, 1);
        }

        private static GuiVAO FrameVao() {
            Vector2[] vertices = new Vector2[(InvColumns + 1) * InvRows];
            int ptr = 0;
            for (int i = 0; i < InvColumns + 1; i++) {
                for (int j = 0; j < InvRows; j++) {
                    vertices[ptr] = new Vector2(i * Hotbar.SizeX, j * Hotbar.SizeY);
                    ptr++;
                }
            }

            int[] elements = new int[2 * (InvColumns + 1) + 2 * InvRows];
            ptr = 0;
            for (int i = 0; i < InvColumns + 1; i++) {
                elements[ptr] = i * (InvRows);
                elements[ptr + 1] = (i + 1) * (InvRows) - 1;
                ptr += 2;
            }
            for (int i = 0; i < InvRows; i++) {
                elements[ptr] = i;
                elements[ptr + 1] = InvColumns * (InvRows) + i;
                ptr += 2;
            }

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new Vector2(0, 0);
            }

            return new GuiVAO(vertices, elements, uvs);

        }

        private static GuiVAO ItemDisplayVao() {
            float SizeX = Hotbar.SizeX, SizeY = Hotbar.SizeY, ItemTextureOffset = Hotbar.ItemTextureOffset;
            Vector2[] vertices = new Vector2[4 * (InvColumns + 1) * (InvRows + 1)];
            int ptr = 0;
            for (int i = 0; i < InvRows - 1; i++) {
                for (int j = 0; j < InvColumns; j++) {
                    //topleft, bottomleft, topright, bottomright
                    vertices[ptr] = new Vector2(j * SizeX + ItemTextureOffset, (i + 1) * SizeY - ItemTextureOffset);
                    vertices[ptr + 1] = new Vector2(j * SizeX + ItemTextureOffset, i * SizeY + ItemTextureOffset);
                    vertices[ptr + 2] = new Vector2((j + 1) * SizeX - ItemTextureOffset, (i + 1) * SizeY - ItemTextureOffset);
                    vertices[ptr + 3] = new Vector2((j + 1) * SizeX - ItemTextureOffset, i * SizeY + ItemTextureOffset);
                    ptr += 4;
                }
            }

            int[] elements = new int[(vertices.Length / 4) * 6];
            for (int i = 0; i < (vertices.Length / 4); i++) {
                elements[i * 6] = (i * 4);
                elements[i * 6 + 1] = (i * 4 + 1);
                elements[i * 6 + 2] = (i * 4 + 2);
                elements[i * 6 + 3] = (i * 4 + 2);
                elements[i * 6 + 4] = (i * 4 + 1);
                elements[i * 6 + 5] = (i * 4 + 3);
            }

            Vector2[] uvs = CalcTextureUVs();

            return new GuiVAO(vertices, elements, uvs);
        }

        private static Vector2[] CalcTextureUVs() {
            float size = Hotbar.ItemTextureSize;
            int ptr = 0;
            Vector2[] uvs = new Vector2[4 * InvColumns * InvRows];
            for (int i = 1; i < InvRows; i++) {
                for (int j = 0; j < InvColumns; j++) {
                    ItemId t = Items[i, j].Item1;
                    float x = ((int)t % size) / size;
                    float y = ((int)((float)t / size)) / size;
                    float s = 1f / size;
                    float h = 1f / (size * size * 2);
                    uvs[ptr] = new Vector2(x + h, y + s - h);
                    uvs[ptr + 1] = new Vector2(x + h, y + h);
                    uvs[ptr + 2] = new Vector2(x + s - h, y + s - h);
                    uvs[ptr + 3] = new Vector2(x + s - h, y + h);
                    ptr += 4;
                }
            }
            return uvs;
        }

        public static void Update() {

            if (Input.Keys[27]) toggle.Toggle();
            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            y -= 24f / Program.Height;
            if (x <= Pos.x || x >= Pos.x + InvColumns * Hotbar.SizeX || y <= Pos.y || y >= Pos.y + (InvRows - 1) * Hotbar.SizeY * Program.AspectRatio || !toggle) {
                Selected = null;
                return;
            }
            int cx = (int)(10 * (x - Pos.x));
            int cy = (int)(10 / Program.AspectRatio * (y - Pos.y));
            Selected = new Vector2i(cx, cy);
            ItemId item = Items[cy + 1, cx].Item1;
            if (Input.Mouse[Input.MouseLeft]) {
                Items[0, Hotbar.CurSelectedSlot] = new Tuple<ItemId, uint>(item, 1);
            }

        }

        public static void Dispose() {
            Frame?.Dispose();
            ItemDisplay?.Dispose();
            SelectedDisplay?.Dispose();
        }

    }
}
