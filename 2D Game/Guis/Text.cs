using Game.Core;
using Game.Guis;
using Game.Main.GLConstructs;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Game.Fonts {


    class Text {
        internal GuiModel model;

        internal TextStyle style;

        private Vector2 relpos;
        internal Vector2 pos;

        private StringBuilder sb_text;

        public Text(string text, TextStyle style, Vector2 pos) {
            sb_text = new StringBuilder();
            sb_text.Append(text);

            this.style = style;
            relpos = pos;
            InitModel();
        }

        private void InitModel() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            SetTextHelper(out vertices, out elements, out uvs);
            GuiVAO vao = new GuiVAO(vertices, elements, uvs);
            model = new GuiModel(vao, style.font.fontTexture, BeginMode.Triangles, new Vector2(style.size, style.size));
        }

        public void SetPos(Vector2 pos) {
            relpos = pos;
            UpdateModel();
        }

        private void SetTextHelper(out Vector2[] vertices, out int[] elements, out Vector2[] uvs) {

            int texwidth = style.font.fontTexture.Size.Width;
            float size = style.size;
            int texheight = style.font.fontTexture.Size.Height;

            string s = sb_text.ToString();
            vertices = new Vector2[s.Length * 4];
            uvs = new Vector2[s.Length * 4];

            string[] lines = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int vptr = 0;
            int xptr = 0;
            float lineheight = style.font.lineHeight * style.linespacing;
            int line = 0;

            float actualmaxwidth = 0;

            for (int k = 0; k < lines.Length; k++) {
                if (k >= style.maxlines) break;
                string[] words = lines[k].Split(' ');
                for (int i = 0; i < words.Length; i++) {
                    string word = words[i];
                    int textwidth = TextWidth(word);

                    //on line overflow
                    if (xptr + textwidth > style.maxwidth && k + 1 < style.maxlines) {
                        //new line
                        if (xptr > actualmaxwidth) {
                            actualmaxwidth = xptr;
                        }
                        xptr = 0;
                        line++;
                    }

                    for (int j = 0; j < word.Length; j++) {

                        //gets the character info of the current character if it exists in the char set, otherwise defaults to whitespace
                        char c = word[j];
                        CharacterInfo info = null;
                        if (style.font.charSet.ContainsKey(c)) {
                            info = style.font.charSet[c];
                        } else {
                            info = style.font.charSet[' '];
                        }

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
                    xptr += style.font.charSet[' '].xadvance;
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


            float left = relpos.x;
            float h_mid = relpos.x - actualmaxwidth * size / 2;
            float right = relpos.x - actualmaxwidth * size;

            float top = relpos.y - lineheight * size;
            float v_mid = relpos.y;

            //not aligning correctly!
            float bottom = relpos.y + 1.1f * line * lineheight * size;

            switch (style.alignment) {
                case TextAlignment.TopLeft:
                    pos = new Vector2(left, top);
                    break;
                case TextAlignment.Top:
                    pos = new Vector2(h_mid, top);
                    break;
                case TextAlignment.TopRight:
                    pos = new Vector2(right, top);
                    break;
                case TextAlignment.Left:
                    pos = new Vector2(left, v_mid);
                    break;
                case TextAlignment.Center:
                    pos = new Vector2(h_mid, v_mid);
                    break;
                case TextAlignment.Right:
                    pos = new Vector2(right, v_mid);
                    break;
                case TextAlignment.BottomLeft:
                    pos = new Vector2(left, bottom);
                    break;
                case TextAlignment.Bottom:
                    pos = new Vector2(h_mid, bottom);
                    break;
                case TextAlignment.BottomRight:
                    pos = new Vector2(right, bottom);
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


        private void UpdateModel() {
            Vector2[] vertices;
            int[] elements;
            Vector2[] uvs;
            SetTextHelper(out vertices, out elements, out uvs);
            model.vao.UpdateAll(vertices, elements, uvs);

        }

        public void ClearText() {
            SetText("");
        }

        /// <summary>
        /// Expensive method - use with caution
        /// </summary>
        /// <param name="s"></param>
        public void SetText(string s) {
            if (ToString() == s) return;
            sb_text.Clear();
            sb_text.Append(s);
            UpdateModel();
        }

        public string GetText() {
            return sb_text.ToString();
        }

        public void InsertCharacter(int index, char c) {
            sb_text.Insert(index, c);
            UpdateModel();
        }

        public void Append(string s) {
            if (s == "") return;
            sb_text.Append(s);
            UpdateModel();
        }

        public void AppendLine(string s) {
            if (s == "") return;
            sb_text.AppendLine(s);
            UpdateModel();
        }

        public void AppendCharacter(char c) {
            sb_text.Append(c);
            UpdateModel();
        }

        public void RemoveLastCharacter() {
            if (sb_text.Length == 0) return;
            sb_text.Remove(sb_text.Length - 1, 1);
            UpdateModel();
        }

        private int TextWidth(string s) {
            Debug.Assert(!s.Contains(' '));
            int xptr = 0;
            foreach (char c in s) {
                CharacterInfo info = null;
                if (style.font.charSet.ContainsKey(c)) {
                    info = style.font.charSet[c];
                } else {
                    info = style.font.charSet[' '];
                }
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
    }
}
