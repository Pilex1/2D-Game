using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Main;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;

namespace Game.Guis {
    class Textbox {

        internal Vector2 size;
        internal Vector2 pos;
        internal Vector4 colour;
        internal bool hoveredover = false;
        internal bool disabled = true;

        internal GuiModel model;
        internal Text text;

        internal CooldownTimer cooldown;
        internal CooldownTimer textcooldown;

        public Textbox(Vector2 pos, Vector2 size, TextFont font, float textsize) {
            Vector2 textpos = pos * new Vector2(1, 1);
            textpos.y += 2 * size.y + 0.01f;
            textpos.x += 0.035f;
            textpos.x -= size.x;
            TextStyle style = new TextStyle(TextAlignment.TopLeft, font, textsize, size.x * 2 - 0.07f, 1, 1f, new Vector3(0.5f, 0f, 1f));
            text = new Text("_", style, textpos);
            this.pos = pos;
            this.size = size;
            model = GenModel();
            cooldown = new CooldownTimer(20);
            textcooldown = new CooldownTimer(3);
        }

        protected virtual GuiModel GenModel() {
            return GuiModel.CreateRectangle(size, Textures.TextboxTex);
        }

        public void ClearText() {
            text.ClearText();
        }

        public void SetText(string txt) {
            text.SetText(txt + "_");
        }

        public string GetText() {
            string s = text.ToString();
            if (s.EndsWith("_"))
                s = s.Remove(s.Length - 1);
            return s;
        }

        public virtual void Update() {

            float x = Input.NDCMouseX;
            float y = Input.NDCMouseY;
            if (x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                hoveredover = true;
                if (Input.Mouse[Input.MouseLeft] && cooldown.Ready()) {
                    disabled = !disabled;
                    cooldown.Reset();
                }
            } else {
                if (Input.Mouse[Input.MouseLeft] && cooldown.Ready()) {
                    disabled = true;
                    cooldown.Reset();
                }
                hoveredover = false;
            }


            text.style.colour = colour =
            !disabled ? new Vector4(1, 1, 1, 1) :
            hoveredover ? new Vector4(0.75, 0.75, 0.75, 1) : new Vector4(0.5, 0.5, 0.5, 1);

            if (disabled) {
                if (text.ToString().EndsWith("_")) {
                    text.RemoveLastCharacter();
                }
                return;
            }

            if (!text.ToString().EndsWith("_"))
                text.AppendCharacter('_');

            if (!textcooldown.Ready())
                return;
            textcooldown.Reset();

            bool[] keys = Input.KeysTyped;
            List<char> keysPressed = new List<char>();

            if (keys['\b'])
                if (text.Length() <= 1)
                    text.SetText("_");
                else
                    text.SetText(text.ToString().Substring(0, text.Length() - 2) + "_");
            for (int i = 0; i < keys.Length; i++) {
                char c = (char)i;
                if (keys[i] && (StringUtil.IsDigit(c) || StringUtil.IsLetter(c) || StringUtil.IsStdKeySymbol(c) || c == ' ')) {
                    text.InsertCharacter(text.Length() - 1, c);
                }
            }



        }

        public override string ToString() {
            return text.ToString().TrimEnd(new char[] { '_' });
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Dispose() {
            model.DisposeAll();
            text.Dispose();
        }
    }

    class InGameTextbox : Textbox {

        public InGameTextbox(Vector2 pos, Vector2 size, TextFont font, float textsize) : base(pos, size, font, textsize) {

        }

        protected override GuiModel GenModel() {
            return GuiModel.CreateRectangle(size, TextureUtil.CreateTexture(0.2f, 0.2f, 0.2f, 0.8f));
        }

        public void Execute() {
            var s = ToString();
            try {
                object obj = CommandParser.Execute(s);
                if (obj != null) {
                    Console.WriteLine(CommandParser.Execute(s).ToString());
                }
                Console.WriteLine("Succesfully executed command: \"" + s + "\"");
            } catch (Exception e) {
                Console.WriteLine("Error while parsing: \"" + s + "\"");
                Console.WriteLine(e);
            }
            ClearText();
        }

        public override void Update() {
            if (!text.ToString().EndsWith("_"))
                text.AppendCharacter('_');

            if (!textcooldown.Ready())
                return;
            textcooldown.Reset();

            bool[] keys = Input.KeysTyped;
            List<char> keysPressed = new List<char>();

            text.style.colour = disabled ? new Vector4(0, 0, 0, 0) : new Vector4(1, 1, 1, 1);
            colour = disabled ? new Vector4(0, 0, 0, 0) : new Vector4(0.2, 0.2, 0.2, 0.8);
            if (disabled) return;

            if (keys['\b'])
                if (text.Length() <= 1)
                    text.SetText("_");
                else
                    text.SetText(text.ToString().Substring(0, text.Length() - 2) + "_");
            for (int i = 0; i < keys.Length; i++) {
                char c = (char)i;
                if (keys[i]) {
                    if ((StringUtil.IsDigit(c) || StringUtil.IsLetter(c) || StringUtil.IsStdKeySymbol(c) || c == ' ')) {
                        text.InsertCharacter(text.Length() - 1, c);
                    }
                }
            }
        }

    }
}
