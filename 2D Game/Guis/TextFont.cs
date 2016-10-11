using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        internal string fontName;
        internal Texture fontTexture;

        internal static HashSet<TextFont> Fonts = new HashSet<TextFont>();

        public static TextFont Chiller;

        internal static void Init() {
            Chiller = new TextFont("Chiller");
        }

        private TextFont(string fontName) {
            this.fontName = fontName;
            StreamReader reader = new StreamReader("Guis/" + fontName + ".fnt");
            fontTexture = new Texture("Guis/" + fontName + ".png");
            Regex regex = new Regex(@"^char id=(-?\d+)\D+x=(-?\d+)\D+y=(-?\d+)\D+width=(-?\d+)\D+height=(-?\d+)\D+xoffset=(-?\d+)\D+yoffset=(-?\d+)\D+xadvance=(-?\d+)");
            string s;
            s = reader.ReadLine();
            s = reader.ReadLine();
            Regex reglineheight = new Regex(@"common lineHeight=(\d+)");
            Match matchlineheight = reglineheight.Match(s);
            if (matchlineheight.Success) {
                lineHeight = int.Parse(matchlineheight.Groups[1].Value);
            } else {
                throw new ArgumentException("Could not find common line height");
            }
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

            Fonts.Add(this);
        }
    }
}
