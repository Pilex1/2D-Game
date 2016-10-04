using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;
using Game.Assets;
using Game.Terrains;
using System.Diagnostics;

namespace Game.Fluids {
    static class FluidsManager {

        //private const int FluidTexSize = 16;

        //private static HashSet<Fluid> FluidsList = new HashSet<Fluid>();
        //private static HashSet<Fluid> BatchFluids = new HashSet<Fluid>();
        //private static HashSet<Fluid> BatchRemoveFluids = new HashSet<Fluid>();

        //internal static void AddFluid(Fluid f) {
        //    BatchFluids.Add(f);
        //}

        //internal static void RemoveFluid(Fluid f) {
        //    BatchRemoveFluids.Add(f);
        //}

        //public static void Update() {

        //    foreach (Fluid f in BatchFluids) FluidsList.Add(f);
        //    foreach (Fluid f in BatchRemoveFluids) FluidsList.Remove(f);

        //    BatchFluids.Clear();
        //    BatchRemoveFluids.Clear();

        //    foreach (Fluid f in FluidsList) {
        //        f.Update();
        //    }

        //}
    }
}
