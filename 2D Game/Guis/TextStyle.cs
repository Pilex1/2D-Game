using Game.Main.Util;
using Pencil.Gaming.MathUtils;

namespace Game.Guis {

    enum TextAlignment {
        TopLeft, Top, TopRight, Left, Center, Right, BottomLeft, Bottom, BottomRight
    }

    struct TextStyle {
        public TextAlignment alignment;
        public TextFont font;
        public float size;
        public float maxwidth;
        public int maxlines;
        public float linespacing;
        public ColourRGBA colour;

        public TextStyle(TextAlignment alignment, TextFont font, float size, float maxwidth, int maxlines, float linespacing, ColourRGBA colour) {
            this.alignment = alignment;
            this.font = font;
            this.maxwidth = maxwidth * font.fontTexture.Size.Width / size;
            this.size = size / font.fontTexture.Size.Width;
            this.maxlines = maxlines;
            this.colour = colour;
            this.linespacing = linespacing;
        }

        #region Constants
        public static readonly TextStyle Chiller_SingleLine_Large = new TextStyle(TextAlignment.Center, TextFont.Chiller, 1f, 2f, 1, 1f, new ColourRGBA(255, 255, 255));
        public static readonly TextStyle Chiller_SingleLine_Small = new TextStyle(TextAlignment.Center, TextFont.Chiller, 0.6f, 2f, 1, 1f, new ColourRGBA(255, 255, 255));
        public static readonly TextStyle Chiller_MultiLine_Large = new TextStyle(TextAlignment.Center, TextFont.Chiller, 1f, 2f, 1 << 30, 1f, new ColourRGBA(255, 255, 255));
        public static readonly TextStyle Chiller_MultiLine_Small = new TextStyle(TextAlignment.Center, TextFont.Chiller, 0.6f, 2f, 1 << 30, 1f, new ColourRGBA(255, 255, 255));

        public static readonly TextStyle LucidaConsole_SingleLine_Large = new TextStyle(TextAlignment.Center, TextFont.LucidaConsole, 1f, 2f, 1, 1f, new ColourRGBA(255, 255, 255));
        public static readonly TextStyle LucidaConsole_SingleLine_Small = new TextStyle(TextAlignment.Center, TextFont.LucidaConsole, 0.5f, 2f, 1, 1f, new ColourRGBA(255, 255, 255));
        public static readonly TextStyle LucidaConsole_MultiLine_Large = new TextStyle(TextAlignment.Center, TextFont.LucidaConsole, 1f, 2f, 1 << 30, 1f, new ColourRGBA(255, 255, 255));
        public static readonly TextStyle LucidaConsole_MultiLine_Small = new TextStyle(TextAlignment.Center, TextFont.LucidaConsole, 0.5f, 2f, 1 << 30, 1f, new ColourRGBA(255, 255, 255));
        #endregion

    }
}
