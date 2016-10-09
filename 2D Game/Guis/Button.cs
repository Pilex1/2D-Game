using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenGL;
using Game.Util;
using Game.Fonts;
using Game.Core;

namespace Game.TitleScreen {

    class Button : IDisposable {

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        internal bool active;
        internal Action OnPress;
        internal CooldownTimer cooldown;

        private static Vector2[] vertices = new Vector2[] {
            new Vector2(-1,1),
            new Vector2(-1,-1),
            new Vector2(1,-1),
            new Vector2(1,1)
        };

        private static Vector2[] uvs = new Vector2[] {
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1, 1)
        };
        private static int[] elements = new int[] {
            0,1,2,0,3
        };

        private static Texture texture;
        private static Texture GetTexture() {
            if (texture == null) {
                texture = new Texture("Guis/Button.png");
                Gl.BindTexture(TextureTarget.Texture2D, texture.TextureID);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                Gl.BindTexture(TextureTarget.Texture2D, 0);
            }
            return texture;
        }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize, Action OnPress) : this(pos, size, textstring, font, textsize, OnPress, 20) { }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, Action OnPress) : this(pos, size, textstring, font, 0.5f, OnPress, 20) { }

        public Button(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize, Action OnPress, float cooldowntime) {
            this.pos = pos;
            this.OnPress = OnPress;
            this.text = new Text(textstring, font, textsize, new Vector2(pos.x, pos.y + 0.05), 0.5f);

            GuiVAO vao = new GuiVAO(vertices, elements, uvs);
            model = new GuiModel(vao, GetTexture(), BeginMode.TriangleStrip, size);

            cooldown = new CooldownTimer(cooldowntime);
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

        internal void Update() {
            float x = (2.0f * Input.MouseX) / Program.Width - 1.0f;
            float y = 1.0f - (2.0f * Input.MouseY + 24) / Program.Height; //for some reason this offset corrects the mouse position
            if (x >= pos.x - model.size.x && x <= pos.x + model.size.x && y >= pos.y - model.size.y * Program.AspectRatio && y <= pos.y + model.size.y * Program.AspectRatio) {
                active = true;
                if (Input.Mouse[Input.MouseLeft] && cooldown.Ready()) {
                    OnPress();
                    cooldown.Reset();
                }
            } else
                active = false;
        }

        public void Dispose() {
            model.Dispose();
        }
    }
}
