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
using Tao.FreeGlut;
using Game.Guis;
using System.Diagnostics;

namespace Game.TitleScreen {
    static class TitleScreenRenderer {

        enum State {
            None, Main, WorldPicker, Credits, Options
        }

        private static State state = State.None;

        private static GuiModel background;
        private static Vector2 backgroundpos;
        private static float backgrounddx;

        private static HashSet<Text> CurTexts = new HashSet<Text>();
        private static HashSet<Button> CurButtons = new HashSet<Button>();
        private static HashSet<Textbox> CurTextboxes = new HashSet<Textbox>();
        private static HashSet<Label> CurLabels = new HashSet<Label>();

        private static Button btnPlay, btnCredits, btnOptions, btnExit;
        private static Button btnBack;
        private static List<Button> btnWorldPickers;
        private static List<Textbox> txtboxWorldPickers;
        private static List<Label> lblWorldPickers;
        private static Text txtTitle;
        private static Text txtCreditsInfo;

        public static void Init() {

            Vector2 buttonSize = new Vector2(0.3, 0.08);

            btnPlay = new Button(new Vector2(0, 0.3), buttonSize, "Play", TextFont.Chiller, delegate () { SwitchTo(State.WorldPicker); });
            btnCredits = new Button(new Vector2(0, -0.05), buttonSize, "Credits", TextFont.Chiller, delegate () { SwitchTo(State.Credits); });
            btnOptions = new Button(new Vector2(0, -0.4), buttonSize, "Options", TextFont.Chiller, delegate () { SwitchTo(State.Options); });
            btnExit = new Button(new Vector2(0, -0.75), buttonSize, "Exit", TextFont.Chiller, delegate () { Glut.glutLeaveMainLoop(); });


            btnBack = new Button(new Vector2(0, -0.7), buttonSize, "Back", TextFont.Chiller, delegate () { SwitchTo(State.Main); });

            btnWorldPickers = new List<Button>();
            Vector2 worldpickerButtonSize = new Vector2(0.15, 0.04);
            Vector2 worldpickerLabelSize = new Vector2(0.25, 0.05);
            lblWorldPickers = new List<Label>();
            for (int i = 0; i < 5; i++) {
                Button btnNewWorld = new Button(new Vector2(0.2, 0.8 - i * 0.3), worldpickerButtonSize, "New World", TextFont.Chiller, 0.3f, delegate () {
                    Terrain.CreateNew();
                    Program.SwitchToGame();
                });
                btnWorldPickers.Add(btnNewWorld);
                Button btnLoadWorld = new Button(new Vector2(0.5, 0.8 - i * 0.3), worldpickerButtonSize, "Load World", TextFont.Chiller, 0.3f, delegate () {
                    if (Terrain.Load()) {
                        Program.SwitchToGame();
                    } else {
                        Console.WriteLine("Failed to load world");
                    }
                });
                btnWorldPickers.Add(btnLoadWorld);

                Label lblWorldName = new Label(new Vector2(-0.3, 0.8 - i * 0.3), worldpickerLabelSize, "<Empty World>", TextFont.Chiller, 0.4f);
                lblWorldPickers.Add(lblWorldName);
            }

            txtTitle = new Text("TITLE HERE", TextFont.Chiller, 2, new Vector2(0, 1), 2f);
            txtCreditsInfo = new Text("An open source project made by Alex Tan.", TextFont.Chiller, 0.5f, new Vector2(0, 0.7), 2f);

            background = InitBackground();
            backgroundpos = Vector2.Zero;
            backgrounddx = 0.000001f;

            SwitchTo(State.Main);
        }

