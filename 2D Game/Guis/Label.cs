using Game.Core;
using Game.Fonts;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Guis {
    class Label {
        private static Texture texture;

        private static Texture GetTexture() {
            if (texture == null) {
                texture = new Texture("Guis/Label.png");
                Gl.BindTexture(TextureTarget.Texture2D, texture.TextureID);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
                Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                Gl.BindTexture(TextureTarget.Texture2D, 0);
            }
            return texture;
        }

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

        internal Vector2 pos;
        internal Text text;
        internal GuiModel model;

        public Label(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize) {
            this.pos = pos;
            this.text = new Text(textstring, font, textsize, new Vector2(pos.x, pos.y + 0.05), 0.5f);
            model = new GuiModel(new GuiVAO(vertices, elements, uvs), GetTexture(), BeginMode.TriangleStrip, size);
        }
    }
}
