using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenGL;
using Game.Util;
using Game.Fonts;
using Game.Core;
using Game.Guis;
using System.Diagnostics;

namespace Game.TitleScreen {

    class Button {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal Action OnPress;
        internal CooldownTimer cooldown;
        internal Vector3 colour;

        private bool hoveredover = false;

        private bool _disabled;
        internal bool disabled {
            get { return _disabled; }
            set {
                _disabled = value;
                text.style.colour = _disabled ? new Vector3(0.5, 0.5, 0.5) : new Vector3(1, 1, 1);
                colour = _disabled ? new Vector3(0.5, 0.5, 0.5) : new Vector3(1, 1, 1);
            }
        }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize, Action OnPress) : this(pos, size, textstring, new TextStyle(TextAlignment.CenterCenter, font, textsize, 2f, 1, new Vector3(1, 1, 1)), OnPress, 20) { }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, Action OnPress) : this(pos, size, textstring, new TextStyle(TextAlignment.CenterCenter, font, 1f, 2f, 1, new Vector3(1, 1, 1)), OnPress, 20) { }

        public Button(Vector2 pos, Vector2 size, string textstring, TextStyle style, Action OnPress, float cooldowntime) {
            Debug.Assert(style.font.fontTexture.TextureID != 0);
            this.pos = pos;
            this.OnPress = OnPress;
            this.text = new Text(textstring, style, new Vector2(pos.x, pos.y + 0.05));

            colour = new Vector3(1, 1, 1);
            model = GuiModel.CreateRectangle(size, Assets.Textures.ButtonTex);
            cooldown = new CooldownTimer(cooldowntime);
        }

        public void SetText(string str) {
            text = new Text(str, text.style, text.pos);
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Update() {
            if (disabled)
                return;

            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            if (x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                hoveredover = true;
                Debug.WriteLine("Dfdfd");
                if (Input.Mouse[Input.MouseLeft] && cooldown.Ready()) {
                    OnPress();
                    cooldown.Reset();
                }
            } else
                hoveredover = false;

            colour = hoveredover ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1);
        }

        public void Dispose() {
            model.DisposeVao();
        }

        public override string ToString() {
            return text.ToString();
        }
    }
}
