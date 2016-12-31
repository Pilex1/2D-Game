using System;
using System.Collections.Generic;
using OpenGL;
using Game.TitleScreen;
using Game.Core;
using Game.Fonts;
using Game.Guis;
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
        internal static InGameTextbox TxtInput;

        private static HashSet<Button> Buttons;

        public static void Init() {
            TextFont.Init();

            TextStyle style = new TextStyle(TextAlignment.TopLeft, TextFont.LucidaConsole, 0.45f, 2f, int.MaxValue, 1f, new Vector3(1, 1, 1));
            DebugText = new Text("", style, new Vector2(-0.99, 0.97));
            Healthbar = GuiModel.CreateRectangle(new Vector2(0.52, 0.03), TextureUtil.ColourFromVec4(new Vector4(0.88, 0.3, 0.1, 0.8)));
            HealthbarTexture = GuiModel.CreateRectangle(new Vector2(0.55, 0.04875), Textures.HealthbarTexture);
            Background = GuiModel.CreateRectangle(new Vector2(1, 1), Textures.GameBackgroundTex);
            PausedOverlay = GuiModel.CreateRectangle(new Vector2(1, 1), TextureUtil.ColourFromVec4(new Vector4(0.2, 0.2, 0.2, 0.9)));
            PausedText = new Text("Paused", new TextStyle(TextAlignment.Top, TextFont.LucidaConsole, 1.3f, 2f, 1, 1f, new Vector3(1, 1, 1)), new Vector2(0, 1));
            TxtInput = new InGameTextbox(new Vector2(-0.3, -0.5), new Vector2(0.7f, 0.03), TextFont.LucidaConsole, 0.5f);

            Buttons = new HashSet<Button>();
            btn_BackToTitle = new Button(new Vector2(0, -0.2), new Vector2(0.4, 0.04), "Save and Quit", TextStyle.LucidaConsole_SingleLine_Small, () => {
                GameLogic.SaveWorld();
                Program.SwitchToTitleScreen();
            });
            Buttons.Add(btn_BackToTitle);
        }

        public static void Update() {
            if (GameLogic.State == GameLogic.GameState.Paused) {
                foreach (var b in Buttons) {
                    b.Update();
                }
            } else {
                TxtInput.Update();

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
            var inv = PlayerInventory.Instance;
            if (inv.InventoryOpen) {
                RenderInstance(inv.Background, inv.Pos);
                RenderInstance(inv.Frame, inv.Pos);
                RenderInstance(inv.ItemDisplay, inv.Pos);
                for (int i = 0; i < inv.ItemCountText.GetLength(0); i++) {
                    for (int j = 0; j < inv.ItemCountText.GetLength(1); j++) {
                        Text t = inv.ItemCountText[i, j];
                        RenderInstance(t.model, inv.Pos + inv.TextPosOffset + t.pos);
                    }
                }


                if (inv.Selected.HasValue) {
                    int cx = inv.Selected.Value.x, cy = inv.Selected.Value.y;
                    RenderInstance(inv.SelectedDisplay, inv.Pos + new Vector2(cx * inv.SizeX, cy * inv.SizeY * Program.AspectRatio));
                }

                var textpos = new Vector2(inv.Pos.x, inv.Pos.y + 2 * (inv.Items.GetLength(1) - 0.5) * inv.SizeY) - new Vector2(0, 2 * inv.SizeY - 0.01);
                RenderInstance(inv.InvTextBackground, textpos);
                RenderInstance(inv.InvTextLine, textpos);
                RenderText(inv.InvText);
            }

            //hotbar
            RenderInstance(inv.HotbarBackground, inv.HotbarPos);
            RenderInstance(inv.HotbarFrame, inv.HotbarPos);
            RenderInstance(inv.HotbarItemDisplay, inv.HotbarPos);
            RenderInstance(inv.HotbarSelectedDisplay, new Vector2(((2 - inv.Items.GetLength(0) * inv.SizeX) / 2 + inv.CurSelectedSlot * inv.SizeX) - 1, inv.HotbarPos.y));
            RenderText(inv.ItemNameText);

            for (int i = 0; i < inv.HotbarItemCountText.GetLength(0); i++) {
                Text t = inv.HotbarItemCountText[i];
                Vector2 pos = inv.HotbarPos + inv.TextPosOffset + t.pos;
                RenderInstance(t.model, pos);
            }

            if (GameLogic.RenderDebugText)
                RenderText(DebugText);

            RenderInstance(TxtInput.model, TxtInput.pos, TxtInput.colour);
            RenderText(TxtInput.text);

            if (GameLogic.State == GameLogic.GameState.Paused) {
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
