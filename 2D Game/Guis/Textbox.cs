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
        private static Texture texture;

        private static Texture GetTexture() {
            if (texture == null) {
                texture = new Texture("Guis/Textbox.png");
                Gl.BindTexture(TextureTarget.Texture2D, texture.TextureID);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                Gl.BindTexture(TextureTarget.Texture2D, 0);
            }
            return texture;
        }

        internal GuiModel model;
        internal Vector2 pos;
        internal Text text;
        private StringBuilder textstring = new StringBuilder("");
        internal bool active = false;
        internal CooldownTimer cooldown;

        public Textbox(Vector2 pos, Vector2 size, TextFont font, float textsize, float maxwidth) {
            text = new Text(textstring.ToString(), font, textsize, pos, maxwidth);
            this.pos = pos;
            model = GuiModel.CreateRectangle(size, GetTexture());

            cooldown = new CooldownTimer(20);
        }

        public void ResetCooldown() {
            cooldown.Reset();
        }

    }
}
