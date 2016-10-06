using OpenGL;
using System.Drawing;

namespace Game.Util {
    static class TextureUtil {
        public static Texture CreateTexture(Color[,] colours) {
            Bitmap bmp = new Bitmap(colours.GetLength(0), colours.GetLength(1));
            for (int i = 0; i < colours.GetLength(0); i++) {
                for (int j = 0; j < colours.GetLength(1); j++) {
                    bmp.SetPixel(i, j, colours[i, j]);
                }
            }
            return new Texture(bmp);
        }
        public static Texture CreateTexture(Color colour) {
            return CreateTexture(new Color[,] { { colour } });
        }
        public static Texture CreateTexture(Vector3[,] colours) {
            Bitmap bmp = new Bitmap(colours.GetLength(0), colours.GetLength(1));
            for (int i = 0; i < colours.GetLength(0); i++) {
                for (int j = 0; j < colours.GetLength(1); j++) {
                    bmp.SetPixel(i, j, Color.FromArgb(1, (int)(255 * colours[i, j].x), (int)(255 * colours[i, j].y), (int)(255 * colours[i, j].z)));
                }
            }
            return new Texture(bmp);
        }
        public static Texture CreateTexture(Vector3 colour) {
            return CreateTexture(new Vector3[,] { { colour } });
        }
    }
}
