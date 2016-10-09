using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    class Program {
        static void Main(string[] args) {
            float[] numbers= new float[] {
                10, 2, 1, 0.5f, -0.5f -1, -2, -10
            };
            for (int i = 0; i < numbers.Length; i++) {
                numbers[i] = Sin(numbers[i]);
            }
            foreach (var i in numbers) {
                Console.WriteLine(i);
            }
            Console.WriteLine(Sin(-0.5f));

            Console.ReadLine();
        }

        static float Sin(float x) {
            x = Mod(x, 2 * (float)Math.PI);
            return x;
        }
        public static float Mod(float a, float b) {
            return a - b * (float)Math.Floor(a / b);
        }
    }
}
