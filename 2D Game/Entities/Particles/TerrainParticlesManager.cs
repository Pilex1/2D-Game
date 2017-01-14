using Game.Terrains.Core;
using System.Collections.Generic;

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
