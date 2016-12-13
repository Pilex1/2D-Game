using System;
using OpenGL;
using Game.Util;
using Game.Fonts;
using Game.Core;
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

        public bool disabled;

        public Button(Vector2 pos, Vector2 size, string textstring, TextStyle style, Action OnPress) {
            Debug.Assert(style.font.fontTexture.TextureID != 0);
            this.pos = pos;
            this.OnPress = OnPress;
            this.text = new Text(textstring, style, new Vector2(pos.x, pos.y + 0.05));

            colour = new Vector3(1, 1, 1);
            model = GuiModel.CreateRectangle(size, Assets.Textures.ButtonTex);
            cooldown = new CooldownTimer(20);
        }

        public void SetText(string str) {
            text.UpdateText(str);
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Update() {

            float x = Input.NDCMouseX, y = Input.NDCMouseY;
            if (!disabled && x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                hoveredover = true;
                if (Input.Mouse[Input.MouseLeft] && cooldown.Ready()) {
                    OnPress();
                    cooldown.Reset();
                }
            } else
                hoveredover = false;

            text.style.colour = colour = 
                //disabled
                disabled ? new Vector3(0.5, 0, 0.25) : 

                //hovered over
                hoveredover ? new Vector3(0.5, 0, 0.75) : 

                //active
                new Vector3(0.5, 0, 1);
        }

        public void Dispose() {
            model.DisposeVao();
        }

        public override string ToString() {
            return text.ToString();
        }
    }
}
