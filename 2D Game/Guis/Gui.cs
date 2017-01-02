using Game.Assets;
using Game.Guis;
using Game.Guis.Renderers;
using Game.Main.GLConstructs;
using Pencil.Gaming.Graphics;
namespace Game.TitleScreen {
    static class Gui {

        public static ShaderProgram shader { get; private set; }

        public static void Init() {
            shader = new ShaderProgram(Shaders.GuiVert, Shaders.GuiFrag);
            shader.AddUniform("position");
            shader.AddUniform("size");
            shader.AddUniform("colour");
            shader.AddUniform("aspectRatio");

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

            GL.UseProgram(shader.ID);
            GL.UseProgram(0);
        }
    }
}
