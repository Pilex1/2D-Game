using System;
using Pencil.Gaming.MathUtils;
using Game.Util;
using Game.Fonts;
using Game.Core;
using Pencil.Gaming;
using Game.Main.Util;

namespace Game.Guis {

    class Button {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal Action OnPress;
        internal CooldownTimer cooldown;
        internal ColourRGBA colour;

        private bool hoveredover = false;

        public bool disabled;

        public Button(Vector2 pos, Vector2 size, string textstring, TextStyle style, Action OnPress) {
            this.pos = pos;
            this.OnPress = OnPress;
            text = new Text(textstring, style, new Vector2(pos.x, pos.y + 0.02f));

            colour = new ColourRGBA(255,255,255, 1);
            model = GuiModel.CreateRectangle(size, Assets.Textures.ButtonTex);
            cooldown = new CooldownTimer(20);
        }

        public void SetText(string str) {
            text.SetText(str);
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Update() {

            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            if (!disabled && x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                hoveredover = true;
                if (Input.MouseDown(MouseButton.LeftButton) && cooldown.Ready()) {
                    OnPress();
                    cooldown.Reset();
                }
            } else
                hoveredover = false;

            text.style.colour = colour =
                //disabled
                disabled ? new ColourRGBA(63, 63, 63, 1) :

                //hovered over
                hoveredover ? new ColourRGBA(191,191,191, 1) :

                //active
                new ColourRGBA(255,255,255, 1);
        }

        public override string ToString() {
            return text.ToString();
        }
    }
}
