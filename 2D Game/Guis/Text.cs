using Game.Core;
using Game.Guis;
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


  

    class Text {
        internal GuiModel model;

        internal TextFont font { get; private set; }
        internal Vector2 pos;
        internal float maxwidth;
        private StringBuilder sb_text;
        internal float size;

        public Text(string text, TextFont font, float size, Vector2 pos, float maxwidth) {
            sb_text = new StringBuilder();
            sb_text.Append(text);
            this.font = font;
            this.maxwidth = maxwidth * font.fontTexture.Size.Width / size;
            this.pos = pos;
            this.size = size / font.fontTexture.Size.Width;
            InitModel();
        }

        private void InitModel() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            SetTextHelper(font.fontTexture.Size.Width, this.size, font.fontTexture.Size.Height, out vertices, out elements, out uvs);
            GuiVAO vao = new GuiVAO(vertices, elements, uvs);
            model = new GuiModel(vao, font.fontTexture, BeginMode.Triangles, new Vector2(size, size));
        }

        private void SetTextHelper(int texwidth, float size, int texheight, out Vector2[] vertices, out int[] elements, out Vector2[] uvs) {
            string s = sb_text.ToString();
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

        private void UpdateText() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            SetTextHelper(font.fontTexture.Size.Width, size, font.fontTexture.Size.Height, out vertices, out elements, out uvs);
            model.vao.UpdateAll(vertices, elements, uvs);

        }

        public void SetText(string s) {
            sb_text.Clear();
            sb_text.Append(s);
            UpdateText();
        }

        public void AppendCharacter(char c) {
            sb_text.Append(c);
            UpdateText();
        }

        public void RemoveCharacter() {
            sb_text.Remove(sb_text.Length - 1, 1);
            UpdateText();
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
