using Game.Terrains;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.Core {
    static class Serialization {

        private static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Plexico\\2D Game\\";
        private const string terrainFile = "terrain.plex";
        private const string playerFile = "player.plex";

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
                Directory.CreateDirectory(dir);
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

        #region Terrain
        public static TileID[,] LoadTerrain() {
            Console.WriteLine("Deserialising terrain...");
            return (TileID[,])Load(dir + terrainFile);
        }

        //returns true if succesfully saved terrain, false otherwise
        public static void SaveTerrain(TileID[,] data) {
            Console.WriteLine("Serialising terrain...");
            Save(dir + terrainFile, data);
        }
        #endregion Terrain

        #region Player

        public static PlayerData LoadPlayer() {
            Console.WriteLine("Deserialising player data...");
            return (PlayerData)Load(dir + playerFile);
        }

        public static void SavePlayer(PlayerData player) {
            Console.WriteLine("Serialising player data...");
            Save(dir + playerFile, player);
        }

        #endregion Player

    }


}
