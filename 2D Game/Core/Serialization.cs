using Game.Terrains;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.Core {
    static class Serialization {

        private static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Plexico\\2D Game\\";
        private const string worlddir = "Worlds\\";
        private const string worldFile = "world.plex";

        private const string br = "------------------------------------";

        #region File IO
        private static object Load(string file) {
            Console.WriteLine("Deserialising " + file);
            try {
                using (BufferedStream stream = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))) {
                    IFormatter formatter = new BinaryFormatter();
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    object data = formatter.Deserialize(stream);
                    watch.Stop();
                    Console.WriteLine("Deserialised in " + watch.ElapsedMilliseconds + " ms");
                    return data;
                }
            } catch (Exception e) {
                Console.WriteLine("Failed to deserialise " + file);
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        private static void Save(string file, object data) {
            Console.WriteLine("Serialising " + data + " to " + file);
            try {
                if (!Directory.Exists(dir + worlddir)) {
                    Directory.CreateDirectory(dir + worlddir);
                }
                using (BufferedStream stream = new BufferedStream(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))) {
                    IFormatter formatter = new BinaryFormatter();
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    formatter.Serialize(stream, data);
                    watch.Stop();
                    Console.WriteLine("Serialised " + data + " in " + watch.ElapsedMilliseconds + " ms");
                }
            } catch (Exception e) {
                Console.WriteLine("Failed to serialise terrain");
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        #endregion

        public static string[] GetWorlds() {
            string path = dir + worlddir;
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
                return new string[] { };
            }
            List<string> worlds = new List<string>();
            foreach (string s in Directory.GetDirectories(path)) {
                if (File.Exists(s + "\\" + worldFile))
                    worlds.Add(s.Substring(Serialization.dir.Length + Serialization.worlddir.Length));
            }
            return worlds.ToArray();
        }

        public static WorldData LoadWorld(string str) {
            WorldData data = (WorldData)Load(dir + worlddir + str + "\\" + worldFile);
            return data;
        }


        public static void SaveWorld(string file, WorldData world) {
            Directory.CreateDirectory(dir + worlddir + file);
            Save(dir + worlddir + file + "\\" + worldFile, world);
        }

        public static void DeleteWorld(string file) {
            string path = dir + worlddir + file;
            try {
                Directory.Delete(path, true);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
