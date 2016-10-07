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

        public static ShaderProgram shader { get; private set; }

        public static void Init() {
            shader = new ShaderProgram(FileUtil.LoadShader("GuiVertex"), FileUtil.LoadShader("GuiFragment"));
            TextFont.Init();
        }

        public static void SwitchToTitleScreen() {
            GameGuiRenderer.Dispose();
            TitleScreenRenderer.Init();
        }

        public static void SwitchToGame() {
            TitleScreenRenderer.Dispose();
            GameGuiRenderer.Init();
        }

        public static void Update() {
            switch (Program.Mode) {
                case ProgramMode.Game:
                    GameGuiRenderer.Update();
                    break;
                case ProgramMode.TitleScreen:
                    TitleScreenRenderer.Update();
                    break;
            }
        }

        public static void Render() {
            switch (Program.Mode) {
                case ProgramMode.Game:
                    GameGuiRenderer.Render();
                    break;
                case ProgramMode.TitleScreen:
                    TitleScreenRenderer.Render();
                    break;
            }

            Gl.UseProgram(shader);
            Gl.UseProgram(0);

        }

        public static void Dispose() {
            TitleScreenRenderer.Dispose();
            GameGuiRenderer.Dispose();
        }
    }
}
