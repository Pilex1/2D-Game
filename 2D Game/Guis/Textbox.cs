using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Guis {
    class Textbox {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal bool active = false;
        internal CooldownTimer cooldown;

        public Textbox(Vector2 pos, Vector2 size, TextFont font, float textsize) {
            Vector2 textpos = pos * new Vector2(1, 1);
            textpos.y += 4 * size.y;
            textpos.x += 0.035f;
            textpos.x -= size.x;
            text = new Text("_", font, textsize, textpos, size.x * 2 - 0.07f, 1, TextAlignment.TopLeft);
            this.pos = pos;
            model = GuiModel.CreateRectangle(size, Asset.TextboxTex);
            cooldown = new CooldownTimer(3);
        }

        public void SetText(string txt) {
            text.SetText(txt);
        }

        public virtual void Update() {

            if (!cooldown.Ready())
                return;
            cooldown.Reset();

            float x = Input.NDCMouseX;
            float y = Input.NDCMouseY;
            if (Input.Mouse[Input.MouseLeft]) {
                if (x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                    active = true;
                } else {
                    active = false;
                }
            }
            if (!active) {
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

        public override string ToString() {
            return text.ToString().TrimEnd(new char[] { '_' });
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Dispose() {
            model.Dispose();
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

            if (!active) {
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
