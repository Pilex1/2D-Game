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
using Game.Assets;

namespace Game.TitleScreen {
    static class TitleScreenRenderer {

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
        private static Text txtTitle;
        private static Text txtCreditsInfo;

        private static WorldPicker curworldpicker;

        private class WorldPicker {

            internal bool emptyworld;
            internal Button btnNewWorld, btnLoadWorld, btnDeleteWorld;
            internal Button btnLaunchNewWorld;
            internal RestrictedTextbox txtboxWorldName;

            internal WorldPicker(string worldname, int i) {

                Vector2 worldpickerButtonSize = new Vector2(0.15, 0.03);
                Vector2 worldpickerTextboxSize = new Vector2(0.3, 0.03);

                btnNewWorld = new Button(new Vector2(0.2, 0.85 - i * 0.3), worldpickerButtonSize, "New World", TextFont.Chiller, 0.6f, delegate () {
                    btnNewWorld.disabled = true;
                    btnLoadWorld.disabled = true;
                    btnDeleteWorld.disabled = true;
                    txtboxWorldName.active = true;
                    txtboxWorldName.SetText("");
                    txtboxWorldName.cooldown.SetTime(-20);
                    btnLaunchNewWorld.disabled = true;
                    AddButton(btnLaunchNewWorld);
                    curworldpicker = this;
                    // Terrain.CreateNew();
                    //  Program.SwitchToGame();
                });
                btnLoadWorld = new Button(new Vector2(0.51, 0.85 - i * 0.3), worldpickerButtonSize, "Load World", TextFont.Chiller, 0.6f, delegate () {
                    try {
                        WorldData worlddata = Serialization.LoadWorld(txtboxWorldName.ToString());
                        Program.worldname = txtboxWorldName.ToString();
                        Program.SwitchToGame(worlddata);
                    } catch (Exception e) {
                        //if world file failed to load (usually because it's corrupted), then delete the world
                        Console.WriteLine(e.Message);
                        btnDeleteWorld.OnPress();
                    }
                });
                btnDeleteWorld = new Button(new Vector2(0.2, 0.85 - i * 0.3 - 0.12), worldpickerButtonSize, "Delete World", TextFont.Chiller, 0.6f, delegate () {
                    if (!emptyworld)
                        try {
                            Serialization.DeleteWorld(txtboxWorldName.ToString());
                            txtboxWorldName.SetText("<Empty World>");
                            btnLoadWorld.disabled = true;
                            btnDeleteWorld.disabled = true;
                        } catch (Exception) { }
                });
                txtboxWorldName = new RestrictedTextbox(new Vector2(-0.45, 0.85 - i * 0.3), worldpickerTextboxSize, TextFont.Chiller, 0.6f);
                txtboxWorldName.SetText(worldname ?? "<Empty World>");
                emptyworld = worldname == null;
                btnLoadWorld.disabled = emptyworld;
                btnDeleteWorld.disabled = emptyworld;
                txtboxWorldPickers.Add(txtboxWorldName);
                btnLaunchNewWorld = new Button(new Vector2(-0.05, 0.85 - i * 0.3), new Vector2(0.08, 0.03), "Create", TextFont.Chiller, 0.6f, delegate () {
                    Debug.Assert(curworldpicker != null);
                    string curworldname = curworldpicker.txtboxWorldName.ToString();
                    Program.worldname = curworldname;
                    Program.SwitchToGame(null);
                });

                btnWorldPickers.Add(btnNewWorld);
                btnWorldPickers.Add(btnLoadWorld);
                btnWorldPickers.Add(btnDeleteWorld);
                txtboxWorldPickers.Add(txtboxWorldName);
            }
        }

        enum State {
            None, Main, WorldPicker, NewWorld, Credits, Options
        }

