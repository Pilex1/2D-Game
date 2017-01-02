using System;
using System.Collections.Generic;

namespace Game.Main.GLConstructs {
    static class ResourceManager {

        internal static HashSet<IDisposable> Resources = new HashSet<IDisposable>();

        public static void CleanUp() {
            foreach (IDisposable i in Resources) {
                i.Dispose();
            }
        }
    }
}
