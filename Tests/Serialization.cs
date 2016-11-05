using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tests {
    static class Serialization {

        private static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Plexico\\Mandelbrot Renderer Double Precision\\";
        private const string file = "settings.plex";

        [Serializable]
        public class FractalPair {
            public Fractal mandelbrot, julia;
            public Fractal.FractalType activeFractal;
            public FractalPair(Fractal mandelbrot, Fractal julia, Fractal.FractalType activeFractal) {
                this.mandelbrot = mandelbrot;
                this.julia = julia;
                this.activeFractal = activeFractal;
            }
        }
        public static FractalPair Load() {
            using (BufferedStream stream = new BufferedStream(new FileStream(dir + file, FileMode.Open, FileAccess.Read, FileShare.None))) {
                IFormatter formatter = new BinaryFormatter();
                object data = formatter.Deserialize(stream);
                return (FractalPair)data;
            }
        }

        public static void Save(Fractal mandelbrot, Fractal julia, Fractal.FractalType fractaltype) {
            FractalPair pair = new FractalPair(mandelbrot, julia, fractaltype);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            using (BufferedStream stream = new BufferedStream(new FileStream(dir + file, FileMode.Create, FileAccess.Write, FileShare.None))) {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, pair);
            }
        }
    }
}
