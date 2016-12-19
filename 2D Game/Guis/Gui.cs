using Game.Assets;
using Game.Guis;
using Game.Interaction;
using OpenGL;

namespace Game.TitleScreen {
    static class Gui {

        public static ShaderProgram shader { get; private set; }

        public static void Init() {
            shader = new ShaderProgram(Shaders.GuiVert,Shaders.GuiFrag);
            TextFont.Init();
            GameGuiRenderer.Init();
            TitleScreenRenderer.Init();
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
