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

        public static GuiVAO vao;
        public static Texture texture;

        public static void Init(float maxHealth) {
            Deaths = 0;
            Dead = false;

            Health = MaxHealth = maxHealth;

            texture = TextureUtil.CreateTexture(Color.DarkRed);
            Vector2[] vertices = CalculateVertices();

            var uvs = new Vector2[] {
               new Vector2(0,0),
               new Vector2(0,0),
               new Vector2(0,0),
               new Vector2(0,0)
           };
            var elements = new int[] {
                0,1,2,3
            };

            vao = new GuiVAO(vertices, elements, uvs, verticeshint: BufferUsageHint.DynamicDraw);
        }

        public static void Update() {
            vao.UpdateVertices(CalculateVertices());
        }

        private static Vector2[] CalculateVertices() {
            return new Vector2[] {
                new Vector2(0,BarHeight),
                new Vector2(0,0),
                new Vector2(BarWidth * Health/MaxHealth,BarHeight),
                new Vector2(BarWidth * Health/MaxHealth,0)
            };
        }

        public static void Revive() {
            Health = MaxHealth;
        }

        public static void Heal(float hp) {
            if (!Dead) {
                Health += hp;
                if (Health > MaxHealth) Health = MaxHealth;
            } else {
                //TODO
                Revive();
            }
        }

        public static void Damage(float hp) {
            Health -= hp;
            if (Health <= 0) {
                Dead = true;
                Deaths++;
            }
        }

        public static void Dispose() {
            if (vao != null)
                vao.Dispose();
            if (texture != null)
                texture.Dispose();
        }

    }
}
