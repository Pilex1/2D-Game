using System;
using Pencil.Gaming.MathUtils;
using Game.Util;
using Game.Fonts;
using Game.Core;
using System.Diagnostics;
using Pencil.Gaming;

namespace Game.TitleScreen {

    class Button {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal Action OnPress;
        internal CooldownTimer cooldown;
        internal Vector4 colour;

        private bool hoveredover = false;

        public bool disabled;

        public Button(Vector2 pos, Vector2 size, string textstring, TextStyle style, Action OnPress) {
            Debug.Assert(style.font.fontTexture.TextureID != 0);
            this.pos = pos;
            this.OnPress = OnPress;
            text = new Text(textstring, style, new Vector2(pos.x, pos.y + 0.02f));

            colour = new Vector4(1, 1, 1, 1);
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
                disabled ? new Vector4(0.25f, 0.25f, 0.25f, 1) :

                //hovered over
                hoveredover ? new Vector4(0.75f, 0.75f, 0.75f, 1) :

                //active
                new Vector4(1, 1, 1, 1);
        }

        public override string ToString() {
            return text.ToString();
        }
    }
}