        public static void Init() {

            Vector2 buttonSize = new Vector2(0.3, 0.08);

            btnPlay = new Button(new Vector2(0, 0.3), buttonSize, "Play", TextFont.Chiller, delegate () { SwitchTo(State.WorldPicker); });
            btnCredits = new Button(new Vector2(0, -0.05), buttonSize, "Credits", TextFont.Chiller, delegate () { SwitchTo(State.Credits); });
            btnOptions = new Button(new Vector2(0, -0.4), buttonSize, "Options", TextFont.Chiller, delegate () { SwitchTo(State.Options); });
            btnExit = new Button(new Vector2(0, -0.75), buttonSize, "Exit", TextFont.Chiller, delegate () { Glut.glutLeaveMainLoop(); });

            btnBack = new Button(new Vector2(0, -0.7), buttonSize, "Back", TextFont.Chiller, delegate () { SwitchTo(State.Main); });

            btnWorldPickers = new List<Button>();

            txtboxWorldPickers = new List<Textbox>();
            string[] worlds = Serialization.GetWorlds();
            for (int i = 0; i < 5; i++) {
                new WorldPicker(i < worlds.Length ? worlds[i] : null, i);
            }

            txtTitle = new Text("TITLE HERE", TextFont.Chiller, 3, new Vector2(0, 1), 2f, 1, TextAlignment.CenterCenter);
            txtCreditsInfo = new Text("An open source project made by Alex Tan", TextFont.Chiller, 0.6f, new Vector2(0, 0.7), 2f, Int32.MaxValue, TextAlignment.CenterCenter);

            backgroundpos = Vector2.Zero;
            background = GuiModel.CreateRectangle(new Vector2(1, 1), Asset.TitleBackgroundTex);
            backgrounddx = 0.000001f;

            SwitchTo(State.Main);
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
                    foreach (var t in txtboxWorldPickers) {
                        AddTextbox(t);
                        t.cooldown.SetTime(-10);
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
            foreach (var t in new List<Textbox>(CurTextboxes)) {
                t.Update();
            }
            backgroundpos.x += backgrounddx;
            background.vao.UpdateUVs(new Vector2[] {
                backgroundpos+new Vector2(0,1),
                backgroundpos+new Vector2(0,0),
                backgroundpos+new Vector2(1f/Program.AspectRatio,0),
                backgroundpos+new Vector2(1f/Program.AspectRatio, 1)
            });
            if (backgroundpos.x + 1 / Program.AspectRatio > 1 || backgroundpos.x < 0) backgrounddx *= -1;

            if (curworldpicker != null) {
                string worldname = curworldpicker.txtboxWorldName.ToString();
                curworldpicker.btnLaunchNewWorld.disabled = worldname.Length == 0 || worldname.StartsWith(" ") || StringUtil.StartsWithNumber(worldname);
            }
        }

        private static void RenderInstance(GuiModel model, Vector2 pos, Vector3 colour) {
            ShaderProgram shader = Gui.shader;

            shader["position"].SetValue(pos);
            shader["size"].SetValue(model.size);
            shader["colour"].SetValue(colour);
            Gl.BindVertexArray(model.vao.ID);
            Gl.BindTexture(model.texture.TextureTarget, model.texture.TextureID);
            Gl.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(model.texture.TextureTarget, 0);
            Gl.BindVertexArray(0);
        }

        public static void Render() {
            ShaderProgram shader = Gui.shader;
            Gl.UseProgram(shader.ProgramID);

            shader["aspectRatio"].SetValue(Program.AspectRatio);

            RenderInstance(background, Vector2.Zero, new Vector3(1, 0.9, 0.9));

            foreach (var b in CurButtons)
                RenderInstance(b.model, b.pos, b.colour);

            foreach (Label l in CurLabels)
                RenderInstance(l.model, l.pos, new Vector3(1, 1, 1));


            foreach (Textbox t in CurTextboxes)
                RenderInstance(t.model, t.pos, t.active ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1));

            foreach (Text t in CurTexts)
                RenderInstance(t.model, t.pos, t.colour);

            Gl.UseProgram(0);
        }

        public static void Dispose() {

            foreach (var t in CurTexts) t.Dispose();
            foreach (var b in CurButtons) b.Dispose();
            foreach (var t in CurTextboxes) t.Dispose();
            foreach (var l in CurLabels) l.Dispose();

            CurTexts.Clear();
            CurButtons.Clear();
            CurTextboxes.Clear();
            CurLabels.Clear();

            //btnPlay?.Dispose();
            //btnCredits?.Dispose();
            //btnOptions?.Dispose();
            if (btnPlay != null)
                btnPlay.Dispose();
            if (btnCredits != null)
                btnCredits.Dispose();
            if (btnOptions != null)
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
