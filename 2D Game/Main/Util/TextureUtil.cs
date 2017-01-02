using Game.Main.GLConstructs;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Drawing;

namespace Game.Util {
    static class TextureUtil {

        public enum TextureInterp {
            Linear, Nearest
        }

        public static Vector4 ToVec4(Color colour) {
            return new Vector4((float)colour.R / 255, (float)colour.G / 255, (float)colour.B / 255, colour.A);
        }

        public static Texture CreateTexture(string file, TextureInterp interp) {
            Texture tex = new Texture(file);
            SetTextureParams(tex, interp);
            return tex;
        }

        private static void SetTextureParams(Texture tex, TextureInterp interp) {
            switch (interp) {
                case TextureInterp.Linear:
                    SetTextureParamsLinear(tex);
                    break;
                case TextureInterp.Nearest:
                    SetTextureParamsNearest(tex);
                    break;
                default:
                    break;
            }
        }

        private static void SetTextureParamsLinear(Texture tex) {
            GL.BindTexture(tex.TextureTarget, tex.TextureID);
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Linear });
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Linear });
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureWrapS, new int[] { (int)TextureParameterName.ClampToEdge });
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureWrapT, new int[] { (int)TextureParameterName.ClampToEdge });
            GL.BindTexture(tex.TextureTarget, 0);
        }

        private static void SetTextureParamsNearest(Texture tex) {
            GL.BindTexture(tex.TextureTarget, tex.TextureID);
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Nearest });
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Nearest });
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureWrapS, new int[] { (int)TextureParameterName.ClampToEdge });
            GL.TexParameterI(tex.TextureTarget, TextureParameterName.TextureWrapT, new int[] { (int)TextureParameterName.ClampToEdge });
            GL.BindTexture(tex.TextureTarget, 0);
        }

        public static Texture CreateTexture(Color[,] colours, TextureInterp interp) {
            Bitmap bmp = new Bitmap(colours.GetLength(0), colours.GetLength(1));
            for (int i = 0; i < colours.GetLength(0); i++) {
                for (int j = 0; j < colours.GetLength(1); j++) {
                    bmp.SetPixel(i, j, colours[i, j]);
                }
            }
            Texture tex = new Texture(bmp);
            SetTextureParams(tex, interp);
            return tex;
        }
        public static Texture CreateTexture(Vector4[,] vecColours, TextureInterp interp) {
            Color[,] colours = new Color[vecColours.GetLength(0), vecColours.GetLength(1)];
            for (int i = 0; i < vecColours.GetLength(0); i++) {
                for (int j = 0; j < vecColours.GetLength(1); j++) {
                    colours[i, j] = Color.FromArgb((int)(255 * vecColours[i, j].w), (int)(255 * vecColours[i, j].x), (int)(255 * vecColours[i, j].y), (int)(255 * vecColours[i, j].z));
                }
            }
            return CreateTexture(colours, interp);
        }
        public static Texture CreateTexture(Vector3[,] vecColours, TextureInterp interp) {
            Color[,] colours = new Color[vecColours.GetLength(0), vecColours.GetLength(1)];
            for (int i = 0; i < vecColours.GetLength(0); i++) {
                for (int j = 0; j < vecColours.GetLength(1); j++) {
                    colours[i, j] = Color.FromArgb(255, (int)(255 * vecColours[i, j].x), (int)(255 * vecColours[i, j].y), (int)(255 * vecColours[i, j].z));
                }
            }
            return CreateTexture(colours, interp);
        }

        public static Texture CreateTexture(Color colour) {
            return CreateTexture(new Color[,] { { colour } }, TextureInterp.Nearest);
        }
        public static Texture CreateTexture(Vector3 colour) {
            return CreateTexture(new Vector3[,] { { colour } }, TextureInterp.Nearest);
        }
        public static Texture CreateTexture(Vector4 colour) {
            return CreateTexture(new Vector4[,] { { colour } }, TextureInterp.Nearest);
        }
        public static Texture CreateTexture(float x, float y, float z) {
            return CreateTexture(new Vector3[,] { { new Vector3(x, y, z) } }, TextureInterp.Nearest);
        }
        public static Texture CreateTexture(float x, float y, float z, float w) {
            return CreateTexture(new Vector4[,] { { new Vector4(x, y, z, w) } }, TextureInterp.Nearest);
        }

        public static Color ColourFromVec3(Vector3 vec) {
            return Color.FromArgb(255, (int)(vec.x * 255), (int)(vec.y * 255), (int)(vec.z * 255));
        }
        public static Color ColourFromVec3(float x, float y, float z) {
            return ColourFromVec3(new Vector3(x, y, z));
        }
        public static Color ColourFromVec4(Vector4 vec) {
            return Color.FromArgb((int)(vec.w * 255), (int)(vec.x * 255), (int)(vec.y * 255), (int)(vec.z * 255));
        }
        public static Color ColourFromVec4(float x, float y, float z, float w) {
            return ColourFromVec4(new Vector4(x, y, z, w));
        }
        /// <summary>
        /// Converts HSV to RGB colour
        /// </summary>
        /// <param name="h">0 - 360</param>
        /// <param name="s">0 - 1</param>
        /// <param name="v">0 - 1</param>
        /// <returns></returns>
        public static Vector3 HSVToRGB_Vec3(float h, float s, float v) {
            float H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            float R, G, B;
            if (v <= 0) {
                R = G = B = 0;
            } else if (s <= 0) {
                R = G = B = v;
            } else {
                float hf = H / 60.0f;
                int i = (int)Math.Floor(hf);
                float f = hf - i;
                float pv = v * (1 - s);
                float qv = v * (1 - s * f);
                float tv = v * (1 - s * (1 - f));
                switch (i) {
                    case 0:
                        R = v;
                        G = tv;
                        B = pv;
                        break;
                    case 1:
                        R = qv;
                        G = v;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = v;
                        B = tv;
                        break;
                    case 3:
                        R = pv;
                        G = qv;
                        B = v;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = v;
                        break;
                    case 5:
                        R = v;
                        G = pv;
                        B = qv;
                        break;
                    case 6:
                        R = v;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = v;
                        G = pv;
                        B = qv;
                        break;
                    default:
                        R = G = B = v;
                        break;
                }
            }

            return new Vector3(R, G, B);
        }
    }
}
