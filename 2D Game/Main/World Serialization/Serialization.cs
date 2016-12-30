using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Game.Util;
using Game.Terrains.Fluids;
using Game.Terrains.Logics;
using Game.Terrains.Terrain_Generation;

namespace Game.Core.World_Serialization {

    static class Serialization {

        #region Fields
        private static readonly string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Plexico\\2D Game\\";
        private const string worlddir = "Worlds\\";

        private const string fileExt = ".plex";
        private const string fluidFile = "fluids";
        private const string logicFile = "logics";
        private const string chunkFile = "chunk_";
        private const string entityFile = "entities";
        #endregion

        #region File IO
        private static object Load(string file) {
            try {
                using (BufferedStream stream = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))) {
                    IFormatter formatter = new BinaryFormatter();
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    object data = formatter.Deserialize(stream);
                    watch.Stop();
                    Debug.WriteLine("Deserialised " + data + " from " + file + " in " + watch.ElapsedMilliseconds + " ms");
                    return data;
                }
            } catch (Exception e) {
                Debug.WriteLine("Failed to deserialise " + file);
                Debug.WriteLine(e.Message);
                throw e;
            }
        }

        private static void Save(string file, object data) {
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
                    Debug.WriteLine("Serialised " + data + " to " + file + " in " + watch.ElapsedMilliseconds + " ms");
                }
            } catch (Exception e) {
                Debug.WriteLine("Failed to serialise " + data + " to " + file);
                Debug.WriteLine(e.Message);
                throw e;
            }
        }

        private static void Delete(string file) {
            try {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Directory.Delete(file, true);
                watch.Stop();
                Debug.WriteLine("Deleted " + file + " in " + watch.ElapsedMilliseconds + " ms");
            } catch (Exception e) {
                Debug.WriteLine("Failed to delete " + file);
                Debug.WriteLine(e.Message);
            }
        }
        #endregion

        #region Util
        /// <summary>
        /// Creates a folder in AppData for saving world files if one does not already exist
        /// </summary>
        public static void CreateSaveFolder() {
            string path = dir + worlddir;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string[] GetWorlds() {
            string path = dir + worlddir;
            List<string> worlds = new List<string>();
            foreach (string s in Directory.GetDirectories(path)) {
                var sections = s.Split('\\');
                var worldfolder = sections[sections.Length - 1];
                if (VerifyWorld(worldfolder))
                    worlds.Add(worldfolder);
            }
            return worlds.ToArray();
        }

        /// <summary>
        /// Verifies that all the files required for a world exist (does not verify the actual contents of the files)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool VerifyWorld(string s) {
            string filedir = dir + worlddir + s + "\\";
            if (!File.Exists(filedir + fluidFile + fileExt)) return false;
            if (!File.Exists(filedir + logicFile + fileExt)) return false;
            if (!File.Exists(filedir + entityFile + fileExt)) return false;

            for (int i = 0; i < TerrainGen.ChunksPerWorld; i++) {
                if (!File.Exists(filedir + chunkFile + i + fileExt)) return false;
            }

            return true;
        }
        #endregion

        #region Save
        public static void SaveWorld(string file, ChunkData[] chunks, EntitiesData entities, Dictionary<Vector2i, FluidAttribs> fluids, Dictionary<Vector2i, LogicAttribs> logics) {
            Directory.CreateDirectory(dir + worlddir + file);

            foreach (ChunkData c in chunks) {
                Save(dir + worlddir + file + "\\" + chunkFile + c.location + fileExt, c);
            }

            Save(dir + worlddir + file + "\\" + entityFile + fileExt, entities);
            Save(dir + worlddir + file + "\\" + fluidFile + fileExt, fluids);
            Save(dir + worlddir + file + "\\" + logicFile + fileExt, logics);
        }
        #endregion

        #region Load
        public static EntitiesData LoadEntities(string world) => (EntitiesData)Load(dir + worlddir + world + "\\" + entityFile + fileExt);
        public static Dictionary<Vector2i, FluidAttribs> LoadFluids(string world) => (Dictionary<Vector2i, FluidAttribs>)Load(dir + worlddir + world + "\\" + fluidFile + fileExt);
        public static Dictionary<Vector2i, LogicAttribs> LoadLogics(string world) => (Dictionary<Vector2i, LogicAttribs>)Load(dir + worlddir + world + "\\" + logicFile + fileExt);
        public static ChunkData LoadChunk(string world, int region) => (ChunkData)Load(dir + worlddir + world + "\\" + chunkFile + region + fileExt);
        #endregion

        #region Delete
        public static void DeleteWorld(string file) {
            string path = dir + worlddir + file;
            Delete(path);
        }
        #endregion

    }
}
