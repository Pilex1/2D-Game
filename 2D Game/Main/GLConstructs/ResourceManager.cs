using Game.Main.GLConstructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
