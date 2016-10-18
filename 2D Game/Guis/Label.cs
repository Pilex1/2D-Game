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

        public Label(Vector2 pos, Vector2 size, string textstring, TextFont font, float textsize) {
            this.pos = pos;
            this.text = new Text(textstring, font, textsize, new Vector2(pos.x, pos.y + 0.05), 0.5f,1, TextAlignment.CenterCenter);
            model = GuiModel.CreateRectangle(size, Asset.LabelTex);
        }

        internal void Dispose() {
            text.Dispose();
            model.Dispose();
        }
    }
}
