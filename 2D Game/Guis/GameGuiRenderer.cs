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
using Game.Items;

namespace Game.Interaction {
    static class GameGuiRenderer {

        private static GuiModel PausedOverlay;
        private static Text PausedText;
        private static GuiModel Background;
        private static Text DebugText;
        private static GuiModel Healthbar;
        private static GuiModel HealthbarTexture;
        private static Button btn_BackToTitle;

        private static HashSet<Button> Buttons;

        public static void Init() {
            TextFont.Init();

            TextStyle style = new TextStyle(TextAlignment.TopLeft, TextFont.LucidaConsole, 0.5f, 2f, int.MaxValue, 1.5f, new Vector3(1, 1, 1));
            DebugText = new Text("", style, new Vector2(-0.99, 0.97));
            Healthbar = GuiModel.CreateRectangle(new Vector2(0.52, 0.04), ColourUtil.ColourFromVec4(new Vector4(0.88, 0.3, 0.1, 0.8)));
            HealthbarTexture = GuiModel.CreateRectangle(new Vector2(0.55, 0.065), Textures.HealthbarTexture);
            Background = GuiModel.CreateRectangle(new Vector2(1, 1), Textures.GameBackgroundTex);
            PausedOverlay = GuiModel.CreateRectangle(new Vector2(1, 1), ColourUtil.ColourFromVec4(new Vector4(0.2, 0.2, 0.2, 0.9)));
            PausedText = new Text("Paused", new TextStyle(TextAlignment.Top, TextFont.Chiller, 1.3f, 2f, 1, 1f, new Vector3(1, 1, 1)), new Vector2(0, 1));

            Buttons = new HashSet<Button>();
            btn_BackToTitle = new Button(new Vector2(0, -0.2), new Vector2(0.4, 0.04), "Save and Quit", TextStyle.Chiller_SingleLine_Large, delegate () {
                Program.SaveWorld();
                Program.SwitchToTitleScreen();
            });
            Buttons.Add(btn_BackToTitle);
        }

        public static void Update() {
            if (!GameLogic.Paused) return;

            foreach (var b in Buttons) {
                b.Update();
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
            RenderInstance(Background, new Vector2(0, 0), new Vector3(ratio, ratio, ratio));
            Gl.UseProgram(0);
        }

        public static void Render() {
            Gl.LineWidth(7);

            Gl.UseProgram(Gui.shader.ProgramID);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //healthbar
            RenderInstance(Healthbar, new Vector2(0, -0.68));
            RenderInstance(HealthbarTexture, new Vector2(0, -0.68));
            Healthbar.size.x = Player.Instance.data.life.val / Player.Instance.data.life.max * 0.52f;

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

                var textpos = new Vector2(PlayerInventory.Instance.Pos.x, PlayerInventory.Instance.Pos.y + 2 * (PlayerInventory.Instance.Items.GetLength(1) - 0.5) * PlayerInventory.SizeY) - new Vector2(0, 2 * PlayerInventory.SizeY - 0.01);
                RenderInstance(PlayerInventory.Instance.InvTextBackground, textpos);
                RenderText(PlayerInventory.Instance.InvText);
                RenderInstance(PlayerInventory.Instance.InvTextLine, textpos);
            }

            //hotbar
            RenderInstance(PlayerInventory.Instance.HotbarBackground, PlayerInventory.Instance.HotbarPos);
            RenderInstance(PlayerInventory.Instance.HotbarFrame, PlayerInventory.Instance.HotbarPos);
            RenderInstance(PlayerInventory.Instance.HotbarItemDisplay, PlayerInventory.Instance.HotbarPos);
            RenderInstance(PlayerInventory.Instance.HotbarSelectedDisplay, new Vector2(((2 - PlayerInventory.Instance.Items.GetLength(0) * PlayerInventory.SizeX) / 2 + PlayerInventory.Instance.CurSelectedSlot * PlayerInventory.SizeX) - 1, -1));
            RenderText(PlayerInventory.Instance.ItemNameText);

            for (int i = 0; i < PlayerInventory.Instance.HotbarItemCountText.GetLength(0); i++) {
                Text t = PlayerInventory.Instance.HotbarItemCountText[i];
                Vector2 pos = PlayerInventory.Instance.HotbarPos + PlayerInventory.Instance.TextPosOffset + t.pos;
                RenderInstance(t.model, pos);
            }

            if (GameLogic.RenderDebugText)
                RenderText(DebugText);


            if (GameLogic.Paused) {
                RenderInstance(PausedOverlay, new Vector2(0, 0), new Vector4(0.5, 0.5, 0.5, 0.8));

                RenderInstance(PausedText.model, PausedText.pos, PausedText.style.colour);
                RenderInstance(btn_BackToTitle.model, btn_BackToTitle.pos, btn_BackToTitle.colour);
                RenderInstance(btn_BackToTitle.text.model, btn_BackToTitle.text.pos, btn_BackToTitle.text.style.colour);
            }

            Gl.UseProgram(0);

            Gl.LineWidth(1);
            Gl.Disable(EnableCap.Blend);

        }

        private static void RenderInstance(GuiModel model, Vector2 position) {
            RenderInstance(model, position, new Vector3(1, 1, 1));
        }

        private static void RenderText(Text t) {
            RenderInstance(t.model, t.pos, t.style.colour);
        }

        private static void RenderInstance(GuiModel model, Vector2 position, Vector3 colour) {
            RenderInstance(model, position, new Vector4(colour.x, colour.y, colour.z, 1));
        }

        private static void RenderInstance(GuiModel model, Vector2 position, Vector4 colour) {
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
