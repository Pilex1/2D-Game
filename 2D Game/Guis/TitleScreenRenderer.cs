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

        private static GuiModel background;

        private static HashSet<Text> Texts = new HashSet<Text>();
        private static HashSet<Button> Buttons = new HashSet<Button>();

        public static void Init() {

            Vector2 buttonSize = new Vector2(0.3, 0.1);
            
            Buttons.Add(new Button(new Vector2(0, 0.3), buttonSize, "New World", TextFont.Chiller, delegate () {
                Terrain.CreateNew();
                Program.SwitchToGame();
            }));
            Buttons.Add(new Button(new Vector2(0, -0.1), buttonSize, "Load World", TextFont.Chiller, delegate () {
                if (Terrain.Load()) {
                    Program.SwitchToGame();
                } else {
                    Console.WriteLine("Failed to load world");
                }
            }));
            Buttons.Add(new Button(new Vector2(0, -0.5), buttonSize, "Credits", TextFont.Chiller, delegate () { }));

            foreach (var b in Buttons) {
                Texts.Add(b.text);
            }
            Texts.Add(new Text("TITLE HERE", TextFont.Chiller, 2, new Vector2(0, 1), 2f));

            background = InitBackground();
        }

        private static GuiModel InitBackground() {
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
                0,1,2,0,3
            };
            Texture texture = new Texture("Assets/TitleScreenBackground.png");
            Gl.BindTexture(texture.TextureTarget, texture.TextureID);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(texture.TextureTarget, 0);

            GuiVAO vao = new GuiVAO(vertices, elements, uvs);
            return new GuiModel(vao, texture, BeginMode.TriangleStrip);
        }

        public static void Update() {
            foreach (var b in new List<Button>(Buttons)) {
                b.Update();
            }
        }

        public static void Render() {
            ShaderProgram shader = Gui.shader;
            Gl.UseProgram(shader.ProgramID);

            //background
            shader["position"].SetValue(new Vector2(0, 0));
            shader["size"].SetValue(new Vector2(1, 1));
            shader["aspectRatio"].SetValue(Program.AspectRatio);
            shader["colour"].SetValue(new Vector3(1, 0.9, 0.9));
            Gl.BindVertexArray(background.vao.ID);
            Gl.BindTexture(background.texture.TextureTarget, background.texture.TextureID);
            Gl.DrawElements(background.drawmode, background.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(background.texture.TextureTarget, 0);
            Gl.BindVertexArray(0);

            //render buttons
            foreach (var b in Buttons) {
                shader["position"].SetValue(b.pos);
                shader["size"].SetValue(b.size);
                shader["colour"].SetValue(b.active ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1));
                Gl.BindVertexArray(b.vao.ID);
                Gl.BindTexture(b.texture.TextureTarget, b.texture.TextureID);
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
                Gl.BindVertexArray(t.model.vao.ID);
                Gl.BindTexture(t.font.fontTexture.TextureTarget, t.font.fontTexture.TextureID);
                Gl.DrawElements(BeginMode.Triangles, t.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(t.font.fontTexture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }

            Gl.UseProgram(0);
        }

        public static void Dispose() {
            background?.Dispose();
            background = null;
            foreach (var t in Texts) {
                t.Dispose();
            }
            Texts.Clear();
            foreach (var b in Buttons) {
                b.Dispose();
            }
            Buttons.Clear();
        }
    }
}
