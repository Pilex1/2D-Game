using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Guis;
using Game.Util;
using OpenGL;
using System.Drawing;

namespace Game.Items {
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
        public Text InvText;
        public GuiModel InvTextBackground;
        public GuiModel InvTextLine;
        public Vector2i? Selected;
        public BoolSwitch InventoryOpen = new BoolSwitch(false, 20);

        public Text ItemNameText;

        public Text[] HotbarItemCountText;
        public GuiModel HotbarFrame;
        public GuiModel HotbarItemDisplay;
        public GuiModel HotbarSelectedDisplay;
        public GuiModel HotbarBackground;

        public int CurSelectedSlot;

        private bool MouseFlag;

        private TextStyle textStyle;

        internal float SizeX = 0.1f;
        internal float SizeY = 0.1f;
        internal float ItemTextureOffset = 0.01f;
        internal float ItemTextureSize = 16;

        #endregion

        #region Init

        private PlayerInventory(int x, int y) : base(x, y) {
            //inventory
            Pos = new Vector2(((2 - x * SizeX) / 2) - 1, -0.5);
            HotbarPos = new Vector2(Pos.x, -0.98);
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
            Background = GuiModel.CreateRectangleTopLeft(new Vector2(x * SizeX, (y - 1) * SizeY), TextureUtil.CreateTexture(new Vector4(0.3, 0.3, 0.3, 0.8)));
            HotbarBackground = GuiModel.CreateRectangleTopLeft(new Vector2(x * SizeX, SizeY), TextureUtil.CreateTexture(new Vector4(0.3, 0.3, 0.3, 0.8)));

            TextStyle style = new TextStyle(TextAlignment.TopLeft, TextFont.LucidaConsole, 0.6f, 1f, 1, 1f, new Vector3(1, 1, 1));
            InvText = new Text("Inventory", style, new Vector2(0.015 + Pos.x, Pos.y + 0.1 + 2 * y * (SizeY - 2 * ItemTextureOffset)));
            InvTextBackground = GuiModel.CreateRectangleTopLeft(new Vector2(x * SizeX, SizeY / 2), TextureUtil.CreateTexture(new Vector4(0.3, 0.3, 0.3, 0.7)));
            InvTextLine = GuiModel.CreateLine(new Vector2(x * SizeX, 0), TextureUtil.CreateTexture(new Vector4(0.05, 0.05, 0.1, 0.9)));
            var itemnamestyle = TextStyle.LucidaConsole_SingleLine_Small;
            itemnamestyle.alignment = TextAlignment.Bottom;
            ItemNameText = new Text("", itemnamestyle, HotbarPos + new Vector2(0, SizeY));

            //hotbar
            CurSelectedSlot = 0;

            HotbarFrame = new GuiModel(Generate_HotbarFrameVao(), TextureUtil.CreateTexture(new Vector3(0, 0, 0.1)), BeginMode.Lines, new Vector2(1, 1));
            HotbarItemDisplay = new GuiModel(Generate_HotbarItemDisplayVao(), Textures.ItemTexture, BeginMode.Triangles, new Vector2(1, 1));
            HotbarSelectedDisplay = GuiModel.CreateWireRectangleTopLeft(new Vector2(SizeX, SizeY), Color.Blue);

        }

        public static void Init() {
            Instance = new PlayerInventory(9, 7);
        }

