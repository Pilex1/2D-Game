using OpenGL;
using System;
using System.Drawing;

namespace Game.Util {
    static class ColourUtil {
        public static Color ColourFromVec3(Vector3 vec) {
            return Color.FromArgb(1, (int)(vec.x * 255), (int)(vec.y * 255), (int)(vec.z * 255));
        }
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
