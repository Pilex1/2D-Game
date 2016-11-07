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
using Game.Util;

namespace Game.Interaction {
    static class GameGuiRenderer {

        public static BoolSwitch RenderDebugText { get; private set; }
        public static BoolSwitch Paused { get; private set; }

        private static GuiModel Background;
        private static Text DebugText;
        private static GuiModel Healthbar;
        private static Button btn_BackToTitle;

        private static HashSet<Button> Buttons;

        public static void Init() {
            TextFont.Init();
            Hotbar.Init();

            Paused = new BoolSwitch(false, 30);
            RenderDebugText = new BoolSwitch(false, 30);


            btn_BackToTitle = new Button(new Vector2(0, -0.5), new Vector2(0.25, 0.04), "Save and Quit", TextFont.Chiller, delegate () { Program.SwitchToTitleScreen(); });
            TextStyle style = new TextStyle(TextAlignment.TopLeft, TextFont.LucidaConsole, 0.5f, 2f, Int32.MaxValue, new Vector3(1, 1, 1));
            DebugText = new Text("", style, new Vector2(-0.99, 0.97));
            Healthbar = GuiModel.CreateRectangle(new Vector2(0.4, 0.04), Color.DarkRed);
            Background = GuiModel.CreateRectangle(new Vector2(1, 1), Textures.GameBackgroundTex);


            Buttons = new HashSet<Button>();
            // Buttons.Add(btn_BackToTitle);
        }

        public static void Update() {
            Hotbar.Update();
            Inventory.Update();
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
            float ratio = Player.Instance.data.Position.y / Terrains.Terrain.Tiles.GetLength(1);
            ratio = CalcBackgroundColour(ratio);
            RenderInstance(Background, new Vector2(0, 0), new Vector3(ratio, ratio, ratio));
            Gl.UseProgram(0);
        }

        public static void Render() {
            Gl.LineWidth(7);

            Gl.UseProgram(Gui.shader.ProgramID);

            //healthbar
            Healthbar.size.x = Player.Instance.data.life.val / Player.Instance.data.life.max * 0.5f;
            RenderInstance(Healthbar, new Vector2(0, -0.75));

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

            if (Paused) {
                //  RenderInstance(btn_BackToTitle.model, btn_BackToTitle.pos);
                //    RenderInstance(btn_BackToTitle.text.model, btn_BackToTitle.text.pos);
            }

            //hotbar
            RenderInstance(Hotbar.Frame, new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -1));
            RenderInstance(Hotbar.ItemDisplay, new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -1));
            RenderInstance(Hotbar.SelectedDisplay, new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2 + Hotbar.CurSelectedSlot * Hotbar.SizeX) - 1, -1));

            if (RenderDebugText)
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
            Inventory.Dispose();
            Hotbar.Dispose();
        }
    }
}