        private static GuiModel InitBackground() {
            Vector2[] vertices = new Vector2[] {
                new Vector2(-1,1),
                new Vector2(-1,-1),
                new Vector2(1,-1),
                new Vector2(1,1)
            };

            Vector2[] uvs = new Vector2[] {
                backgroundpos+new Vector2(0,1),
                backgroundpos+new Vector2(0,0),
                backgroundpos+new Vector2(1f/Program.AspectRatio,0),
                backgroundpos+new Vector2(1f/Program.AspectRatio, 1)
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
            return new GuiModel(vao, texture, BeginMode.TriangleStrip, new Vector2(1,1));
        }

        private static void AddButton(Button b) {
            b.ResetCooldown();
            CurButtons.Add(b);
            CurTexts.Add(b.text);
        }

        private static void AddText(Text t) {
            CurTexts.Add(t);
        }

        private static void AddTextbox(Textbox t) {
            t.ResetCooldown();
            CurTexts.Add(t.text);
            CurTextboxes.Add(t);
        }

        private static void AddLabel(Label l) {
            CurTexts.Add(l.text);
            CurLabels.Add(l);
        }

        private static void SwitchTo(State state) {
            if (state == TitleScreenRenderer.state) return;
            CurTexts.Clear();
            CurButtons.Clear();
            CurTextboxes.Clear();
            CurLabels.Clear();
            switch (state) {
                case State.Main:
                    AddButton(btnPlay);
                    AddButton(btnCredits);
                    AddButton(btnOptions);
                    AddButton(btnExit);
                    AddText(txtTitle);
                    break;
                case State.WorldPicker:
                    foreach (var b in btnWorldPickers) {
                        AddButton(b);
                    }
                    foreach (var l in lblWorldPickers) {
                        AddLabel(l);
                    }
                    AddButton(btnBack);
                    break;
                case State.Credits:
                    AddButton(btnBack);
                    AddText(txtCreditsInfo);
                    break;
                case State.Options:
                    AddButton(btnBack);
                    break;
            }
            TitleScreenRenderer.state = state;
        }

        public static void Update() {
            foreach (var b in new List<Button>(CurButtons)) {
                b.Update();
            }
            backgroundpos.x += backgrounddx;
            background.vao.UpdateUVs(new Vector2[] {
                backgroundpos+new Vector2(0,1),
                backgroundpos+new Vector2(0,0),
                backgroundpos+new Vector2(1f/Program.AspectRatio,0),
                backgroundpos+new Vector2(1f/Program.AspectRatio, 1)
            });
            if (backgroundpos.x + 1 / Program.AspectRatio > 1 || backgroundpos.x < 0) backgrounddx *= -1;
        }

        public static void Render() {
            ShaderProgram shader = Gui.shader;
            Gl.UseProgram(shader.ProgramID);


            //background
            shader["position"].SetValue(Vector2.Zero);
            shader["size"].SetValue(new Vector2(1, 1));
            shader["aspectRatio"].SetValue(Program.AspectRatio);
            shader["colour"].SetValue(new Vector3(1, 0.9, 0.9));
            Gl.BindVertexArray(background.vao.ID);
            Gl.BindTexture(background.texture.TextureTarget, background.texture.TextureID);
            Gl.DrawElements(background.drawmode, background.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(background.texture.TextureTarget, 0);
            Gl.BindVertexArray(0);
            

            //render buttons
            foreach (var b in CurButtons) {
                shader["position"].SetValue(b.pos);
                shader["size"].SetValue(b.model.size);
                shader["colour"].SetValue(b.active ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1));
                Gl.BindVertexArray(b.model.vao.ID);
                Gl.BindTexture(b.model.texture.TextureTarget, b.model.texture.TextureID);
                Gl.DrawElements(b.model.drawmode, b.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(b.model.texture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }

            //render labels
            foreach (Label l in CurLabels) {
                shader["size"].SetValue(l.model.size);
                shader["position"].SetValue(l.pos);
                shader["colour"].SetValue(new Vector3(1, 1, 1));
                Gl.BindVertexArray(l.model.vao.ID);
                Gl.BindTexture(l.model.texture.TextureTarget, l.model.texture.TextureID);
                Gl.DrawElements(l.model.drawmode, l.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(l.model.texture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }



            //render text
            foreach (Text t in CurTexts) {
                shader["size"].SetValue(t.model.size);
                shader["position"].SetValue(t.pos);
                shader["colour"].SetValue(new Vector3(1, 1, 1));
                Gl.BindVertexArray(t.model.vao.ID);
                Gl.BindTexture(t.font.fontTexture.TextureTarget, t.font.fontTexture.TextureID);
                Gl.DrawElements(BeginMode.Triangles, t.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(t.font.fontTexture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }

            //render textboxes
            foreach (Textbox t in CurTextboxes) {
                shader["size"].SetValue(t.model.size);
                shader["position"].SetValue(t.pos);
                shader["colour"].SetValue(new Vector3(1, 1, 1));
                Gl.BindVertexArray(t.model.vao.ID);
                Gl.BindTexture(t.model.texture.TextureTarget, t.model.texture.TextureID);
                Gl.DrawElements(t.model.drawmode, t.model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                Gl.BindTexture(t.model.texture.TextureTarget, 0);
                Gl.BindVertexArray(0);
            }

            Gl.UseProgram(0);
        }

        public static void Dispose() {
            CurTexts.Clear();
            CurButtons.Clear();

            //btnPlay?.Dispose();
            //btnCredits?.Dispose();
            //btnOptions?.Dispose();
            if (btnPlay!=null)
                btnPlay.Dispose();
            if (btnCredits!=null)
                btnCredits.Dispose();
            if (btnOptions!=null)
                btnOptions.Dispose();

            if (btnWorldPickers != null)
                foreach (var b in btnWorldPickers) {
                    b.Dispose();
                }
            //btnBack?.Dispose();
            //txtTitle?.Dispose();
            //txtCreditsInfo?.Dispose();
            if (btnBack != null)
                btnBack.Dispose();
            if (txtTitle != null)
                txtTitle.Dispose();
            if (txtCreditsInfo != null)
                txtCreditsInfo.Dispose();

            btnPlay = btnCredits = btnOptions = btnBack = null;
            txtTitle = txtCreditsInfo = null;
        }
    }
}
