using Game.Assets;
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

        internal Vector2 pos;
        internal Text text;
        internal GuiModel model;

        public Label(Vector2 pos, Vector2 size, string textstring, TextStyle style) {
            this.pos = pos;
            this.text = new Text(textstring, style, new Vector2(pos.x, pos.y + 0.05));
            model = GuiModel.CreateRectangle(size, Textures.LabelTex);
        }

        internal void Dispose() {
            text.Dispose();
            model.DisposeAll();
        }
    }
}
