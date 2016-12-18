using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Interaction;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Guis {
    class PlayerInventory : Inventory {

        #region Fields

        public static PlayerInventory Instance;



        public Vector2 Pos;
        public Vector2 HotbarPos;
        public Vector2 TextPosOffset;

        public GuiModel Frame;
        public GuiModel ItemDisplay;
        public GuiModel SelectedDisplay;
        public GuiModel Background;

        public Text[,] ItemCountText;
        public Text text;
        public GuiModel textbg;
        public Vector2i? Selected;
        public BoolSwitch InventoryOpen = new BoolSwitch(false, 20);


        public Text[] HotbarItemCountText;
        public GuiModel HotbarFrame;
        public GuiModel HotbarItemDisplay;
        public GuiModel HotbarSelectedDisplay;

        public int CurSelectedSlot;

        private bool MouseFlag;

        private TextStyle textStyle;

        internal const float SizeX = 0.1f;
        internal const float SizeY = 0.1f;
        internal const float ItemTextureOffset = 0.01f;
        internal const float ItemTextureSize = 16;

        #endregion

        #region Init

        private PlayerInventory(int x, int y) : base(x, y) {
            //inventory
            Pos = new Vector2(((2 - x * SizeX) / 2) - 1, -0.5);
            HotbarPos = new Vector2(Pos.x, -1);
            TextPosOffset = new Vector2(0.115, 0.03);

            textStyle = TextStyle.LucidaConsole_SingleLine_Small;
            textStyle.alignment = TextAlignment.BottomRight;


            ItemCountText = new Text[x, y - 1];
            for (int i = 0; i < ItemCountText.GetLength(0); i++) {
                for (int j = 0; j < ItemCountText.GetLength(1); j++) {
                    ItemCountText[i, j] = new Text("", textStyle, new Vector2(i * SizeX, 2 * j * (SizeY - ItemTextureOffset)));
                }
            }
            HotbarItemCountText = new Text[x];
            for (int i = 0; i < ItemCountText.GetLength(0); i++) {
                HotbarItemCountText[i] = new Text("", textStyle, new Vector2(i * SizeX, 0));
            }



            Frame = new GuiModel(Generate_FrameVao(), TextureUtil.CreateTexture(new Vector3(0, 0, 0.1)), BeginMode.Lines, new Vector2(1, 1));
            {
                Vector2[] vertices;
                int[] elements;
                Vector2[] uvs;
                Generate_ItemDisplayData(out vertices, out elements, out uvs);
                GuiVAO vao = new GuiVAO(vertices, elements, uvs);
                ItemDisplay = new GuiModel(vao, Textures.ItemTexture, BeginMode.Triangles, new Vector2(1, 1));

            }
            SelectedDisplay = GuiModel.CreateWireRectangleTopLeft(new Vector2(SizeX, SizeY), Color.Blue);
            Background = GuiModel.CreateRectangleTopLeft(new Vector2(x * SizeX, (y - 1) * SizeY), Color.DimGray);

            TextStyle style = new TextStyle(TextAlignment.Center, TextFont.Chiller, 0.5f, 1f, 1, 1f, new Vector3(1, 1, 1));
            text = new Text("Inventory", style, new Vector2(0, 0));
            textbg = GuiModel.CreateRectangle(new Vector2(0.5, 0.05), Color.DimGray);

            //hotbar
            CurSelectedSlot = 0;

            HotbarFrame = new GuiModel(Generate_HotbarFrameVao(), TextureUtil.CreateTexture(new Vector3(0, 0, 0.1)), BeginMode.Lines, new Vector2(1, 1));
            HotbarItemDisplay = new GuiModel(Generate_HotbarItemDisplayVao(), Textures.ItemTexture, BeginMode.Triangles, new Vector2(1, 1));
            HotbarSelectedDisplay = GuiModel.CreateWireRectangleTopLeft(new Vector2(SizeX, SizeY), ColourUtil.ColourFromVec3(new Vector3(0, 0, 1)));

        }

        public static void Init() {
            Instance = new PlayerInventory(9, 7);
        }

        public void LoadDefaultItems() {
            int row = 1;

            Items[0, row] = new Item(RawItem.Grass, MaxItems);
            Items[1, row] = new Item(RawItem.Dirt, MaxItems);
            Items[2, row] = new Item(RawItem.Sand, MaxItems);
            Items[3, row] = new Item(RawItem.Stone, MaxItems);
            Items[4, row] = new Item(RawItem.Wood, MaxItems);
            Items[5, row] = new Item(RawItem.Leaf, MaxItems);
            Items[6, row] = new Item(RawItem.SnowWood, MaxItems);
            Items[7, row] = new Item(RawItem.SnowLeaf, MaxItems);
            Items[8, row] = new Item(RawItem.Cactus, MaxItems);

            row++;
            Items[0, row] = new Item(RawItem.Sapling, MaxItems);
            Items[1, row] = new Item(RawItem.GrassDeco, MaxItems);

            row++;
            Items[0, row] = new Item(RawItem.Brick, MaxItems);
            Items[1, row] = new Item(RawItem.Metal1, MaxItems);
            Items[2, row] = new Item(RawItem.SmoothSlab, MaxItems);
            Items[3, row] = new Item(RawItem.WeatheredStone, MaxItems);
            Items[4, row] = new Item(RawItem.FutureMetal, MaxItems);
            Items[5, row] = new Item(RawItem.Marble, MaxItems);
            Items[6, row] = new Item(RawItem.PlexSpecial, MaxItems);
            Items[7, row] = new Item(RawItem.Sandstone, MaxItems);

            row++;
            Items[0, row] = new Item(RawItem.Bounce, MaxItems);
            Items[1, row] = new Item(RawItem.Accelerator, MaxItems);
            Items[2, row] = new Item(RawItem.Water, MaxItems);

            row++;
            Items[0, row] = new Item(RawItem.Tnt, MaxItems);
            Items[1, row] = new Item(RawItem.Nuke, MaxItems);
            Items[2, row] = new Item(RawItem.Igniter, MaxItems);
            Items[3, row] = new Item(RawItem.StaffGreen, MaxItems);
            Items[4, row] = new Item(RawItem.StaffBlue, MaxItems);
            Items[5, row] = new Item(RawItem.StaffRed, MaxItems);
            Items[6, row] = new Item(RawItem.StaffPurple, MaxItems);
            Items[7, row] = new Item(RawItem.StaffYellow, MaxItems);

            row++;
            Items[0, row] = new Item(RawItem.Switch, MaxItems);
            Items[1, row] = new Item(RawItem.Wire, MaxItems);
            Items[2, row] = new Item(RawItem.WireBridge, MaxItems);
            Items[3, row] = new Item(RawItem.GateAnd, MaxItems);
            Items[4, row] = new Item(RawItem.GateOr, MaxItems);
            Items[5, row] = new Item(RawItem.GateNot, MaxItems);
            Items[6, row] = new Item(RawItem.LogicLamp, MaxItems);
            Items[7, row] = new Item(RawItem.SingleTilePusher, MaxItems);
        }

        #endregion

        #region Model Generation
        private GuiVAO Generate_FrameVao() {
            Vector2[] vertices = new Vector2[(Items.GetLength(0) + 1) * Items.GetLength(1)];
            int ptr = 0;
            for (int i = 0; i < Items.GetLength(0) + 1; i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    vertices[ptr] = new Vector2(i * SizeX, j * SizeY);
                    ptr++;
                }
            }

            int[] elements = new int[2 * (Items.GetLength(0) + 1) + 2 * Items.GetLength(1)];
            ptr = 0;
            for (int i = 0; i < Items.GetLength(0) + 1; i++) {
                elements[ptr] = i * (Items.GetLength(1));
                elements[ptr + 1] = (i + 1) * (Items.GetLength(1)) - 1;
                ptr += 2;
            }
            for (int i = 0; i < Items.GetLength(1); i++) {
                elements[ptr] = i;
                elements[ptr + 1] = Items.GetLength(0) * (Items.GetLength(1)) + i;
                ptr += 2;
            }

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new Vector2(0, 0);
            }

            return new GuiVAO(vertices, elements, uvs);

        }

        private void Generate_ItemDisplayData(out Vector2[] vertices, out int[] elements, out Vector2[] uvs) {
            vertices = new Vector2[4 * (Items.GetLength(0) + 1) * (Items.GetLength(1) + 1)];
            int ptr = 0;
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1) - 1; j++) {
                    //topleft, bottomleft, topright, bottomright
                    vertices[ptr] = new Vector2(i * SizeX + ItemTextureOffset, (j + 1) * SizeY - ItemTextureOffset);
                    vertices[ptr + 1] = new Vector2(i * SizeX + ItemTextureOffset, j * SizeY + ItemTextureOffset);
                    vertices[ptr + 2] = new Vector2((i + 1) * SizeX - ItemTextureOffset, (j + 1) * SizeY - ItemTextureOffset);
                    vertices[ptr + 3] = new Vector2((i + 1) * SizeX - ItemTextureOffset, j * SizeY + ItemTextureOffset);
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

            uvs = Generate_TextureUVs();
        }

        private Vector2[] Generate_TextureUVs() {
            int ptr = 0;
            Vector2[] uvs = new Vector2[4 * Items.GetLength(0) * Items.GetLength(1)];
            //ignore first row (hotbar)
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 1; j < Items.GetLength(1); j++) {
                    ItemID t = Items[i, j].rawitem.id;
                    float x = ((int)t % ItemTextureSize) / ItemTextureSize;
                    float y = ((int)((float)t / ItemTextureSize)) / ItemTextureSize;
                    float s = 1f / ItemTextureSize;
                    float h = 1f / (ItemTextureSize * ItemTextureSize * 2);
                    uvs[ptr] = new Vector2(x + h, y + s - h);
                    uvs[ptr + 1] = new Vector2(x + h, y + h);
                    uvs[ptr + 2] = new Vector2(x + s - h, y + s - h);
                    uvs[ptr + 3] = new Vector2(x + s - h, y + h);
                    ptr += 4;
                }
            }
            return uvs;
        }

        private void Update_ItemText() {
            for (int i = 0; i < ItemCountText.GetLength(0); i++) {
                for (int j = 0; j < ItemCountText.GetLength(1); j++) {
                    var s = Items[i, j + 1].amt.ToString();
                    ItemCountText[i, j].SetText(s == "0" ? "" : s);
                }
            }
            for (int i = 0; i < ItemCountText.GetLength(0); i++) {
                var s = Items[i, 0].amt.ToString();
                HotbarItemCountText[i].SetText(s == "0" ? "" : s);
            }
        }

        private void Update_ItemDisplayData() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            Generate_ItemDisplayData(out vertices, out elements, out uvs);
            ItemDisplay.vao.UpdateAll(vertices, elements, uvs);
        }


        private GuiVAO Generate_HotbarFrameVao() {
            Vector2[] vertices = new Vector2[2 * (Items.GetLength(0) + 1)];
            for (int i = 0; i < Items.GetLength(0) + 1; i++) {
                vertices[i * 2] = new Vector2(i * SizeX, SizeY);
                vertices[i * 2 + 1] = new Vector2(i * SizeX, 0);
            }

            int[] elements = new int[2 * (Items.GetLength(0) + 1) + 4];
            for (int i = 0; i < 2 * (Items.GetLength(0) + 1); i++) {
                elements[i] = i;
            }
            elements[2 * (Items.GetLength(0) + 1)] = 0;
            elements[2 * (Items.GetLength(0) + 1) + 1] = 2 * (Items.GetLength(0) + 1) - 2;
            elements[2 * (Items.GetLength(0) + 1) + 2] = 1;
            elements[2 * (Items.GetLength(0) + 1) + 3] = 2 * (Items.GetLength(0) + 1) - 1;

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new Vector2(0, 0);
            }

            return new GuiVAO(vertices, elements, uvs);
        }

        private GuiVAO Generate_HotbarItemDisplayVao() {
            Vector2[] vertices = new Vector2[4 * Items.GetLength(0)];
            for (int i = 0; i < Items.GetLength(0); i++) {
                //topleft, bottomleft, topright, bottomright
                vertices[i * 4] = new Vector2(i * SizeX + ItemTextureOffset, SizeY - ItemTextureOffset);
                vertices[i * 4 + 1] = new Vector2(i * SizeX + ItemTextureOffset, ItemTextureOffset);
                vertices[i * 4 + 2] = new Vector2((i + 1) * SizeX - ItemTextureOffset, SizeY - ItemTextureOffset);
                vertices[i * 4 + 3] = new Vector2((i + 1) * SizeX - ItemTextureOffset, ItemTextureOffset);
            }

            int[] elements = new int[vertices.GetLength(0) * 6 / 4];
            for (int i = 0; i < (vertices.GetLength(0) / 4); i++) {
                elements[i * 6] = (i * 4);
                elements[i * 6 + 1] = (i * 4 + 1);
                elements[i * 6 + 2] = (i * 4 + 2);
                elements[i * 6 + 3] = (i * 4 + 2);
                elements[i * 6 + 4] = (i * 4 + 1);
                elements[i * 6 + 5] = (i * 4 + 3);
            }

            Vector2[] uvs = Generate_HotbarTexturedItemsUV();

            return new GuiVAO(vertices, elements, uvs, verticeshint: BufferUsageHint.DynamicDraw, uvhint: BufferUsageHint.DynamicDraw);
        }

        private Vector2[] Generate_HotbarTexturedItemsUV() {
            Vector2[] uvs = new Vector2[4 * Items.GetLength(0)];
            for (int i = 0; i < Items.GetLength(0); i++) {
                ItemID t = Items[i, 0].rawitem.id;
                float x = ((int)t % ItemTextureSize) / ItemTextureSize;
                float y = ((int)((float)t / ItemTextureSize)) / ItemTextureSize;
                float s = 1f / ItemTextureSize;
                float h = 1f / (ItemTextureSize * ItemTextureSize * 2);
                uvs[i * 4] = new Vector2(x + h, y + s - h);
                uvs[i * 4 + 1] = new Vector2(x + h, y + h);
                uvs[i * 4 + 2] = new Vector2(x + s - h, y + s - h);
                uvs[i * 4 + 3] = new Vector2(x + s - h, y + h);
            }
            return uvs;
        }

        #endregion



        public void Update() {

            HotbarItemDisplay.vao.UpdateUVs(Generate_HotbarTexturedItemsUV());
            Update_ItemText();
            Update_ItemDisplayData();

            if (Input.Keys['e'])
                InventoryOpen.Toggle();
            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            y -= 24f / Program.Height;

            int cx = (int)(10 * (x - Pos.x));
            int cy = (int)(10 / Program.AspectRatio * (y - Pos.y));
            if (cx < 0 || cx >= Items.GetLength(0) || cy < 0 || cy >= Items.GetLength(1) || !InventoryOpen) {
                Selected = null;
                return;
            }
            Selected = new Vector2i(cx, cy);
            if (Input.Mouse[Input.MouseLeft]) {
                if (!MouseFlag)
                    SwapItems(new Vector2i(cx, cy + 1), new Vector2i(CurSelectedSlot, 0));
                MouseFlag = true;
            } else {
                MouseFlag = false;
            }


        }

        public void Dispose() {
            if (Frame != null)
                Frame.DisposeAll();
            if (ItemDisplay != null)
                ItemDisplay.DisposeVao();
            if (SelectedDisplay != null)
                SelectedDisplay.DisposeAll();

            if (HotbarFrame != null)
                HotbarFrame.DisposeAll();
            if (HotbarItemDisplay != null)
                HotbarItemDisplay.DisposeAll();
            if (HotbarSelectedDisplay != null)
                HotbarSelectedDisplay.DisposeAll();
        }


        public void IncreaseHotbarSelection() {
            CurSelectedSlot++;
            if (CurSelectedSlot >= Items.GetLength(0)) CurSelectedSlot = 0;
        }

        public void DecreaseHotbarSelection() {
            CurSelectedSlot--;
            if (CurSelectedSlot < 0) CurSelectedSlot = Items.GetLength(0) - 1;
        }

        public Item CurrentlySelectedItem() {
            return Items[CurSelectedSlot, 0];
        }
    }
}
