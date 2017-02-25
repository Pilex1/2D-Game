using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Main.Util {
    class ColourRGB {

        private float red, green, blue;

        public ColourRGB(float red, float green, float blue) {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public float Red {
            get { return red; }
            set {
                value = MathUtil.Clamp(value, 0, 255);
                red = value;
            }
        }

        public float Green {
            get { return green; }
            set {
                value = MathUtil.Clamp(value, 0, 255);
                green = value;
            }
        }

        public float Blue {
            get { return blue; }
            set {
                value = MathUtil.Clamp(value, 0, 255);
                blue = value;
            }
        }

        public Vector3 ToVec3() => new Vector3(Red / 255, Green / 255, Blue / 255);

        public ColourRGB Copy() => new ColourRGB(Red, Green, Blue);

        public static implicit operator ColourRGB(ColourHSB hsb) {
            int r = 0, g = 0, b = 0;
            if (hsb.Saturation == 0) {
                r = g = b = (int)(hsb.Brightness * 255.0f + 0.5f);
            } else {
                float h = (hsb.Hue - (float)Math.Floor(hsb.Hue)) * 6.0f;
                float f = h - (float)Math.Floor(h);
                float p = hsb.Brightness * (1.0f - hsb.Saturation);
                float q = hsb.Brightness * (1.0f - hsb.Saturation * f);
                float t = hsb.Brightness * (1.0f - (hsb.Saturation * (1.0f - f)));
                switch ((int)h) {
                    case 0:
                        r = (int)(hsb.Brightness * 255.0f + 0.5f);
                        g = (int)(t * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        r = (int)(q * 255.0f + 0.5f);
                        g = (int)(hsb.Brightness * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(hsb.Brightness * 255.0f + 0.5f);
                        b = (int)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(q * 255.0f + 0.5f);
                        b = (int)(hsb.Brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        r = (int)(t * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(hsb.Brightness * 255.0f + 0.5f);
                        break;
                    case 5:
                        r = (int)(hsb.Brightness * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(q * 255.0f + 0.5f);
                        break;
                }
            }
            return new ColourRGB(r, g, b);
        }


    }

    class ColourRGBA : ColourRGB {

        private float alpha;

        public ColourRGBA(float red, float green, float blue, float alpha) : base(red, green, blue) {
            Alpha = alpha;
        }
        public ColourRGBA(float red, float green, float blue) : this(red, green, blue, 1) { }
        public ColourRGBA(ColourRGB rgb, float alpha) : this(rgb.Red, rgb.Green, rgb.Blue, alpha) { }
        public ColourRGBA(ColourRGB rgb) : this(rgb.Red, rgb.Green, rgb.Blue) { }

        public float Alpha {
            get { return alpha; }
            set {
                value = MathUtil.Clamp(value, 0, 1);
                alpha = value;
            }
        }

        public Vector4 ToVec4() => new Vector4(ToVec3(), Alpha);

        public new ColourRGBA Copy() => new ColourRGBA(Red, Green, Blue, Alpha);

        public static implicit operator ColourRGBA(ColourHSBA hsba) => new ColourRGBA(hsba, hsba.Alpha);
    }

}
