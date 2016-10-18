using Game.Core;
using Game.Guis;
using Game.TitleScreen;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game.Fonts {

    enum TextAlignment {
        CenterCenter, TopLeft
    }


    class Text {
        internal GuiModel model;

        internal Vector3 colour;
        internal TextFont font { get; private set; }
        private Vector2 relpos;
        internal Vector2 pos;
        internal float maxwidth;
        private StringBuilder sb_text;
        private TextAlignment alignment;
        internal float size;
        private int maxlines;

        public Text(string text, TextFont font, float size, Vector2 pos, float maxwidth, int maxlines, TextAlignment alignment) {
            sb_text = new StringBuilder();
            sb_text.Append(text);
            this.alignment = alignment;
            this.font = font;
            this.maxwidth = maxwidth * font.fontTexture.Size.Width / size;
            this.relpos = pos;
            this.size = size / font.fontTexture.Size.Width;
            this.maxlines = maxlines;
            colour = new Vector3(1, 1, 1);
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

            string[] lines = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);



            int vptr = 0;
            int xptr = 0;
            int lineheight = font.lineHeight;
            int line = 0;

            float actualmaxwidth = 0;

            for (int k = 0; k < lines.Length; k++) {
                if (k >= maxlines) break;
                string[] words = lines[k].Split(' ');
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

                //new line
                if (xptr > actualmaxwidth) {
                    actualmaxwidth = xptr;
                }
                xptr = 0;
                line++;
            }




            if (line == 0)
                actualmaxwidth = xptr;

            switch (alignment) {
                case TextAlignment.CenterCenter:
                    pos = new Vector2(relpos.x - actualmaxwidth / 2 * size, relpos.y);
                    break;
                case TextAlignment.TopLeft:
                    pos = new Vector2(relpos.x, relpos.y - font.lineHeight * size);
                    break;
                default:
                    break;
            }


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

        public void InsertCharacter(int index, char c) {
            sb_text.Insert(index, c);
            UpdateText();
        }

        public void AppendCharacter(char c) {
            sb_text.Append(c);
            UpdateText();
        }

        public void RemoveLastCharacter() {
            if (sb_text.Length == 0) return;
            sb_text.Remove(sb_text.Length - 1, 1);
            UpdateText();
        }

        private int TextWidth(string s) {
            Debug.Assert(!s.Contains(' '));
            int xptr = 0;
            foreach (char c in s) {
                CharacterInfo info = font.charSet[c];
                xptr += info.xadvance;
            }
            return xptr;
        }

        public int Length() {
            return sb_text.Length;
        }

        public override string ToString() {
            return sb_text.ToString();
        }

        internal void Dispose() {
            if (model != null)
                model.Dispose();
        }
    }
}
