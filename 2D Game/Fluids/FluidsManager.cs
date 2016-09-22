using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;
using Game.Assets;
using Game.Terrains;
using System.Diagnostics;

namespace Game.Fluids {
    static class FluidsManager {

        private const int FluidTexSize = 16;

        private static HashSet<IFluid> FluidsList = new HashSet<IFluid>();
        private static HashSet<IFluid> BatchFluids = new HashSet<IFluid>();
        private static HashSet<IFluid> BatchRemoveFluids = new HashSet<IFluid>();

        public static void AddFluid(IFluid f) {
            BatchFluids.Add(f);
        }

        public static void RemoveFluid(IFluid f) {
            BatchRemoveFluids.Add(f);
            Tile t = (Tile)f;
            Terrain.BreakTile(t);
        }

        public static void Update() {

            foreach (IFluid f in BatchFluids) FluidsList.Add(f);
            foreach (IFluid f in BatchRemoveFluids) FluidsList.Remove(f);
            BatchFluids.Clear();
            BatchRemoveFluids.Clear();

            foreach (IFluid f in FluidsList) {
                f.Flow();
            }

        }
    }
}
