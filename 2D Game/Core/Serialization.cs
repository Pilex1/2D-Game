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
        //public static Tile[,] LoadTerrain() {
        //try {
        //    using (BufferedStream stream = new BufferedStream(new FileStream(dir + file, FileMode.Open, FileAccess.Read, FileShare.None))) {
        //        IFormatter formatter = new BinaryFormatter();
        //        return (Tile[,])formatter.Deserialize(stream);
        //    }
        //} catch (Exception e) {
        //    Debug.WriteLine(e.Message);
        //    return null;
        //}
        //}

        //returns true if succesfully saved terrain, false otherwise
        // public static bool SaveTerrain(Tile[,] data) {
        //try {
        //    Directory.CreateDirectory(dir);
        //    using (BufferedStream stream = new BufferedStream(new FileStream(dir + file, FileMode.Create, FileAccess.Write, FileShare.None))) {
        //        IFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(stream, data);
        //    }
        //    return true;
        //} catch (Exception e) {
        //    Debug.WriteLine(e.Message);
        //    return false;
        //}
        //  }

    }


}
