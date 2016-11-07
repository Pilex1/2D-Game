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

        #region Fields
        private static State state = State.None;

        private static GuiModel background;
        private static Vector2 backgroundpos;
        private static float backgrounddx;

        private static HashSet<Text> CurTexts = new HashSet<Text>();
        private static HashSet<Button> CurButtons = new HashSet<Button>();
        private static HashSet<Textbox> CurTextboxes = new HashSet<Textbox>();
        private static HashSet<Label> CurLabels = new HashSet<Label>();

        private static Button btn_Main_Play, btn_Main_Credits, btn_Main_Options, btn_Main_Exit;
        private static Text txt_Main_Title;

        private static Button btn_Back_Title;
        private static Button btn_Back_WorldSelect;

        private static Textbox txtbx_NewWorld_Name;
        private static Textbox txtbx_NewWorld_Seed;
        private static Text txt_NewWorld_Name;
        private static Text txt_NewWorld_Seed;
        private static Button btn_NewWorld_CreateWorld;

        private static List<Button> btnWorldPickers;
        private static List<Textbox> txtboxWorldPickers;

        private static Text txt_CreditsInfo;
        #endregion

        enum State {
            None, Main, WorldPicker, NewWorld, Credits, Options
        }

        #region Init
        private class WorldPicker {

            internal bool emptyworld;
            internal Button btnNewWorld, btnDeleteWorld;
            internal Button btnWorldName;

            internal WorldPicker(string worldname, int i) {

                Vector2 buttonSize = new Vector2(0.15, 0.03);
                Vector2 worldButtonSize = new Vector2(0.3, 0.03);

                btnNewWorld = new Button(new Vector2(0.2, 0.85 - i * 0.3), buttonSize, "New World", TextFont.Chiller, 0.6f, delegate () {
                    SwitchTo(State.NewWorld);
                });
                btnWorldName = new Button(new Vector2(-0.45, 0.85 - i * 0.3), worldButtonSize, worldname, TextFont.Chiller, 0.6f, delegate () {
                    try {
                        WorldData worlddata = Serialization.LoadWorld(btnWorldName.ToString());
                        Program.worldname = btnWorldName.ToString();
                        Program.SwitchToGame(worlddata);
                    } catch (Exception e) {
                        //if world file failed to load (usually because it's corrupted), then delete the world
                        Console.WriteLine(e.Message);
                        btnDeleteWorld.OnPress();
                    }
                });
                //btnWorldName.SetTextAlignment(TextAlignment.CenterCenter);

                btnDeleteWorld = new Button(new Vector2(0.2, 0.85 - i * 0.3 - 0.12), buttonSize, "Delete World", TextFont.Chiller, 0.6f, delegate () {
                    if (!emptyworld)
                        try {
                            Serialization.DeleteWorld(btnWorldName.ToString());
                            btnWorldName.SetText("<Empty World>");
                            btnWorldName.disabled = true;
                            btnDeleteWorld.disabled = true;
                        } catch (Exception) { }
                });

                emptyworld = worldname == null;
                btnNewWorld.disabled = !emptyworld;
                btnWorldName.disabled = emptyworld;
                btnDeleteWorld.disabled = emptyworld;

                btnWorldPickers.Add(btnNewWorld);
                btnWorldPickers.Add(btnWorldName);
                btnWorldPickers.Add(btnDeleteWorld);
            }
        }


        public static void Init() {

            Vector2 buttonSize = new Vector2(0.3, 0.08);

            {
                //home screen
                btn_Main_Play = new Button(new Vector2(0, 0.3), buttonSize, "Play", TextFont.Chiller, delegate () { SwitchTo(State.WorldPicker); });
                btn_Main_Credits = new Button(new Vector2(0, -0.05), buttonSize, "Credits", TextFont.Chiller, delegate () { SwitchTo(State.Credits); });
                btn_Main_Options = new Button(new Vector2(0, -0.4), buttonSize, "Options", TextFont.Chiller, delegate () { SwitchTo(State.Options); });
                btn_Main_Exit = new Button(new Vector2(0, -0.75), buttonSize, "Exit", TextFont.Chiller, delegate () { Glut.glutLeaveMainLoop(); });
            }



            txt_Main_Title = new Text("TITLE HERE", new TextStyle(TextAlignment.CenterCenter, TextFont.Chiller, 3f, 2f, 1, new Vector3(1, 1, 1)), new Vector2(0, 1));
            backgroundpos = Vector2.Zero;
            float s = 1f / Program.AspectRatio;
            float imageAspectRatio = (float)Textures.TitleBackgroundTex.Size.Width / Textures.TitleBackgroundTex.Size.Height;
            background = GuiModel.CreateRectangle(new Vector2(s * imageAspectRatio, s), Textures.TitleBackgroundTex);
            background.vao.UpdateUVs(new Vector2[] {
                backgroundpos+new Vector2(0,1f),
                backgroundpos+new Vector2(0,0),
                backgroundpos+new Vector2(1,0),
                backgroundpos+new Vector2(1, 1f)
            });
            backgrounddx = 0.00001f;
            //backgrounddx = 0;


            //credits screen
            txt_CreditsInfo = new Text("An open source project made by Alex Tan", new TextStyle(TextAlignment.CenterCenter, TextFont.Chiller, 0.6f, 2, Int32.MaxValue, new Vector3(1, 1, 1)), new Vector2(0, 0.7));


            //picking a world
            btnWorldPickers = new List<Button>();

            txtboxWorldPickers = new List<Textbox>();
            string[] worlds = Serialization.GetWorlds();
            for (int i = 0; i < 5; i++) {
                new WorldPicker(i < worlds.Length ? worlds[i] : null, i);
            }

            {
                //creating new world
                txtbx_NewWorld_Name = new Textbox(new Vector2(0, 0.5), new Vector2(0.3, 0.03), TextFont.Chiller, 0.6f);
                txtbx_NewWorld_Seed = new Textbox(new Vector2(0, 0.2), new Vector2(0.3, 0.03), TextFont.Chiller, 0.6f);
                float offset = 0.15f;
                TextStyle style = new TextStyle(TextAlignment.CenterCenter, TextFont.Chiller, 1f, 1f, 1, new Vector3(1, 1, 1));
                txt_NewWorld_Name = new Text("World Name", style, new Vector2(0, 0.5 + offset));
                txt_NewWorld_Seed = new Text("Seed", style, new Vector2(0, 0.2 + offset));
                btn_NewWorld_CreateWorld = new Button(new Vector2(0, -0.3), new Vector2(0.3, 0.08), "Create", TextFont.Chiller, delegate () {
                    Program.SwitchToGame(txtbx_NewWorld_Name.GetText(), Int32.Parse(txtbx_NewWorld_Seed.GetText()));
                });
            }

            btn_Back_Title = new Button(new Vector2(0, -0.7), buttonSize, "Back", TextFont.Chiller, delegate () { SwitchTo(State.Main); });
            btn_Back_WorldSelect = new Button(new Vector2(0, -0.7), buttonSize, "Back", TextFont.Chiller, delegate () { SwitchTo(State.WorldPicker); });

            SwitchTo(State.Main);
        }
        #endregion

        #region Adding Components

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
                    AddButton(btn_Main_Play);
                    AddButton(btn_Main_Credits);
                    AddButton(btn_Main_Options);
                    AddButton(btn_Main_Exit);
                    AddText(txt_Main_Title);
                    break;

                case State.WorldPicker:
                    foreach (var b in btnWorldPickers) {
                        AddButton(b);
                    }
                    foreach (var t in txtboxWorldPickers) {
                        AddTextbox(t);
                        t.cooldown.SetTime(-10);
                    }
                    AddButton(btn_Back_Title);
                    break;

                case State.Credits:
                    AddButton(btn_Back_Title);
                    AddText(txt_CreditsInfo);
                    break;

                case State.Options:
                    AddButton(btn_Back_Title);
                    break;

                case State.NewWorld:
                    AddTextbox(txtbx_NewWorld_Name);
                    AddTextbox(txtbx_NewWorld_Seed);
                    AddText(txt_NewWorld_Name);
                    AddText(txt_NewWorld_Seed);
                    AddButton(btn_NewWorld_CreateWorld);
                    AddButton(btn_Back_WorldSelect);
                    break;
            }
            TitleScreenRenderer.state = state;
        }

        #endregion

        #region Update & Render



        public static void Update() {
            foreach (var b in new List<Button>(CurButtons)) {
                b.Update();
            }
            foreach (var t in new List<Textbox>(CurTextboxes)) {
                t.Update();
            }

            backgroundpos.x -= backgrounddx;
            if (backgroundpos.x < Program.AspectRatio - (float)Textures.TitleBackgroundTex.Size.Width / Textures.TitleBackgroundTex.Size.Height) {
                backgrounddx *= -1;
            }
            // Debug.WriteLine(backgroundpos);

            if (state == State.NewWorld) {
                string worldname = txtbx_NewWorld_Name.GetText();
                string seed = txtbx_NewWorld_Seed.GetText();
                btn_NewWorld_CreateWorld.disabled = worldname.Length == 0 || worldname.StartsWith(" ") || StringUtil.StartsWithNumber(worldname) || seed.Length == 0 || !StringUtil.IsNumeric(seed);
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

            RenderInstance(background, backgroundpos, new Vector3(1, 0.9, 0.9));

            //  Debug.WriteLine(btn_NewWorld_CreateWorld.colour);

            foreach (var b in CurButtons)
                RenderInstance(b.model, b.pos, b.colour);

            foreach (Label l in CurLabels)
                RenderInstance(l.model, l.pos, new Vector3(1, 1, 1));


            foreach (Textbox t in CurTextboxes)
                RenderInstance(t.model, t.pos, t.active ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1));

            foreach (Text t in CurTexts)
                RenderInstance(t.model, t.pos, t.style.colour);

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
            if (btn_Main_Play != null)
                btn_Main_Play.Dispose();
            if (btn_Main_Credits != null)
                btn_Main_Credits.Dispose();
            if (btn_Main_Options != null)
                btn_Main_Options.Dispose();

            if (btnWorldPickers != null)
                foreach (var b in btnWorldPickers) {
                    b.Dispose();
                }
            //btnBack?.Dispose();
            //txtTitle?.Dispose();
            //txtCreditsInfo?.Dispose();
            if (btn_Back_Title != null)
                btn_Back_Title.Dispose();
            if (txt_Main_Title != null)
                txt_Main_Title.Dispose();
            if (txt_CreditsInfo != null)
                txt_CreditsInfo.Dispose();

            btn_Main_Play = btn_Main_Credits = btn_Main_Options = btn_Back_Title = null;
            txt_Main_Title = txt_CreditsInfo = null;
        }
        #endregion  
    }
}