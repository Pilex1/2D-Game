using System;
using Game.Util;
using Game.Fluids;
using System.Diagnostics;

namespace Game.Terrains.Gen {

    enum Biome {
        None, Plains, Desert, SnowForest, Mountain, Ocean
    }

    internal static class TerrainGen {

        private const int FluidSettlingCount = 2000;

        public const int size = 4000;
        internal const int widthfactor = 10;
        internal const int freq = size / widthfactor;

        internal const int minlandheight = 128;

        internal static Random rand;
        internal static int seed;

        internal static void Generate(int seed) {
            TerrainGen.seed = seed;
            rand = new Random(seed);
            Stopwatch watch = new Stopwatch();

            watch.Start();
            Console.WriteLine("Generating terrain...");

            Terrain.Tiles = new Tile[size, 512];
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                for (int j = 0; j < Terrain.Tiles.GetLength(1); j++) {
                    Terrain.Tiles[i, j] = Tile.Air;
                }
            }

            Terrain.TerrainBiomes = new Biome[size];
            for (int i = 0; i < Terrain.TerrainBiomes.GetLength(0); i++) {
                Terrain.TerrainBiomes[i] = Biome.None;
            }
            GenTerrain();

            // CaveGen.GenCaves();

            GenBedrock();

            GenDeco();

            watch.Stop();
            Console.WriteLine("Terrain generation finished in " + watch.ElapsedMilliseconds + " ms");

            watch.Reset();
            watch.Start();
            Console.WriteLine("Settling " + FluidManager.UpdateCount() + " fluids");

            for (int i = 0; i < FluidSettlingCount; i++) {
                FluidManager.Update();
            }
            FluidManager.ClearUpdates();

            watch.Stop();
            Console.WriteLine("Settled fluids in " + watch.ElapsedMilliseconds + " ms");

        }

        private static void GenDeco() {
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {

            }
        }

        private static void GenBedrock() {
            //gen bedrock
            for (int i = 0; i < Terrain.Tiles.GetLength(0); i++) {
                int y = MathUtil.RandInt(rand, 1, 6);
                for (int j = 0; j < y; j++) {
                    Terrain.SetTile(i, j, Tile.Bedrock, true);
                }
            }
        }

        private static void GenTerrain() {

            //gen
            int ptr = 0;
            int h = MathUtil.RandInt(rand, minlandheight, minlandheight + 20);
            int biomeSizeMin = 10, biomeSizeMax = 20;
            while (ptr < size / widthfactor) {
                int biomeSize = MathUtil.RandInt(rand, biomeSizeMin, biomeSizeMax);
                Biome b = (Biome)MathUtil.RandInt(rand, (int)Biome.Mountain, (int)Biome.Ocean);
                h = GenBiome(ptr * widthfactor, h, biomeSize, b);
                for (int i = ptr; i < ptr + biomeSize; i++) {
                    Terrain.TerrainBiomes[i] = b;
                }
                ptr += biomeSize;
            }


        }



        //returns height of last generated terrain
        private static int GenBiome(int posX, int posY, int size, Biome biome) {
            int lastHeight = 0;
            switch (biome) {
                case Biome.Plains:
                    lastHeight = Plains.Generate(posX, posY, size);
                    break;
                case Biome.Desert:
                    lastHeight = Desert.Generate(posX, posY, size);
                    break;
                case Biome.Mountain:
                    lastHeight = Mountain.Generate(posX, posY, size);
                    break;
                case Biome.SnowForest:
                    lastHeight = SnowForest.Generate(posX, posY, size);
                    break;
                case Biome.Ocean:
                    lastHeight = Ocean.Generate(posX, posY, size);
                    break;
            }

            return lastHeight;
        }



    }
}
