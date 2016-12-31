using OpenGL;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Game.Guis {

    class CharacterInfo {
        internal int x, y, width, height, xoffset, yoffset, xadvance;
        internal CharacterInfo(int x, int y, int width, int height, int xoffset, int yoffset, int xadvance) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.xoffset = xoffset;
            this.yoffset = yoffset;
            this.xadvance = xadvance;
        }
    }
    class TextFont {

        internal Dictionary<char, CharacterInfo> charSet = new Dictionary<char, CharacterInfo>(255);
        internal int lineHeight;
        internal Texture fontTexture;

        internal static HashSet<TextFont> Fonts = new HashSet<TextFont>();

        public static TextFont Chiller;
        public static TextFont CenturyGothic;
        public static TextFont DialogInput;
        public static TextFont LucidaConsole;

        internal static void Init() {
            Chiller = new TextFont(Assets.Fonts.FontChillerFnt, Assets.Textures.FontChillerTex);
            CenturyGothic = new TextFont(Assets.Fonts.FontCenturyGothicFnt, Assets.Textures.FontCenturyGothicTex);
            DialogInput = new TextFont(Assets.Fonts.FontDialogInputFnt, Assets.Textures.FontDialogInputTex);
            LucidaConsole = new TextFont(Assets.Fonts.FontLucidaConsoleFnt, Assets.Textures.FontLucidaConsoleTex);
        }

        private TextFont(string font, Texture texture) {
            StringReader reader = new StringReader(font);
            fontTexture = texture;
            Regex regex = new Regex(@"^char id=(-?\d+)\D+x=(-?\d+)\D+y=(-?\d+)\D+width=(-?\d+)\D+height=(-?\d+)\D+xoffset=(-?\d+)\D+yoffset=(-?\d+)\D+xadvance=(-?\d+)");
            string s;
            s = reader.ReadLine();
            s = reader.ReadLine();
            while ((s = reader.ReadLine()) != null) {
                Match match = regex.Match(s);
                if (match.Success) {
                    charSet.Add((char)int.Parse(match.Groups[1].Value), new CharacterInfo(
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[3].Value),
                        int.Parse(match.Groups[4].Value),
                        int.Parse(match.Groups[5].Value),
                        int.Parse(match.Groups[6].Value),
                        int.Parse(match.Groups[7].Value),
                        int.Parse(match.Groups[8].Value)
                    ));
                }
            }

            foreach (char c in charSet.Keys) {
                int h = charSet[c].height;
                if (h > lineHeight) lineHeight = h;
            }

            Fonts.Add(this);
        }
    }
}
