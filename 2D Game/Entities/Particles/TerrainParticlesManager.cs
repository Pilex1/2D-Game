using Game.Terrains.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Entities.Particles {

    //todo - when chunks are properly implemented
    class TerrainParticlesManager : UpdateTileManager<HashSet<TerrainParticleGenerator>> {

        public static TerrainParticlesManager Instance { get; private set; }
        private TerrainParticlesManager() : base(1000f / 60) { }
        public static void Init() => Instance = new TerrainParticlesManager();

        protected override void OnUpdate() {

        }
    }
}
