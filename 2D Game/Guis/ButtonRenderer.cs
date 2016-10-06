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
        internal GuiVAO vao;
        internal Texture texture;
        internal Vector2 pos;
        internal Vector2 size;
        internal Text text;
        internal bool active;
        internal Action OnPress;

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
            0,1,2,2,3,0
        };

        public Button(Vector2 pos, Vector2 size, string textstring, Font font, Action OnPress) {
            this.pos = pos;
            this.size = size;
            this.OnPress = OnPress;
            this.text = new Text(textstring, font, 0.5f, new Vector2(pos.x, pos.y + 0.05), 0.5f);

            texture = new Texture("Assets/Button.png");
            Gl.BindTexture(TextureTarget.Texture2D, texture.TextureID);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(TextureTarget.Texture2D, 0);

            vao = new GuiVAO(vertices, elements, uvs);

            Gui.AddButton(this);
        }

        internal void Update() {
            float x = (2.0f * Input.MouseX) / Program.Width - 1.0f;
            float y = 1.0f - (2.0f * Input.MouseY + 24) / Program.Height; //for some reason this offset corrects the mouse position
            if (x >= pos.x - size.x && x <= pos.x + size.x && y >= pos.y - size.y * Program.AspectRatio && y <= pos.y + size.y * Program.AspectRatio) {
                active = true;
                if (Input.Mouse[Input.MouseLeft])
                    OnPress();
            } else
                active = false;
        }

        public void Dispose() {
            vao.Dispose();
            texture.Dispose();
        }
    }
}
