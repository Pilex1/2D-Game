using Game.Terrains;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.Core {
    static class Serialization {

        public static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Plexico\\2D Game\\";
        public static readonly string file = "terrain.plexicoDAT";

        //returns a tile array of the saved terrain if available, else null
        //TODO
        public static TileID[,] LoadTerrain() {
            Console.WriteLine("Deserialising terrain...");
            try {
                using (BufferedStream stream = new BufferedStream(new FileStream(dir + file, FileMode.Open, FileAccess.Read, FileShare.None))) {
                    IFormatter formatter = new BinaryFormatter();
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    TileID[,] data = (TileID[,])formatter.Deserialize(stream);
                    watch.Stop();
                    Console.WriteLine("Deserialised terrain in " + watch.ElapsedMilliseconds + " ms");
                    return data;
                }
            } catch (Exception e) {
                Console.WriteLine("Failed to deserialise terrain");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //returns true if succesfully saved terrain, false otherwise
        public static bool SaveTerrain(TileID[,] data) {
            Console.WriteLine("Serialising terrain...");
            try {
                Directory.CreateDirectory(dir);
                using (BufferedStream stream = new BufferedStream(new FileStream(dir + file, FileMode.Create, FileAccess.Write, FileShare.None))) {
                    IFormatter formatter = new BinaryFormatter();
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    formatter.Serialize(stream, data);
                    watch.Stop();
                    Console.WriteLine("Serialised terrain in " + watch.ElapsedMilliseconds + " ms");
                }
                return true;
            } catch (Exception e) {
                Console.WriteLine("Failed to serialise terrain");
                Console.WriteLine(e.Message);
                throw e;
                return false;
            }
        }

    }


}
