using Game.Terrains.Gen;
using System;
using Game.Terrains;
using System.Collections.Generic;
using Game.Util;
using Game.Fluids;
using Game.Logics;

namespace Game.Core.World_Serialization {

    [Serializable]
    public class TerrainData {
        internal Tile[,] terrain;
        internal Biome[] terrainbiomes;
        internal Dictionary<Vector2i, FluidAttribs> fluidDict;
        internal Dictionary<Vector2i, LogicAttribs> logicDict;
    }
}
