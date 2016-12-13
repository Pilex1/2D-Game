using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Guis;
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

        internal const int InvColumns = 9, InvRows = 7;
        public static Tuple<Item, uint>[,] Items;

        public static void Init() {

            Items = new Tuple<Item, uint>[InvRows, InvColumns];
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<Item, uint>(Item.None, 0);
                }
            }

            Frame = new GuiModel(FrameVao(), TextureUtil.CreateTexture(new Vector3(0, 0, 0.1)), BeginMode.Lines, new Vector2(1, 1));
            {
                Vector2[] vertices;
                int[] elements;
                Vector2[] uvs;
                ItemDisplayData(out vertices, out elements, out uvs);
                GuiVAO vao = new GuiVAO(vertices, elements, uvs);
                ItemDisplay = new GuiModel(vao, Textures.ItemTexture, BeginMode.Triangles, new Vector2(1, 1));

            }
            SelectedDisplay = GuiModel.CreateWireRectangleTopLeft(new Vector2(Hotbar.SizeX, Hotbar.SizeY), Color.Blue);
            Background = GuiModel.CreateRectangleTopLeft(new Vector2(InvColumns * Hotbar.SizeX, (InvRows - 1) * Hotbar.SizeY), Color.DimGray);

            TextStyle style = new TextStyle(TextAlignment.CenterCenter, TextFont.Chiller, 0.5f, 1f, 1, 1f, new Vector3(1, 1, 1));
            text = new Text("Inventory", style, new Vector2(0, 0));
            textbg = GuiModel.CreateRectangle(new Vector2(0.5, 0.05), Color.DimGray);
        }

        private static void UpdateItemDisplayData() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            ItemDisplayData(out vertices, out elements, out uvs);
            ItemDisplay.vao.UpdateAll(vertices, elements, uvs);
        }

        public static void LoadItems(Tuple<Item, uint>[,] items) {
            if (items.GetLength(0) > InvRows || items.GetLength(1) > InvColumns) {
                throw new ArgumentException("Invalid inventory dimensions");
            }
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = i >= items.GetLength(0) || j >= items.GetLength(1) ? new Tuple<Item, uint>(Item.None, 0) : items[i, j];
                }
            }

            UpdateItemDisplayData();
        }

        public static void LoadDefaultItems() {
            uint amt = 1;

            int row = 1;

            Items[row, 0] = new Tuple<Item, uint>(Item.Grass, amt);
            Items[row, 1] = new Tuple<Item, uint>(Item.Dirt, amt);
            Items[row, 2] = new Tuple<Item, uint>(Item.Sand, amt);
            Items[row, 3] = new Tuple<Item, uint>(Item.Stone, amt);
            Items[row, 4] = new Tuple<Item, uint>(Item.Wood, amt);
            Items[row, 5] = new Tuple<Item, uint>(Item.Leaf, amt);
            Items[row, 6] = new Tuple<Item, uint>(Item.SnowWood, amt);
            Items[row, 7] = new Tuple<Item, uint>(Item.SnowLeaf, amt);
            Items[row, 8] = new Tuple<Item, uint>(Item.Cactus, amt);

            row++;
            Items[row, 0] = new Tuple<Item, uint>(Item.Sapling, amt);
            Items[row, 1] = new Tuple<Item, uint>(Item.GrassDeco, amt);

            row++;
            Items[row, 0] = new Tuple<Item, uint>(Item.Brick, amt);
            Items[row, 1] = new Tuple<Item, uint>(Item.Metal1, amt);
            Items[row, 2] = new Tuple<Item, uint>(Item.SmoothSlab, amt);
            Items[row, 3] = new Tuple<Item, uint>(Item.WeatheredStone, amt);
            Items[row, 4] = new Tuple<Item, uint>(Item.FutureMetal, amt);
            Items[row, 5] = new Tuple<Item, uint>(Item.Marble, amt);
            Items[row, 6] = new Tuple<Item, uint>(Item.PlexSpecial, amt);

            row++;
            Items[row, 0] = new Tuple<Item, uint>(Item.Bounce, amt);
            Items[row, 1] = new Tuple<Item, uint>(Item.Accelerator, amt);
            Items[row, 2] = new Tuple<Item, uint>(Item.Water, amt);
            Items[row, 3] = new Tuple<Item, uint>(Item.Lava, amt);

            row++;
            Items[row, 0] = new Tuple<Item, uint>(Item.Tnt, amt);
            Items[row, 1] = new Tuple<Item, uint>(Item.Nuke, amt);
            Items[row, 2] = new Tuple<Item, uint>(Item.Igniter, amt);
            Items[row, 3] = new Tuple<Item, uint>(Item.StaffGreen, amt);
            Items[row, 4] = new Tuple<Item, uint>(Item.StaffBlue, amt);
            Items[row, 5] = new Tuple<Item, uint>(Item.StaffRed, amt);
            Items[row, 6] = new Tuple<Item, uint>(Item.StaffPurple, amt);

            row++;
            Items[row, 0] = new Tuple<Item, uint>(Item.Switch, amt);
            Items[row, 1] = new Tuple<Item, uint>(Item.Wire, amt);
            Items[row, 2] = new Tuple<Item, uint>(Item.WireBridge, amt);
            Items[row, 3] = new Tuple<Item, uint>(Item.GateAnd, amt);
            Items[row, 4] = new Tuple<Item, uint>(Item.GateOr, amt);
            Items[row, 5] = new Tuple<Item, uint>(Item.GateNot, amt);
            Items[row, 6] = new Tuple<Item, uint>(Item.LogicLamp, amt);
            Items[row, 7] = new Tuple<Item, uint>(Item.SingleTilePusher, amt);

            UpdateItemDisplayData();
        }

        public static void ClearItems() {
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Tuple<Item, uint>(Item.None, 0);
                }
            }

            UpdateItemDisplayData();
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

        private static void ItemDisplayData(out Vector2[] vertices, out int[] elements, out Vector2[] uvs) {
            float SizeX = Hotbar.SizeX, SizeY = Hotbar.SizeY, ItemTextureOffset = Hotbar.ItemTextureOffset;
            vertices = new Vector2[4 * (InvColumns + 1) * (InvRows + 1)];
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

            elements = new int[(vertices.Length / 4) * 6];
            for (int i = 0; i < (vertices.Length / 4); i++) {
                elements[i * 6] = (i * 4);
                elements[i * 6 + 1] = (i * 4 + 1);
                elements[i * 6 + 2] = (i * 4 + 2);
                elements[i * 6 + 3] = (i * 4 + 2);
                elements[i * 6 + 4] = (i * 4 + 1);
                elements[i * 6 + 5] = (i * 4 + 3);
            }

            uvs = CalcTextureUVs();
        }

        private static Vector2[] CalcTextureUVs() {
            float size = Hotbar.ItemTextureSize;
            int ptr = 0;
            Vector2[] uvs = new Vector2[4 * InvColumns * InvRows];
            for (int i = 1; i < InvRows; i++) {
                for (int j = 0; j < InvColumns; j++) {
                    Item t = Items[i, j].Item1;
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

            if (Input.Keys['e']) toggle.Toggle();
            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            y -= 24f / Program.Height;
            if (x <= Pos.x || x >= Pos.x + InvColumns * Hotbar.SizeX || y <= Pos.y || y >= Pos.y + (InvRows - 1) * Hotbar.SizeY * Program.AspectRatio || !toggle) {
                Selected = null;
                return;
            }
            int cx = (int)(10 * (x - Pos.x));
            int cy = (int)(10 / Program.AspectRatio * (y - Pos.y));
            Selected = new Vector2i(cx, cy);
            Item item = Items[cy + 1, cx].Item1;
            if (Input.Mouse[Input.MouseLeft]) {
                Items[0, Hotbar.CurSelectedSlot] = new Tuple<Item, uint>(item, 1);
            }

        }

        public static void Dispose() {
            //Frame?.Dispose();
            //ItemDisplay?.Dispose();
            //SelectedDisplay?.Dispose();
            if (Frame != null)
                Frame.DisposeAll();
            if (ItemDisplay != null)
                ItemDisplay.DisposeVao();
            if (SelectedDisplay != null)
                SelectedDisplay.DisposeAll();
        }

    }
}