        public void LoadDefaultItems() {
            int row = 1;

            Items[0, row] = new Item(RawItem.Grass, RawItem.Grass.attribs.stackSize);
            Items[1, row] = new Item(RawItem.Dirt, RawItem.Dirt.attribs.stackSize);
            Items[2, row] = new Item(RawItem.Sand, RawItem.Sand.attribs.stackSize);
            Items[3, row] = new Item(RawItem.Stone, RawItem.Stone.attribs.stackSize);
            Items[4, row] = new Item(RawItem.Wood, RawItem.Wood.attribs.stackSize);
            Items[5, row] = new Item(RawItem.Leaf, RawItem.Leaf.attribs.stackSize);
            Items[6, row] = new Item(RawItem.SnowWood, RawItem.SnowWood.attribs.stackSize);
            Items[7, row] = new Item(RawItem.SnowLeaf, RawItem.SnowLeaf.attribs.stackSize);
            Items[8, row] = new Item(RawItem.Cactus, RawItem.Cactus.attribs.stackSize);

            row++;
            Items[0, row] = new Item(RawItem.Sapling, RawItem.Sapling.attribs.stackSize);
            Items[1, row] = new Item(RawItem.GrassDeco, RawItem.GrassDeco.attribs.stackSize);
            Items[2, row] = new Item(RawItem.Bounce, RawItem.Bounce.attribs.stackSize);
            Items[3, row] = new Item(RawItem.Accelerator, RawItem.Accelerator.attribs.stackSize);
            Items[4, row] = new Item(RawItem.Water, RawItem.Water.attribs.stackSize);
            Items[5, row] = new Item(RawItem.Lava, RawItem.Lava.attribs.stackSize);
            Items[6, row] = new Item(RawItem.BounceFluid, RawItem.BounceFluid.attribs.stackSize);
            Items[7, row] = new Item(RawItem.Light, RawItem.Light.attribs.stackSize);
            //Items[5, row] = new Item(RawItem.WardedTile, RawItem.WardedTile.attribs.stackSize);

            row++;
            Items[0, row] = new Item(RawItem.Brick, RawItem.Brick.attribs.stackSize);
            Items[1, row] = new Item(RawItem.Metal1, RawItem.Metal1.attribs.stackSize);
            Items[2, row] = new Item(RawItem.SmoothSlab, RawItem.SmoothSlab.attribs.stackSize);
            Items[3, row] = new Item(RawItem.WeatheredStone, RawItem.WeatheredStone.attribs.stackSize);
            Items[4, row] = new Item(RawItem.FutureMetal, RawItem.FutureMetal.attribs.stackSize);
            Items[5, row] = new Item(RawItem.Marble, RawItem.Marble.attribs.stackSize);
            Items[6, row] = new Item(RawItem.PlexSpecial, RawItem.PlexSpecial.attribs.stackSize);
            Items[7, row] = new Item(RawItem.Sandstone, RawItem.Sandstone.attribs.stackSize);

            row++;
            Items[0, row] = new Item(RawItem.Tnt, RawItem.Tnt.attribs.stackSize);
            Items[1, row] = new Item(RawItem.Nuke, RawItem.Nuke.attribs.stackSize);
            Items[2, row] = new Item(RawItem.Igniter, RawItem.Igniter.attribs.stackSize);
            Items[3, row] = new Item(RawItem.StaffGreen, RawItem.StaffGreen.attribs.stackSize);
            Items[4, row] = new Item(RawItem.StaffBlue, RawItem.StaffBlue.attribs.stackSize);
            Items[5, row] = new Item(RawItem.StaffRed, RawItem.StaffRed.attribs.stackSize);
            Items[6, row] = new Item(RawItem.StaffPurple, RawItem.StaffPurple.attribs.stackSize);
            Items[7, row] = new Item(RawItem.StaffYellow, RawItem.StaffYellow.attribs.stackSize);
            Items[8, row] = new Item(RawItem.Firework, RawItem.Firework.attribs.stackSize);

            row++;
            Items[0, row] = new Item(RawItem.Switch, RawItem.Switch.attribs.stackSize);
            Items[1, row] = new Item(RawItem.Wire, RawItem.Wire.attribs.stackSize);
            Items[2, row] = new Item(RawItem.WireBridge, RawItem.WireBridge.attribs.stackSize);
            Items[3, row] = new Item(RawItem.GateAnd, RawItem.GateAnd.attribs.stackSize);
            Items[4, row] = new Item(RawItem.GateOr, RawItem.GateOr.attribs.stackSize);
            Items[5, row] = new Item(RawItem.GateNot, RawItem.GateNot.attribs.stackSize);
            Items[6, row] = new Item(RawItem.LogicLamp, RawItem.LogicLamp.attribs.stackSize);
            Items[7, row] = new Item(RawItem.SingleTilePusher, RawItem.SingleTilePusher.attribs.stackSize);
            Items[8, row] = new Item(RawItem.EntitySpawner, RawItem.EntitySpawner.attribs.stackSize);

            row++;
            Items[0, row] = new Item(RawItem.AutoShooter, RawItem.AutoShooter.attribs.stackSize);
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

        private void Update_ItemCountText() {
            GameTime.GuiTimer.Start();
            for (int i = 0; i < ItemCountText.GetLength(0); i++) {
                for (int j = 0; j < ItemCountText.GetLength(1); j++) {
                    var s = Items[i, j + 1].amt.ToString();
                    s = s == "0" ? "" : s;
                    ItemCountText[i, j].SetText(s);
                }
            }
            for (int i = 0; i < ItemCountText.GetLength(0); i++) {
                var s = Items[i, 0].amt.ToString();
                s = s == "0" ? "" : s;
                HotbarItemCountText[i].SetText(s);
            }
            GameTime.GuiTimer.Pause();
        }

        private void Update_HotbarItemNameText() {
            var name = CurrentlySelectedItem().rawitem.attribs.name;
            name = name == "None" ? "" : name;
            ItemNameText.SetText(name);
            ItemNameText.SetPos(HotbarPos + new Vector2((CurSelectedSlot + 0.5) * SizeX + ItemTextureOffset, 2 * SizeY));
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

        private void HandleKeys() {
            if (Input.Keys['1']) Instance.CurSelectedSlot = 0;
            if (Input.Keys['2']) Instance.CurSelectedSlot = 1;
            if (Input.Keys['3']) Instance.CurSelectedSlot = 2;
            if (Input.Keys['4']) Instance.CurSelectedSlot = 3;
            if (Input.Keys['5']) Instance.CurSelectedSlot = 4;
            if (Input.Keys['6']) Instance.CurSelectedSlot = 5;
            if (Input.Keys['7']) Instance.CurSelectedSlot = 6;
            if (Input.Keys['8']) Instance.CurSelectedSlot = 7;
            if (Input.Keys['9']) Instance.CurSelectedSlot = 8;

            if (Input.MouseScroll < 0)
                Instance.IncreaseHotbarSelection();
            if (Input.MouseScroll > 0)
                Instance.DecreaseHotbarSelection();
        }

        public void UpdateHotbar() {
            //   GameTime.GuiTimer.Start();
            HandleKeys();
            HotbarItemDisplay.vao.UpdateUVs(Generate_HotbarTexturedItemsUV());

            Update_HotbarItemNameText();

            Update_ItemCountText();
            //  GameTime.GuiTimer.Pause();
        }

        public void UpdateInventory() {

            Update_ItemDisplayData();

            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            y -= 24f / Program.Height;

            int cx = (int)(10 * (x - Pos.x));
            int cy = (int)(10 / Program.AspectRatio * (y - Pos.y));
            if (cx < 0 || cx >= Items.GetLength(0) || cy < 0 || cy >= Items.GetLength(1) - 1 || !InventoryOpen) {
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
