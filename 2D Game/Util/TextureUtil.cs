using OpenGL;
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
            Gl.BindTexture(tex.TextureTarget, tex.TextureID);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(tex.TextureTarget, 0);
        }

        private static void SetTextureParamsNearest(Texture tex) {
            Gl.BindTexture(tex.TextureTarget, tex.TextureID);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(tex.TextureTarget, 0);
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
        public static Texture CreateTexture(Vector3[,] colours, TextureInterp interp) {
            Bitmap bmp = new Bitmap(colours.GetLength(0), colours.GetLength(1));
            for (int i = 0; i < colours.GetLength(0); i++) {
                for (int j = 0; j < colours.GetLength(1); j++) {
                    bmp.SetPixel(i, j, Color.FromArgb(255, (int)(255 * colours[i, j].x), (int)(255 * colours[i, j].y), (int)(255 * colours[i, j].z)));
                }
            }
            Texture tex = new Texture(bmp);
            SetTextureParams(tex, interp);
            return tex;
        }

        public static Texture CreateTexture(Color colour) {
            return CreateTexture(new Color[,] { { colour } },TextureInterp.Nearest);
        }
        public static Texture CreateTexture(Vector3 colour) {
            return CreateTexture(new Vector3[,] { { colour } },TextureInterp.Nearest);
        }
    }
}
