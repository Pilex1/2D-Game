using OpenGL;
using System.Drawing;

namespace Game.Util {
    static class ColourUtil {
        public static Color ColourFromVec4(Vector4 vec) {
            return Color.FromArgb((int)(vec.w * 255), (int)(vec.x * 255), (int)(vec.y * 255), (int)(vec.z * 255));
        }
        public static Texture TexFromColour(Vector4 vec) {
            return TexFromColour(ColourFromVec4(vec));
        }
        public static Texture TexFromColour(Color colour) {
            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, colour);
            return new Texture(bmp);
        }
    }
}
