using System;
using System.Collections.Generic;
using OpenGL;
using Game.TitleScreen;
using Game.Core;
using Game.Fonts;
using Game.Guis;
using System.Drawing;
using Game.Assets;
using Game.Util;

namespace Game.Interaction {
    static class GameGuiRenderer {

        public static BoolSwitch RenderDebugText { get; private set; }
        public static BoolSwitch Paused { get; private set; }

        private static GuiModel Background;
        private static Text DebugText;
        private static GuiModel Healthbar;
        private static GuiModel HealthbarTexture;
        private static Button btn_BackToTitle;

        private static HashSet<Button> Buttons;

        public static void Init() {
            TextFont.Init();

            Paused = new BoolSwitch(false, 30);
            RenderDebugText = new BoolSwitch(false, 30);


            btn_BackToTitle = new Button(new Vector2(0, -0.5), new Vector2(0.25, 0.04), "Save and Quit", TextStyle.Chiller_SingleLine_Large, delegate () { Program.SwitchToTitleScreen(); });
            TextStyle style = new TextStyle(TextAlignment.TopLeft, TextFont.LucidaConsole, 0.5f, 2f, Int32.MaxValue, 1.5f, new Vector3(0.5, 0f, 1f));
            DebugText = new Text("", style, new Vector2(-0.99, 0.97));
            Healthbar = GuiModel.CreateRectangle(new Vector2(0.4, 0.04), Color.DarkRed);
            HealthbarTexture = GuiModel.CreateRectangle(new Vector2(0.55, 0.065), Textures.HealthbarTexture);
            Background = GuiModel.CreateRectangle(new Vector2(1, 1), Textures.GameBackgroundTex);


            Buttons = new HashSet<Button>();
            // Buttons.Add(btn_BackToTitle);
        }

        public static void Update() {
            PlayerInventory.Instance.Update();
            if (Input.Keys[27]) {
                Paused.Toggle();
            }
            foreach (Button b in Buttons) {
                //    b.Update();
            }
        }

        private static float CalcBackgroundColour(float x) {
            x += 0.2f;
            x *= 2;
            float a = 2 * x - 1;
            float b = (float)Math.Sqrt((2 * x - 1) * (2 * x - 1) + 1);
            return a / b;
        }

        public static void SetDebugText(string str) {
            DebugText.SetText(str);
        }

        public static void RenderBackground() {
            Gl.UseProgram(Gui.shader.ProgramID);
            float ratio = Player.Instance.data.pos.y / Terrains.Terrain.Tiles.GetLength(1);
            ratio = CalcBackgroundColour(ratio);
            RenderInstance(Background, new Vector2(0, 0), new Vector3(0.5, 0, ratio));
            Gl.UseProgram(0);
        }

        public static void Render() {
            Gl.LineWidth(7);

            Gl.UseProgram(Gui.shader.ProgramID);

            //healthbar
            RenderInstance(Healthbar, new Vector2(0, -0.7));
            RenderInstance(HealthbarTexture, new Vector2(0, -0.7));
            Healthbar.size.x = Player.Instance.data.life.val / Player.Instance.data.life.max * 0.5f;

            //inventory
            if (PlayerInventory.Instance.InventoryOpen) {
                RenderInstance(PlayerInventory.Instance.Background, PlayerInventory.Instance.Pos);
                RenderInstance(PlayerInventory.Instance.Frame, PlayerInventory.Instance.Pos);
                RenderInstance(PlayerInventory.Instance.ItemDisplay, PlayerInventory.Instance.Pos);
                for (int i = 0; i < PlayerInventory.Instance.ItemCountText.GetLength(0); i++) {
                    for (int j = 0; j < PlayerInventory.Instance.ItemCountText.GetLength(1); j++) {
                        Text t = PlayerInventory.Instance.ItemCountText[i, j];
                        RenderInstance(t.model, PlayerInventory.Instance.Pos + PlayerInventory.Instance.TextPosOffset + t.pos);
                    }
                }


                if (PlayerInventory.Instance.Selected.HasValue) {
                    int cx = PlayerInventory.Instance.Selected.Value.x, cy = PlayerInventory.Instance.Selected.Value.y;
                    RenderInstance(PlayerInventory.Instance.SelectedDisplay, PlayerInventory.Instance.Pos + new Vector2(cx * PlayerInventory.SizeX, cy * PlayerInventory.SizeY * Program.AspectRatio));
                }

                // RenderInstance(Inventory.textbg, Vector2.Zero);
                //RenderText(Inventory.text);
            }

            if (Paused) {
                //  RenderInstance(btn_BackToTitle.model, btn_BackToTitle.pos);
                //    RenderInstance(btn_BackToTitle.text.model, btn_BackToTitle.text.pos);
            }

            //hotbar
            RenderInstance(PlayerInventory.Instance.HotbarFrame, PlayerInventory.Instance.HotbarPos);
            RenderInstance(PlayerInventory.Instance.HotbarItemDisplay, PlayerInventory.Instance.HotbarPos);
            RenderInstance(PlayerInventory.Instance.HotbarSelectedDisplay, new Vector2(((2 - PlayerInventory.Instance.Items.GetLength(0) * PlayerInventory.SizeX) / 2 + PlayerInventory.Instance.CurSelectedSlot * PlayerInventory.SizeX) - 1, -1));

            for (int i = 0; i < PlayerInventory.Instance.HotbarItemCountText.GetLength(0); i++) {
                Text t = PlayerInventory.Instance.HotbarItemCountText[i];
                Vector2 pos = PlayerInventory.Instance.HotbarPos + PlayerInventory.Instance.TextPosOffset + t.pos;
                RenderInstance(t.model, pos);
            }

            if (RenderDebugText)
                RenderText(DebugText);


            Gl.UseProgram(0);

            Gl.LineWidth(1);
        }

        private static void RenderInstance(GuiModel model, Vector2 position) {
            RenderInstance(model, position, new Vector3(0.5f, 0f, 1f));
        }

        private static void RenderText(Text t) {
            ShaderProgram shader = Gui.shader;
            shader["size"].SetValue(t.model.size);
            shader["position"].SetValue(t.pos);
            shader["colour"].SetValue(t.style.colour);
            Gl.BindVertexArray(t.model.vao.ID);
            Gl.BindTexture(t.style.font.fontTexture.TextureTarget, t.style.font.fontTexture.TextureID);
            Gl.DrawElements(BeginMode.Triangles, t.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(t.style.font.fontTexture.TextureTarget, 0);
            Gl.BindVertexArray(0);
        }

        private static void RenderInstance(GuiModel model, Vector2 position, Vector3 colour) {
            ShaderProgram shader = Gui.shader;
            shader["position"].SetValue(position);
            shader["size"].SetValue(model.size);
            shader["colour"].SetValue(colour);
            Gl.BindVertexArray(model.vao.ID);
            Gl.BindTexture(model.texture.TextureTarget, model.texture.TextureID);
            Gl.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(model.texture.TextureTarget, 0);
            Gl.BindVertexArray(0);
        }

        public static void Dispose() {
            if (PlayerInventory.Instance != null)
                PlayerInventory.Instance.Dispose();
        }
    }
}
