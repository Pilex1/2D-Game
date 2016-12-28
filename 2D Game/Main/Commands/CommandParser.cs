using Game.Core;
using Game.Entities;
using Game.Fluids;
using Game.Terrains.Gen;
using OpenGL;
using System;
using System.Reflection;

namespace Game.Main {
    static class CommandParser {

        private class UnknownCommandException : Exception {
            public UnknownCommandException(string message) : base("Unknown command: \"" + message + "\"") { }
        }

        private static class EntityParser {
            internal static object Parse(string s) {
                //separates the entity
                var arr = s.Split(new char[] { ' ' }, 2);
                switch (arr[0]) {
                    case "Player":
                        return ParseWithEntity(Player.Instance, arr[1]);
                }
                throw new UnknownCommandException(arr[0]);
            }

            private static object ParseWithEntity(Entity e, string s) {
                var arr = s.Split(new char[] { ' ' }, 2);
                switch (arr[0]) {
                    case "teleport":
                        var pos = ParseVec2(arr[1]);
                        e.Teleport(pos);
                        return null;
                    case "entitydata":
                        return GetField(e.data, e.data.GetType(), arr[1]);
                }
                throw new UnknownCommandException(arr[0]);
            }
        }

        private static class FluidParser {
            internal static object Parse(string s) {
                return InvokeMethod(typeof(FluidManager), s, FluidManager.Instance, null);
            }
        }

        public static object Execute(string s) {
            if (s == "") throw new ArgumentException("Empty command");
            var arr = s.Split(new char[] { ' ' }, 2);
            switch (arr[0]) {
                case "fluids":
                    return FluidParser.Parse(arr[1]);
                case "entity":
                    return EntityParser.Parse(arr[1]);
                case "seed":
                    return TerrainGen.seed;
            }
            throw new ArgumentException("Unknown command: \"" + arr[0] + "\"");
        }


        private static object GetField(object obj, Type type, string field) {
            FieldInfo info = type.GetField(field);
            return info.GetValue(obj);
        }

        private static object InvokeMethod(Type type, string method, object instance, object[] parameters) {
            return type.GetMethod(method).Invoke(instance, parameters);
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
