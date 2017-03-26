using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Items;
using Game.Main.GLConstructs;
using Game.Main.Util;
using Game.Terrains.Terrain_Generation;
using Game.TitleScreen;
using Game.Util;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;

namespace Game.Guis.Renderers {
    static class GameGuiRenderer {

        private static GuiModel PausedOverlay;
        private static Text PausedText;
        private static GuiModel Background;
        private static Text DebugText;
        private static GuiModel Healthbar;
        private static GuiModel HealthbarTexture;
        private static Button btn_BackToTitle;
        internal static InGameTextbox TxtInput;
        internal static Text TextLog;

        private static Vector2 HealthBarSize = new Vector2(0.52f, 0.03f);
        private static Vector2 HealthBarTextureSize = new Vector2(0.55f, 0.04875f);
        private static Vector2 HealthBarPos = new Vector2(0f, -0.7f);

        private static HashSet<Button> Buttons;

        public static void Init() {
            TextFont.Init();

            TextStyle style = new TextStyle(TextAlignment.TopLeft, TextFont.LucidaConsole, 0.35f, 2f, int.MaxValue, 1f, new ColourRGBA(255, 255, 255));
            DebugText = new Text("", style, new Vector2(-0.99f, 0.97f));
            Healthbar = GuiModel.CreateRectangle(HealthBarSize, TextureUtil.ColourFromVec4(new Vector4(0.88f, 0.3f, 0.1f, 0.8f)));
            HealthbarTexture = GuiModel.CreateRectangle(HealthBarTextureSize, Textures.HealthbarTexture);
            Background = GuiModel.CreateRectangle(new Vector2(1, 1), Textures.GameBackgroundTex);
            PausedOverlay = GuiModel.CreateRectangle(new Vector2(1, 1), TextureUtil.ColourFromVec4(new Vector4(0.2f, 0.2f, 0.2f, 0.9f)));
            PausedText = new Text("Paused", new TextStyle(TextAlignment.Top, TextFont.LucidaConsole, 1.3f, 2f, 1, 1f, new ColourRGBA(255, 255, 255)), new Vector2(0, 1));
            TxtInput = new InGameTextbox(new Vector2(0, -0.5f), new Vector2(1f, 0.03f), TextFont.LucidaConsole, 0.5f);
            TextLog = new Text("", new TextStyle(TextAlignment.BottomLeft, TextFont.LucidaConsole, 0.35f, 2, 20, 1f, new ColourRGBA(255, 255, 255)), new Vector2(-1 + Textbox.TextOffset.x, -0.4f));

            Buttons = new HashSet<Button>();
            btn_BackToTitle = new Button(new Vector2(0, -0.2f), new Vector2(0.4f, 0.04f), "Save and Quit", TextStyle.LucidaConsole_SingleLine_Small, () => {
                GameLogic.SaveAndExit();
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
            GL.UseProgram(Gui.shader.ID);
            float ratio = Player.Instance.data.pos.y / TerrainGen.SizeY;
            ratio = CalcBackgroundColour(ratio);
            ratio *= 255;
            RenderInstance(Background, new Vector2(0, 0), new ColourRGBA(ratio, ratio, ratio));
            GL.UseProgram(0);
        }

        public static void Render() {
            GL.LineWidth(7);

            GL.UseProgram(Gui.shader.ID);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //healthbar
            RenderInstance(Healthbar, HealthBarPos);
            RenderInstance(HealthbarTexture, HealthBarPos);
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

                var textpos = new Vector2(inv.Pos.x, inv.Pos.y + 2 * (inv.Items.GetLength(1) - 0.5f) * inv.SizeY) - new Vector2(0, 2 * inv.SizeY - 0.01f);
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
            if (GameLogic.State == GameLogic.GameState.Text) {
                RenderText(TextLog);
            }

            if (GameLogic.State == GameLogic.GameState.Paused) {
                RenderInstance(PausedOverlay, new Vector2(0, 0), new ColourRGBA(127, 127, 127, 0.8f));

                RenderInstance(PausedText.model, PausedText.pos, PausedText.style.colour);
                RenderInstance(btn_BackToTitle.model, btn_BackToTitle.pos, btn_BackToTitle.colour);
                RenderInstance(btn_BackToTitle.text.model, btn_BackToTitle.text.pos, btn_BackToTitle.text.style.colour);
            }

            GL.UseProgram(0);

            GL.LineWidth(1);
            GL.Disable(EnableCap.Blend);

        }

        private static void RenderInstance(GuiModel model, Vector2 position) {
            RenderInstance(model, position, new ColourRGBA(255, 255, 255));
        }

        private static void RenderText(Text t) {
            RenderInstance(t.model, t.pos, t.style.colour);
        }

        private static void RenderInstance(GuiModel model, Vector2 position, ColourRGBA colour) {
            ShaderProgram shader = Gui.shader;
            shader.SetUniform2f("position", position);
            shader.SetUniform2f("size", model.size);
            shader.SetUniform4f("colour", colour.ToVec4());
            GL.BindVertexArray(model.vao.ID);
            GL.BindTexture(model.texture.TextureTarget, model.texture.TextureID);
            GL.DrawElements(model.drawmode, model.vao.Elements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindTexture(model.texture.TextureTarget, 0);
            GL.BindVertexArray(0);
        }
    }
}
