using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Util;
using OpenGL;
using System.Collections.Generic;

namespace Game.Guis {
    class Textbox {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal bool disabled = true;
        internal CooldownTimer cooldown;
        internal CooldownTimer textcooldown;
        internal Vector3 colour;
        internal bool hoveredover = false;

        public Textbox(Vector2 pos, Vector2 size, TextFont font, float textsize) {
            Vector2 textpos = pos * new Vector2(1, 1);
            textpos.y += 4 * size.y;
            textpos.x += 0.035f;
            textpos.x -= size.x;
            TextStyle style = new TextStyle(TextAlignment.TopLeft, font, textsize, size.x * 2 - 0.07f, 1, 1f, new Vector3(0.5f, 0f, 1f));
            text = new Text("_", style, textpos);
            this.pos = pos;
            model = GuiModel.CreateRectangle(size, Textures.TextboxTex);
            cooldown = new CooldownTimer(20);
            textcooldown = new CooldownTimer(3);
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


            text.style.colour = colour = (hoveredover && disabled) ? new Vector3(0.75, 0, 0.75) : disabled ? new Vector3(0.5, 0, 0.5) : new Vector3(0.5, 0, 1);

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
                if (keys[i] && StringUtil.IsAlphaNumericSpace(c)) {
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

    class RestrictedTextbox : Textbox {
        public RestrictedTextbox(Vector2 pos, Vector2 size, TextFont font, float textsize) : base(pos, size, font, textsize) {
        }

        public override void Update() {
            if (!cooldown.Ready())
                return;
            cooldown.Reset();

            if (disabled) {
                if (text.ToString().EndsWith("_")) {
                    text.RemoveLastCharacter();
                }
                return;
            }

            if (!text.ToString().EndsWith("_"))
                text.AppendCharacter('_');

            bool[] keys = Input.KeysTyped;
            List<char> keysPressed = new List<char>();

            if (keys['\b'])
                if (text.Length() <= 1)
                    text.SetText("_");
                else
                    text.SetText(text.ToString().Substring(0, text.Length() - 2) + "_");
            for (int i = 0; i < keys.Length; i++) {
                char c = (char)i;
                if (keys[i] && StringUtil.IsAlphaNumericSpace(c)) {
                    text.InsertCharacter(text.Length() - 1, c);
                }
            }

        }
    }
}
