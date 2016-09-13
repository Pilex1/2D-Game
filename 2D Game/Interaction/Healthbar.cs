using System;
using OpenGL;
using Game.Entities;

namespace Game.Interaction {
    static class Healthbar {

        public static float MaxHealth { get; private set; }
        public static float Health { get; private set; }
        public static int Deaths { get; private set; }
        public static bool Dead { get; private set; }

        public const float BarWidth = 0.75f, BarHeight = 0.075f;
        public static ColouredModel Model;

        public static void Init(float maxHealth) {

            Deaths = 0;
            Dead = false;

            Health = MaxHealth = maxHealth;

            VBO<Vector2> vertices = CalculateVertices();
            VBO<Vector4> colours = new VBO<Vector4>(new Vector4[] { new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 1), });
            VBO<int> elements = new VBO<int>(new int[] {
                0,1,2,3
            }, BufferTarget.ElementArrayBuffer);

            Model = new ColouredModel(vertices, elements, colours, BeginMode.TriangleStrip, PolygonMode.Fill);
        }

        public static void Update() {
            Model.Vertices = CalculateVertices();
        }

        private static VBO<Vector2> CalculateVertices() {
           return new VBO<Vector2>(new Vector2[] {
                new Vector2(0,BarHeight),
                new Vector2(0,0),
                 new Vector2(BarWidth * Health/MaxHealth,BarHeight),
                new Vector2(BarWidth * Health/MaxHealth,0)
            }, Hint: BufferUsageHint.DynamicDraw);
        }

        public static void Revive() {
            Health = MaxHealth;
        }

        public static void Heal(float hp) {
            if (!Dead) {
                Health += hp;
                if (Health > MaxHealth) Health = MaxHealth;
            }
        }

        public static void Damage(float hp) {
            Health -= hp;
            if (Health <= 0) {
                Dead = true;
                Deaths++;
            }
        }
        
    }
}
