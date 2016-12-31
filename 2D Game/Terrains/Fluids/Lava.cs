using OpenGL;
using System;

namespace Game.Terrains.Fluids {
    [Serializable]
    class LavaAttribs : FlowFluidAttribs, ILight {
        public LavaAttribs(int increments = 8) : base(increments, 8, Tile.Lava) {
            mvtFactor = 0.02f;
        }

        protected override void UpdateFinal(int x, int y) {
            MorphStone(x, y, x, y + 1);
            MorphStone(x, y, x, y - 1);
            MorphStone(x, y, x - 1, y);
            MorphStone(x, y, x + 1, y);
        }

        private void MorphStone(int srcx, int srcy, int x, int y) {
            if (Terrain.TileAt(x, y).tileattribs is WaterAttribs) {
                Terrain.BreakTile(srcx, srcy);
                Terrain.SetTile(srcx, srcy, Tile.Obsidian);
            }
        }

        int ILight.Radius() => 12;
        Vector3 ILight.Colour() => new Vector3(1, 0.4, 0.2);
        float ILight.Strength() => 0.1f;
    }
}
