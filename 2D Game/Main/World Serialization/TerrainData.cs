using Game.Terrains.Terrain_Generation;
using System;
using Game.Terrains;
using System.Collections.Generic;
using Game.Util;
using Game.Terrains.Logics;
using Game.Terrains.Fluids;

namespace Game.Core.World_Serialization {

    [Serializable]
    public class TerrainData {
        internal Tile[,] terrain;
        internal Biome[] terrainbiomes;
        internal Dictionary<Vector2i, FluidAttribs> fluidDict;
        internal Dictionary<Vector2i, LogicAttribs> logicDict;
        internal float[,] lightings;
    }
}
