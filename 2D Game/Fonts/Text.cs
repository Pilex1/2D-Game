using Game.Core;
using Game.TitleScreen;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game.Fonts {

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
            StreamReader reader = new StreamReader("Fonts/" + fontName + ".fnt");
            fontTexture = new Texture("Fonts/" + fontName + ".png");
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

    class Text {
        public GuiModel model;

        public TextFont font { get; }
        public float size { get; }
        public Vector2 pos;
        private float maxwidth;

        public Text(string text, TextFont font, float size, Vector2 pos, float maxwidth) {
            this.font = font;
            this.maxwidth = maxwidth * font.fontTexture.Size.Width / size;
            this.pos = pos;
            size /= font.fontTexture.Size.Width;
            this.size = size;
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            SetTextHelper(text, font.fontTexture.Size.Width, font.fontTexture.Size.Height, out vertices, out elements, out uvs);
            GuiVAO vao = new GuiVAO(vertices, elements, uvs);
            model = new GuiModel(vao, font.fontTexture, BeginMode.Triangles);
        }

        private void SetTextHelper(string s, int texwidth, int texheight, out Vector2[] vertices, out int[] elements, out Vector2[] uvs) {
            vertices = new Vector2[s.Length * 4];
            uvs = new Vector2[s.Length * 4];

            string[] words = s.Split(' ');

            int vptr = 0;
            int xptr = 0;
            int lineheight = font.lineHeight;
            int line = 0;

            float actualmaxwidth = 0;

            for (int i = 0; i < words.Length; i++) {
                string word = words[i];
                int textwidth = TextWidth(word);

                //if it will overflow
                if (xptr + textwidth > maxwidth) {
                    //new line
                    if (xptr > actualmaxwidth) {
                        actualmaxwidth = xptr;
                    }
                    xptr = 0;
                    line++;
                }

                for (int j = 0; j < word.Length; j++) {

                    char c = word[j];
                    CharacterInfo info = font.charSet[c];

                    //topleft, bottomleft, bottomright, topright
                    vertices[vptr] = new Vector2(xptr + info.xoffset, info.yoffset - line * lineheight);
                    vertices[vptr + 1] = new Vector2(xptr + info.xoffset, -info.yoffset - info.height - line * lineheight);
                    vertices[vptr + 2] = new Vector2(xptr + info.xoffset + info.width, -info.yoffset - info.height - line * lineheight);
                    vertices[vptr + 3] = new Vector2(xptr + info.xoffset + info.width, info.yoffset - line * lineheight);
                    xptr += info.xadvance;

                    uvs[vptr] = new Vector2(info.x, info.y);
                    uvs[vptr + 1] = new Vector2(info.x, info.y + info.height);
                    uvs[vptr + 2] = new Vector2(info.x + info.width, info.y + info.height);
                    uvs[vptr + 3] = new Vector2(info.x + info.width, info.y);

                    vptr += 4;
                }
                xptr += font.charSet[' '].xadvance;
            }
            if (line == 0)
                actualmaxwidth = xptr;

            pos.x -= actualmaxwidth / 2 * size;

            for (int i = 0; i < uvs.Length; i++) {
                uvs[i].x /= texwidth;
                uvs[i].y /= texheight;
                uvs[i].y = 1 - uvs[i].y;
            }

            elements = new int[s.Length * 6];
            for (int i = 0; i < s.Length; i++) {
                elements[6 * i] = 4 * i;
                elements[6 * i + 1] = 4 * i + 1;
                elements[6 * i + 2] = 4 * i + 2;
                elements[6 * i + 3] = 4 * i;
                elements[6 * i + 4] = 4 * i + 2;
                elements[6 * i + 5] = 4 * i + 3;
            }

        }

        private int TextWidth(string s) {
            if (s.Contains(' ')) throw new ArgumentException("Cannot contain spaces");
            int xptr = 0;
            foreach (char c in s) {
                CharacterInfo info = font.charSet[c];
                xptr += info.xadvance;
            }
            return xptr;
        }

        internal void Dispose() {
            model.Dispose();
            model = null;
        }
    }
}
