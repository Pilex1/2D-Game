using Game.Core;
using Game.Fluids;
using Game.Terrains.Gen;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Main {
    static class CommandParser {
        public static void Execute(string s) {
            if (s == "") return;
            var arr = s.Split(new char[] { ' ' }, 2);
            switch (arr[0]) {
                case "clear_fluids":
                    FluidManager.ClearUpdates();
                    break;
                case "teleport":
                    Vector2 pos = ParseVec2(arr[1]);
                    Player.Instance.Teleport(pos);
                    break;
                case "seed":
                    Console.WriteLine(TerrainGen.seed);
                    break;
                default:
                    throw new ArgumentException("Unknown command: \"" + arr[0] + "\"");
            }
        }

        private static Vector2 ParseVec2(string s) {
            var arr = s.Split(' ');
            if (arr.Length != 2) throw new ArgumentException("Incorrect length: " + arr.Length + " - Required: 2");

            string s1 = arr[0], s2 = arr[1];
            float x = 0;
            float y = 0;
            if (s1.StartsWith("r")) {
                x = Player.Instance.data.pos.x;
                s1 = s1.Substring(1);
            }
            x += float.Parse(s1);

            if (s2.StartsWith("r")) {
                y = Player.Instance.data.pos.y;
                s2 = s2.Substring(1);
            }
            y += float.Parse(s2);

            return new Vector2(x, y);
        }
    }
}
