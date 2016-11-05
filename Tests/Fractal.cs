using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {

    [Serializable]
    class Fractal {

        public enum Mode {
            Normal, Reciprocal, SquaredReciprocal, t1, t2, t3, t4, t5
        }

        public enum FractalType {
            Mandelbrot, Julia
        }

        public float rot;
        public float maxIter;
        public Color4 clrRatio;

        public Vector2d pos;
        public double zoom;

        public bool crosshair;
        [NonSerialized]
        private CooldownTimer crosshairtimer;

        public Mode mode { get; private set; }
        [NonSerialized]
        private CooldownTimer modeTimer;
        public FractalType fractalType { get; private set; }

        [NonSerialized]
        public Entity quad;

        private Fractal(float rot, int maxIter, Color4 clrRatio, Vector2d pos, double zoom, FractalType fractalType) {
            this.rot = rot;
            this.maxIter = maxIter;
            this.clrRatio = clrRatio;
            this.pos = pos;
            this.zoom = zoom;
            this.fractalType = fractalType;
            this.crosshair = true;
        }

        public void Load() {
            modeTimer = new CooldownTimer(20);
            crosshairtimer = new CooldownTimer(20);
            Model model = Model.CreateRectangle(new Vector2(0.5f, 1));
            switch (fractalType) {
                case FractalType.Mandelbrot:
                    quad = new Entity(model, new Vector2(-0.5f, 0));
                    break;
                case FractalType.Julia:
                    quad = new Entity(model, new Vector2(0.5f, 0));
                    break;
            }
        }

        public void SetMode(Mode mode) {
            if (!modeTimer.Ready()) return;
            modeTimer.Reset();
            this.mode = mode;
        }

        public void ToggleCrosshair() {
            if (!crosshairtimer.Ready()) return;
            crosshairtimer.Reset();
            crosshair = !crosshair;
        }

        public static Fractal CreateMandelbrot() {
            return new Fractal(0, 500, new Color4(-0.0590199828f, 0.07081998f, 0.0890399441f, 1), Vector2d.Zero, 2, FractalType.Mandelbrot);
        }

        public static Fractal CreateJulia() {
            return new Fractal(0, 500, new Color4(-0.0590199828f, 0.07081998f, 0.0890399441f, 1), Vector2d.Zero, 2, FractalType.Julia);
        }

        internal void Reset() {
            pos = Vector2d.Zero;
            zoom = 2;
        }
    }
}
