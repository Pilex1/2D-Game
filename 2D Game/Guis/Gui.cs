using Game.Fonts;
using Game.Interaction;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.TitleScreen {
    static class Gui {

        private static HashSet<Text> Texts = new HashSet<Text>();
        private static HashSet<Button> Buttons = new HashSet<Button>();

        public static ShaderProgram shader { get; private set; }

        public static void Init() {
            shader = new ShaderProgram(FileUtil.LoadShader("GuiVertex"), FileUtil.LoadShader("GuiFragment"));
        }

        public static void Dispose() {
            foreach (var b in Buttons) {
                b.Dispose();
            }
            Buttons.Clear();

            foreach (var t in Texts) {
                t.Dispose();
            }
            Texts.Clear();
        }

        public static void SwitchToTitleScreen() {
            Dispose();
            GameGuiRenderer.Dispose();
            RemoveAllText();
            RemoveAllButtons();
            TitleScreenRenderer.Init();
        }

        public static void SwitchToGame() {
            Dispose();
            TitleScreenRenderer.Dispose();
            RemoveAllText();
            RemoveAllButtons();
            GameGuiRenderer.Init();
        }

        public static void AddText(Text t) {
            Texts.Add(t);
        }

        public static void RemoveAllText() {
            Texts.Clear();
        }

        public static void AddButton(Button b) {
            Buttons.Add(b);
        }

        public static void RemoveAllButtons() {
            Buttons.Clear();
        }

        public static void Update() {
            foreach (var b in new List<Button>(Buttons)) {
                b.Update();
            }
        }

        public static void Render() {
            ProgramMode mode = Program.Mode;
            switch (mode) {
                case ProgramMode.Game:
                    GameGuiRenderer.Render();
                    break;
                case ProgramMode.TitleScreen:
                    TitleScreenRenderer.Render();
                    break;
            }

            Gl.UseProgram(shader);

            //render buttons
            foreach (var b in Buttons) {
                shader["position"].SetValue(b.pos);
                shader["size"].SetValue(b.size);
                shader["colour"].SetValue(b.active ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1));
                Gl.BindVertexArray(b.vao.ID);
                Gl.BindTexture(b.texture.TextureTarget,b.texture.TextureID);
                Gl.DrawElements(BeginMode.Triangles, b.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(b.texture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }

            //render text
            shader["aspectRatio"].SetValue(Program.AspectRatio);
            foreach (Text t in Texts) {
                shader["size"].SetValue(new Vector2(t.size, t.size));
                shader["position"].SetValue(t.pos);
                shader["colour"].SetValue(new Vector3(1, 1, 1));
                Gl.BindVertexArray(t.vao.ID);
                Gl.BindTexture(t.font.fontTexture.TextureTarget, t.font.fontTexture.TextureID);
                Gl.DrawElements(BeginMode.Triangles, t.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(t.font.fontTexture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }

            Gl.UseProgram(0);

        }
    }
}
