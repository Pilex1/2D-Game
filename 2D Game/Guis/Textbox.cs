﻿using Game.Assets;
using Game.Core;
using Game.Fonts;
using Game.Main.Util;
using Game.Util;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;

namespace Game.Guis {
    class Textbox {

        public static readonly Vector2 TextOffset = new Vector2(0.035f, 0.01f);

        internal Vector2 size;
        internal Vector2 pos;
        internal ColourRGBA colour = new ColourRGBA(255, 255, 255, 1);
        internal bool hoveredover = false;
        private bool _disabled;
        internal bool disabled {
            get { return _disabled; }
            set {
                if (_disabled == value) return;
                if (value == false) {
                    Input.CharsTyped.Clear();
                }
                _disabled = value;
            }
        }

        internal GuiModel model;
        internal Text text;

        internal CooldownTimer cooldown;
        internal CooldownTimer textcooldown;

        public Textbox(Vector2 pos, Vector2 size, TextFont font, float textsize) {
            Vector2 textpos = pos;
            textpos.y += 2 * size.y + TextOffset.y;
            textpos.x += TextOffset.x;
            textpos.x -= size.x;
            TextStyle style = new TextStyle(TextAlignment.TopLeft, font, textsize, size.x * 2 - 0.07f, 1, 1f, new ColourRGBA(127, 0, 255));
            text = new Text("_", style, textpos);
            this.pos = pos;
            this.size = size;
            model = GenModel();
            cooldown = new CooldownTimer(20);
            textcooldown = new CooldownTimer(3);
            disabled = true;
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
                if (Input.MouseDown(MouseButton.LeftButton) && cooldown.Ready()) {
                    disabled = !disabled;
                    cooldown.Reset();
                }
            } else {
                if (Input.MouseDown(MouseButton.LeftButton) && cooldown.Ready()) {
                    disabled = true;
                    cooldown.Reset();
                }
                hoveredover = false;
            }


            text.style.colour = colour =
            !disabled ? new ColourRGBA(255,255,255, 1) :
            hoveredover ? new ColourRGBA(191, 191, 191, 1) : new ColourRGBA(127, 127, 127, 1);

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

            HandleInputs();
        }

        protected virtual void HandleInputs() {
            bool b = false;
            while (Input.CharsTyped.Count > 0) {
                char c = Input.CharsTyped.Dequeue();
                if (c == '\b') {
                    if (!b) {
                        b = true;
                        if (text.Length() <= 1) {
                            text.SetText("_");
                        } else {
                            text.SetText(text.ToString().Substring(0, text.Length() - 2) + "_");
                        }
                    }
                } else {
                    if (StringUtil.IsDigit(c) || StringUtil.IsLetter(c) || StringUtil.ContainsStdSymbol(c) || c == ' ') {
                        text.InsertCharacter(text.Length() - 1, c);
                    }
                }
            }
        }

        public override string ToString() {
            return text.ToString().TrimEnd(new char[] { '_' });
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }
    }


}
