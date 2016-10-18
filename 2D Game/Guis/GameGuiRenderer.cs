using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using Game.TitleScreen;
using Game.Core;
using Game.Fonts;
using Game.Guis;
using System.Drawing;
using Game.Assets;

namespace Game.Interaction {
    static class GameGuiRenderer {

        private static GuiModel Background;
        private static Text DebugText;
        private static GuiModel Healthbar;
        private static Button btnTitle;

        public static void Init() {
            TextFont.Init();
            Hotbar.Init();

            btnTitle = new Button(new Vector2(0, -0.5), new Vector2(0.5, 0.1), "Save and Quit", TextFont.Chiller, delegate () { Program.SwitchToTitleScreen(); });
            DebugText = new Text("", TextFont.CenturyGothic, 0.4f, new Vector2(-1, 1), 2f, Int32.MaxValue, TextAlignment.TopLeft);
            Healthbar = GuiModel.CreateRectangle(new Vector2(0.5, 0.04), Color.DarkRed);
            Background = GuiModel.CreateRectangle(new Vector2(1, 1), Asset.GameBackgroundTex);
        }

        public static void Update() {
            Hotbar.Update();
            Inventory.Update();
        }

        public static void SetDebugText(string str) {
            DebugText.SetText(str);
        }

        public static void RenderBackground() {
            Gl.UseProgram(Gui.shader.ProgramID);
            RenderInstance(Background, new Vector2(0, 0));
            Gl.UseProgram(0);
        }

        public static void Render() {
            Gl.LineWidth(7);

            Gl.UseProgram(Gui.shader.ProgramID);

            //healthbar
            Healthbar.size.x = Player.Instance.data.life.val / Player.Instance.data.life.max * 0.5f;
            RenderInstance(Healthbar, new Vector2(0, -0.65));

            //inventory
            if (Inventory.toggle) {
                RenderInstance(Inventory.Background, Inventory.Pos);
                RenderInstance(Inventory.Frame, Inventory.Pos);
                RenderInstance(Inventory.ItemDisplay, Inventory.Pos);
                if (Inventory.Selected.HasValue) {
                    int cx = Inventory.Selected.Value.x, cy = Inventory.Selected.Value.y;
                    RenderInstance(Inventory.SelectedDisplay, Inventory.Pos + new Vector2(cx * Hotbar.SizeX, cy * Hotbar.SizeY * Program.AspectRatio));
                }

                // RenderInstance(Inventory.textbg, Vector2.Zero);
                //RenderText(Inventory.text);
            }

            //hotbar
            RenderInstance(Hotbar.Frame, new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -1));
            RenderInstance(Hotbar.ItemDisplay, new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -1));
            RenderInstance(Hotbar.SelectedDisplay, new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2 + Hotbar.CurSelectedSlot * Hotbar.SizeX) - 1, -1));

            RenderText(DebugText);


            Gl.UseProgram(0);

            Gl.LineWidth(1);
        }

        private static void RenderInstance(GuiModel model, Vector2 position) {
            RenderInstance(model, position, new Vector3(1, 1, 1));
        }

        private static void RenderText(Text t) {
            ShaderProgram shader = Gui.shader;
            shader["size"].SetValue(t.model.size);
            shader["position"].SetValue(t.pos);
            shader["colour"].SetValue(t.colour);
            Gl.BindVertexArray(t.model.vao.ID);
            Gl.BindTexture(t.font.fontTexture.TextureTarget, t.font.fontTexture.TextureID);
            Gl.DrawElements(BeginMode.Triangles, t.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(t.font.fontTexture.TextureTarget, 0);
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
            Inventory.Dispose();
            Hotbar.Dispose();
        }
    }
}
