using Game.Assets;
using Game.Core;
using Game.Fonts;
using Pencil.Gaming.MathUtils;

namespace Game.Guis {
    class Label {

        internal Vector2 pos;
        internal Text text;
        internal GuiModel model;

        public Label(Vector2 pos, Vector2 size, string textstring, TextStyle style) {
            this.pos = pos;
            text = new Text(textstring, style, new Vector2(pos.x, pos.y + 0.05f));
            model = GuiModel.CreateRectangle(size, Textures.LabelTex);
        }
    }
}
