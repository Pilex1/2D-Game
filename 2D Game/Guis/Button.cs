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
using Game.Assets;

namespace Game.TitleScreen {

    class Button : IDisposable {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal Action OnPress;
        internal CooldownTimer cooldown;
        internal Vector3 colour = new Vector3(1, 1, 1);

        private bool hoveredover = false;

        private bool _disabled;
        internal bool disabled {
            get { return _disabled; }
            set {
                _disabled = value;
                text.colour = _disabled ? new Vector3(0.5, 0.5, 0.5) : new Vector3(1, 1, 1);
                colour = _disabled ? new Vector3(0.5, 0.5, 0.5) : new Vector3(1, 1, 1);
            }
        }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize, Action OnPress) : this(pos, size, textstring, font, textsize, OnPress, 20) { }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, Action OnPress) : this(pos, size, textstring, font, 1.1f, OnPress, 20) { }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize, Action OnPress, float cooldowntime) {
            this.pos = pos;
            this.OnPress = OnPress;
            this.text = new Text(textstring, font, textsize, new Vector2(pos.x, pos.y + 0.05), 0.5f, 1, TextAlignment.CenterCenter);

            model = GuiModel.CreateRectangle(size, Asset.ButtonTex);
            cooldown = new CooldownTimer(cooldowntime);
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Update() {
            if (disabled)
                return;

            float x = (2.0f * Input.MouseX) / Program.Width - 1.0f;
            float y = 1.0f - (2.0f * Input.MouseY + 24) / Program.Height; //for some reason this offset corrects the mouse position
            if (x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                hoveredover = true;
                if (Input.Mouse[Input.MouseLeft] && cooldown.Ready()) {
                    OnPress();
                    cooldown.Reset();
                }
            } else
                hoveredover = false;

            if (disabled) return;
            colour = hoveredover ? new Vector3(1, 0.5, 1) : new Vector3(1, 1, 1);
        }

        public void Dispose() {
            model.Dispose();
        }

        public override string ToString() {
            return text.ToString();
        }
    }
}
