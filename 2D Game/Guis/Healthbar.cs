using Game.Core;
using Game.Entities;
using Game.Util;
using OpenGL;
using System.Drawing;

namespace Game.Interaction {
    static class Healthbar {

        public static float MaxHealth { get; private set; }
        public static float Health { get; private set; }
        public static int Deaths { get; private set; }
        public static bool Dead { get; private set; }

        public const float BarWidth = 0.75f, BarHeight = 0.075f;

        public static GuiModel Bar;

        public static void Init(float maxHealth) {
            Deaths = 0;
            Dead = false;
            Health = MaxHealth = maxHealth;

            Bar = GuiModel.CreateRectangle(new Vector2(BarWidth, BarHeight), Color.DarkRed);
        }

        public static void Revive() {
            Health = MaxHealth;
            Dead = false;
        }

        private static void Update() {
            Bar.size.x = Health / MaxHealth * BarWidth;
        }

        public static void Heal(float hp) {
            if (!Dead) {
                Health += hp;
                if (Health > MaxHealth) Health = MaxHealth;
            } else {
                //TODO
                Revive();
            }
            Update();
        }

        public static void Damage(float hp) {
            Health -= hp;
            if (Health <= 0) {
                Dead = true;
                Deaths++;
            }
            Update();
        }

        public static void Dispose() {
            Bar?.Dispose();
        }

    }
}
