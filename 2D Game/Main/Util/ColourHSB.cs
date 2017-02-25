using Game.Util;
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;
using System.Linq;

namespace Game.Main.Util {
    class ColourHSB {

        public static ColourHSB White => new ColourHSB(0, 0, 1);
        public static ColourHSB Black => new ColourHSB(0, 0, 0);

        public static ColourHSB Red => new ColourHSB(0, 1, 1);
        public static ColourHSB Yellow => new ColourHSB(60, 1, 1);
        public static ColourHSB Green => new ColourHSB(120, 1, 1);
        public static ColourHSB Cyan => new ColourHSB(180, 1, 1);
        public static ColourHSB Blue => new ColourHSB(240, 1, 1);
        public static ColourHSB Magenta => new ColourHSB(300, 1, 1);

        private float hue, saturation, brightness;

        public ColourHSB(float hue, float saturation, float brightness) {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
        }

        public float Hue {
            get { return hue; }
            set {
                value = MathUtil.Clamp(value, 0, 360);
                hue = value;
            }
        }

        public float Saturation {
            get { return saturation; }
            set {
                value = MathUtil.Clamp(value, 0, 1);
                saturation = value;
            }
        }

        public float Brightness {
            get { return brightness; }
            set {
                value = MathUtil.Clamp(value, 0, 1);
                brightness = value;
            }
        }

        public Vector3 ToVec3() => new Vector3(Hue, Saturation, Brightness);

        public ColourHSB Copy() => new ColourHSB(Hue, Saturation, Brightness);

        public static implicit operator ColourHSB(ColourRGB rgb) {
            float r = rgb.Red, g = rgb.Green, b = rgb.Blue;
            float hue, saturation, brightness;
            float cmax = (r > g) ? r : g;
            if (b > cmax) cmax = b;
            float cmin = (r < g) ? r : g;
            if (b < cmin) cmin = b;

            brightness = cmax / 255.0f;
            if (cmax != 0)
                saturation = (cmax - cmin) / cmax;
            else
                saturation = 0;
            if (saturation == 0)
                hue = 0;
            else {
                float redc = (cmax - r) / (cmax - cmin);
                float greenc = (cmax - g) / (cmax - cmin);
                float bluec = (cmax - b) / (cmax - cmin);
                if (r == cmax)
                    hue = bluec - greenc;
                else if (g == cmax)
                    hue = 2.0f + redc - bluec;
                else
                    hue = 4.0f + greenc - redc;
                hue = hue / 6.0f;
                if (hue < 0)
                    hue = hue + 1.0f;
            }
            return new ColourHSB(hue, saturation, brightness);
        }


    }

    class ColourHSBA : ColourHSB {

        private float alpha;

        public ColourHSBA(float hue, float saturation, float brightness, float alpha) : base(hue, saturation, brightness) {
            Alpha = alpha;
        }
        public ColourHSBA(float hue, float saturation, float brightness) : this(hue, saturation, brightness, 1) { }
        public ColourHSBA(ColourHSB hsb, float alpha) : this(hsb.Hue, hsb.Saturation, hsb.Brightness, alpha) { }
        public ColourHSBA(ColourHSB hsb) : this(hsb.Hue, hsb.Saturation, hsb.Brightness) { }

        public float Alpha {
            get { return alpha; }
            set {
                value = MathUtil.Clamp(value, 0, 1);
                alpha = value;
            }
        }

        public Vector4 ToVec4() => new Vector4(ToVec3(), Alpha);

        public new ColourHSBA Copy() => new ColourHSBA(Hue, Saturation, Brightness, Alpha);

        public static implicit operator ColourHSBA(ColourRGBA rgba) => new ColourHSBA(rgba, rgba.Alpha);

    }

    class ColourHSBList {

        private List<float> hues;
        private List<float> saturations;
        private List<float> brightnesses;

        public ColourHSBList(params ColourHSB[] colours) {
            hues = new List<float>();
            saturations = new List<float>();
            brightnesses = new List<float>();
            foreach (ColourHSB c in colours) {
                AddColour(c);
            }
        }

        public void AddColour(ColourHSB colour) {
            hues.Add(colour.Hue);
            saturations.Add(colour.Saturation);
            brightnesses.Add(colour.Brightness);
        }

        public void RemoveColour(ColourHSB colour) {
            hues.Remove(colour.Hue);
            saturations.Remove(colour.Saturation);
            brightnesses.Remove(colour.Brightness);
        }

        public ColourHSB GetAverage() => new ColourHSB(hues.Average(), saturations.Average(), brightnesses.Average());

    }

}
