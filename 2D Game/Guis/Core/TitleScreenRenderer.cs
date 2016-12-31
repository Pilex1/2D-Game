using Game.Fonts;
using System;
using System.Collections.Generic;
using OpenGL;
using Game.Core;
using Game.Util;
using Tao.FreeGlut;
using Game.Guis;
using Game.Assets;
using Game.Core.World_Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace Game.TitleScreen {
    static class TitleScreenRenderer {

        #region Fields
        private static State state = State.None;

        private static GuiModel background;
        private static Vector2 backgroundpos;
        private static float backgroundhue;

        private static HashSet<Text> CurTexts = new HashSet<Text>();
        private static HashSet<Button> CurButtons = new HashSet<Button>();
        private static HashSet<Textbox> CurTextboxes = new HashSet<Textbox>();
        private static HashSet<Label> CurLabels = new HashSet<Label>();

        private static Button btn_Main_Play, btn_Main_Credits, btn_Main_Help, btn_Main_Exit;
        private static Text txt_Main_Title;

        private static Text txt_Help;

        private static Button btn_Back_Title;
        private static Button btn_NewWorld_Back;

        private static Textbox txtbx_NewWorld_Name;
        private static Textbox txtbx_NewWorld_Seed;
        private static Text txt_NewWorld_Name;
        private static Text txt_NewWorld_Seed;
        private static Button btn_NewWorld_CreateWorld;
        private static Button btn_NewWorld_RandSeed;

        private static List<WorldPicker> worldPickers;

        private static Text txt_CreditsInfo;

        private static HashSet<string> worlds;
        #endregion

        enum State {
            None, Main, WorldSelect, NewWorld, Credits, Help
        }

        #region Init
        private class WorldPicker {

            internal bool emptyworld;
            internal Button btnNewWorld, btnDeleteWorld;
            internal Button btnLaunchWorld;

            internal WorldPicker(string worldname, int i) {

                Vector2 buttonSize = new Vector2(0.15, 0.03);
                Vector2 worldButtonSize = new Vector2(0.3, 0.03);

                btnNewWorld = new Button(new Vector2(0.2, 0.85 - i * 0.3), buttonSize, "New World", TextStyle.LucidaConsole_SingleLine_Small, () =>
                    SwitchTo(State.NewWorld));
                btnLaunchWorld = new Button(new Vector2(-0.45, 0.85 - i * 0.3), worldButtonSize, worldname, TextStyle.LucidaConsole_SingleLine_Small, () => Program.LoadGame_FromSave(btnLaunchWorld.ToString()));

                btnDeleteWorld = new Button(new Vector2(0.2, 0.85 - i * 0.3 - 0.12), buttonSize, "Delete World", TextStyle.LucidaConsole_SingleLine_Small, async () => {
                    if (emptyworld) return;
                    string w = btnLaunchWorld.ToString();
                    btnLaunchWorld.disabled = true;
                    btnDeleteWorld.disabled = true;

                    await Task.Factory.StartNew(() => Serialization.DeleteWorld(w));

                    worlds.Remove(w);
                    btnLaunchWorld.SetText("");
                    btnNewWorld.disabled = false;
                }
                );

                emptyworld = worldname == null;
                btnNewWorld.disabled = !emptyworld;
                btnLaunchWorld.disabled = emptyworld;
                btnDeleteWorld.disabled = emptyworld;
            }
        }


        public static void Init() {

            Vector2 buttonSize = new Vector2(0.3, 0.08);
            btn_Back_Title = new Button(new Vector2(0, -0.7), buttonSize, "Back", TextStyle.LucidaConsole_SingleLine_Large, () => SwitchTo(State.Main));
            backgroundhue = 180;

            #region Home Screen
            btn_Main_Play = new Button(new Vector2(0, 0.3), buttonSize, "Play", TextStyle.LucidaConsole_SingleLine_Large, delegate () { SwitchTo(State.WorldSelect); });
            btn_Main_Credits = new Button(new Vector2(0, -0.05), buttonSize, "Credits", TextStyle.LucidaConsole_SingleLine_Large, delegate () { SwitchTo(State.Credits); });
            btn_Main_Help = new Button(new Vector2(0, -0.4), buttonSize, "Help", TextStyle.LucidaConsole_SingleLine_Large, delegate () { SwitchTo(State.Help); });
            btn_Main_Exit = new Button(new Vector2(0, -0.75), buttonSize, "Exit", TextStyle.LucidaConsole_SingleLine_Large, delegate () { Glut.glutLeaveMainLoop(); });
            #endregion

            #region Background
            txt_Main_Title = new Text("HIC TITULUS EST", new TextStyle(TextAlignment.Center, TextFont.Chiller, 3f, 2f, 1, 1f, new Vector3(1, 1, 1)), new Vector2(0, 1));
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
            #endregion

            #region Credits
            TextStyle tstyle = new TextStyle(TextAlignment.Center, TextFont.LucidaConsole, 0.6f, 1.8f, 1 << 30, 1f, new Vector3(1, 1, 1));
            txt_CreditsInfo = new Text("Copyright Alex Tan (2016). Apache License. https://github.com/Pilex1/2D-Game/blob/master/LICENSE" + Environment.NewLine + Environment.NewLine + "- code, bugs, game design, bugs, graphics, bugs, everything you see here, bugs. Did I mention bugs?" + Environment.NewLine + Environment.NewLine + "Help me continue making these projects:" + Environment.NewLine + Environment.NewLine + "Visit my github repositories to view and download the full source code plus my other projects" + Environment.NewLine + " - https://github.com/Pilex1" + Environment.NewLine + " - https://github.com/Pilex1/2D-Game" + Environment.NewLine + Environment.NewLine + "Subscribe to my youtube channel where I post videos of Mandelbrot renders and game development" + Environment.NewLine + " - Pilex", tstyle, new Vector2(0, 0.9));
            #endregion

            #region World Pickers
            worldPickers = new List<WorldPicker>();

            worlds = new HashSet<string>();
            foreach (var w in Serialization.GetWorlds()) {
                worlds.Add(w);
            }
            LoadWorldPickers();
            #endregion

            #region New World
            //creating new world
            txtbx_NewWorld_Name = new Textbox(new Vector2(0, 0.5), new Vector2(0.3, 0.03), TextFont.LucidaConsole, 0.6f);
            txtbx_NewWorld_Seed = new Textbox(new Vector2(0, 0.2), new Vector2(0.3, 0.03), TextFont.LucidaConsole, 0.6f);
            float offset = 0.15f;
            txt_NewWorld_Name = new Text("World Name", tstyle, new Vector2(0, 0.5 + offset));
            txt_NewWorld_Seed = new Text("Seed", tstyle, new Vector2(0, 0.2 + offset));
            btn_NewWorld_CreateWorld = new Button(new Vector2(0, -0.3), new Vector2(0.3, 0.08), "Create", TextStyle.LucidaConsole_SingleLine_Large, () => {
                string worldname = txtbx_NewWorld_Name.GetText();
                worlds.Add(worldname);
                Program.LoadGame_New(worldname, int.Parse(txtbx_NewWorld_Seed.GetText()));
            });
            btn_NewWorld_RandSeed = new Button(new Vector2(0.5, 0.2), new Vector2(0.15, 0.03), "Random seed", TextStyle.LucidaConsole_SingleLine_Small, () => {
                int irand = MathUtil.RandInt(Program.Rand, 1 << 24, 1 << 30);
                txtbx_NewWorld_Seed.SetText(irand.ToString());
            });
            btn_NewWorld_Back = new Button(new Vector2(0, -0.7), buttonSize, "Back", TextStyle.LucidaConsole_SingleLine_Large, () => {
                txtbx_NewWorld_Name.ClearText();
                txtbx_NewWorld_Name.disabled = true;

                txtbx_NewWorld_Seed.ClearText();
                txtbx_NewWorld_Seed.disabled = true;

                SwitchTo(State.WorldSelect);
            });
            #endregion

            #region Help
            txt_Help = new Text("How to play:" + Environment.NewLine + "W A S D - movement" + Environment.NewLine + "Left click - destroy tile" + Environment.NewLine + "Right click - interact / place tile" + Environment.NewLine + "E - open / close inventory" + Environment.NewLine + "Mousewheel and keys 1 to 9 - hotbar selection" + Environment.NewLine + "Escape - pause game" + Environment.NewLine + Environment.NewLine + "Debugging controls:" + Environment.NewLine + "F1 - debug view" + Environment.NewLine + "F2 - render hitboxes" + Environment.NewLine + "F3 - smooth lighting" + Environment.NewLine + "Enter - open command prompt", tstyle, new Vector2(0, 0.9));
            #endregion

            SwitchTo(State.Main);
        }
        #endregion

        public static void Reset() {
            SwitchTo(State.Main);
            LoadWorldPickers();
            txtbx_NewWorld_Name.ClearText();
            txtbx_NewWorld_Seed.ClearText();
        }

        private static void SetWorldPickerDisabledState(string world, bool val) {
            if (world == "") return;
            foreach (WorldPicker w in worldPickers) {
                if (w.btnLaunchWorld.text.ToString() == world) {
                    w.btnLaunchWorld.disabled = val;
                    w.btnDeleteWorld.disabled = val;
                }
            }
        }

        private static void AddWorldPicker(string world, int i) {
            WorldPicker w = new WorldPicker(world, i);
            worldPickers.Add(w);
        }

        private static void LoadWorldPickers() {
            string[] worldsarr = worlds.ToArray();
            worldPickers.Clear();
            for (int i = 0; i < 5; i++) {
                AddWorldPicker(i < worldsarr.Length ? worldsarr[i] : null, i);
            }
        }

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
                    AddButton(btn_Main_Help);
                    AddButton(btn_Main_Exit);
                    AddText(txt_Main_Title);
                    break;

                case State.WorldSelect:
                    foreach (var w in worldPickers) {
                        AddButton(w.btnDeleteWorld);
                        AddButton(w.btnLaunchWorld);
                        AddButton(w.btnNewWorld);
                    }
                    AddButton(btn_Back_Title);
                    break;

                case State.Credits:
                    AddButton(btn_Back_Title);
                    AddText(txt_CreditsInfo);
                    break;

                case State.Help:
                    AddText(txt_Help);
                    AddButton(btn_Back_Title);
                    break;

                case State.NewWorld:
                    AddTextbox(txtbx_NewWorld_Name);
                    AddTextbox(txtbx_NewWorld_Seed);
                    AddText(txt_NewWorld_Name);
                    AddText(txt_NewWorld_Seed);
                    AddButton(btn_NewWorld_RandSeed);
                    AddButton(btn_NewWorld_CreateWorld);
                    AddButton(btn_NewWorld_Back);
                    break;
            }

            foreach (var b in CurButtons) {
                b.cooldown.SetTime(0);
            }
            foreach (var t in CurTextboxes) {
                t.cooldown.SetTime(0);
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

            backgroundhue += GameTime.DeltaTime / 25;

            SetWorldPickerDisabledState(Program.worldname, GameLogic.saving);

            if (state == State.NewWorld) {
                string worldname = txtbx_NewWorld_Name.GetText();
                string seed = txtbx_NewWorld_Seed.GetText();

                btn_NewWorld_CreateWorld.disabled = !IsValidNewWorld(worldname, seed);
            }


        }

        public static void Render() {
            ShaderProgram shader = Gui.shader;
            Gl.UseProgram(shader.ProgramID);

            shader["aspectRatio"].SetValue(Program.AspectRatio);

            RenderInstance(background, backgroundpos, new Vector4(TextureUtil.HSVToRGB_Vec3(backgroundhue, 0.5f, 1f), 1));
            // Debug.WriteLine(backgroundhue);

            foreach (var b in CurButtons)
                RenderInstance(b.model, b.pos, b.colour);

            foreach (Label l in CurLabels)
                RenderInstance(l.model, l.pos, new Vector4(1, 1, 1, 1));


            foreach (Textbox t in CurTextboxes)
                RenderInstance(t.model, t.pos, t.colour);

            foreach (Text t in CurTexts)
                RenderInstance(t.model, t.pos, t.style.colour);

            Gl.UseProgram(0);
        }


        private static bool IsValidNewWorld(string name, string seed) {
            if (!StringUtil.StartsWithLetter(name)) return false;
            int i_seed;
            if (!int.TryParse(seed, out i_seed)) return false;
            if (worlds.Contains(name)) return false;

            return true;
        }

        private static void RenderInstance(GuiModel model, Vector2 pos, Vector4 colour) {
            ShaderProgram shader = Gui.shader;

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            shader["position"].SetValue(pos);
            shader["size"].SetValue(model.size);
            shader["colour"].SetValue(colour);
            Gl.BindVertexArray(model.vao.ID);
            Gl.BindTexture(model.texture.TextureTarget, model.texture.TextureID);
            Gl.DrawElements(model.drawmode, model.vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(model.texture.TextureTarget, 0);
            Gl.BindVertexArray(0);

            Gl.Disable(EnableCap.Blend);
        }

        public static void Dispose() {
            if (Program.Mode != ProgramMode.TitleScreen) return;


            foreach (var t in CurTexts) t.Dispose();
            foreach (var b in CurButtons) b.Dispose();
            foreach (var t in CurTextboxes) t.Dispose();
            foreach (var l in CurLabels) l.Dispose();

            CurTexts.Clear();
            CurButtons.Clear();
            CurTextboxes.Clear();
            CurLabels.Clear();

            btn_Main_Play.Dispose();
            btn_Main_Credits.Dispose();
            btn_Main_Help.Dispose();

            foreach (var b in worldPickers) {
                b.btnNewWorld.Dispose();
                b.btnLaunchWorld.Dispose();
                b.btnDeleteWorld.Dispose();
            }
            btn_Back_Title.Dispose();
            txt_Main_Title.Dispose();
            txt_CreditsInfo.Dispose();

        }
        #endregion  
    }
}