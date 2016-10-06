using Game.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using Game.Core;
using Game.Util;
using Game.Terrains;

namespace Game.TitleScreen {
    static class TitleScreenRenderer {

        private static GuiVAO background;
        private static Texture backgroundTexture;

        public static void Init() {

            Vector2 buttonSize = new Vector2(0.3, 0.1);

            Font font = new Font("Chiller");
            new Button(new Vector2(0, 0.3), buttonSize, "New World", font, delegate () {
                Terrain.CreateNew();
                Program.SwitchToGame();
            });
            new Button(new Vector2(0, -0.1), buttonSize, "Load World", font, delegate () {
                if (Terrain.Load()) {
                    Program.SwitchToGame();
                } else {
                    Console.WriteLine("Failed to load world");
                }
            });
            new Button(new Vector2(0, -0.5), buttonSize, "Credits", font, delegate () { });
            new Text("TITLE HERE", font, 2, new Vector2(0, 1), 2f);

            InitBackground();
        }

        private static void InitBackground() {
            Vector2[] vertices = new Vector2[] {
                new Vector2(-1,1),
                new Vector2(-1,-1),
                new Vector2(1,-1),
                new Vector2(1,1)
            };

            Vector2[] uvs = new Vector2[] {
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1f/Program.AspectRatio,0),
                new Vector2(1f/Program.AspectRatio, 1)
            };
            int[] elements = new int[] {
                0,1,2,2,3,0
            };
            backgroundTexture = new Texture("Assets/TitleScreenBackground.png");
            Gl.BindTexture(backgroundTexture.TextureTarget, backgroundTexture.TextureID);
            Gl.TexParameteri(backgroundTexture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(backgroundTexture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(backgroundTexture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(backgroundTexture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(backgroundTexture.TextureTarget, 0);
            background = new GuiVAO(vertices, elements, uvs);
        }


        public static void Render() {
            ShaderProgram shader = Gui.shader;
            Gl.UseProgram(shader.ProgramID);

            //background
            shader["position"].SetValue(new Vector2(0, 0));
            shader["size"].SetValue(new Vector2(1, 1));
            shader["aspectRatio"].SetValue(Program.AspectRatio);
            shader["colour"].SetValue(new Vector3(1, 0.9, 0.9));
            Gl.BindVertexArray(background.ID);
            Gl.BindTexture(backgroundTexture.TextureTarget, backgroundTexture.TextureID);
            Gl.DrawElements(BeginMode.Triangles, background.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(backgroundTexture.TextureTarget, 0);
            Gl.BindVertexArray(0);

            Gl.UseProgram(0);
        }

        public static void Dispose() {
            background.Dispose();
            backgroundTexture.Dispose();
            background = null;
            backgroundTexture = null;
        }
    }
}
