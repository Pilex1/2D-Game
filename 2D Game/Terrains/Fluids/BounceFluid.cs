using System;
using Game.Entities;
using Game.Util;

namespace Game.Terrains.Fluids {

    [Serializable]
    class BounceFluidAttribs : FlowFluidAttribs, ILight {
        public BounceFluidAttribs(int increments = 4) : base(increments, 4, Tile.BounceFluid) {
            mvtFactor = 0.1f;
        }

        public override void OnEntityCollision(int x, int y, Direction side, Entity e) {
            e.data.vel.val *= 1.01f;
        }

        int ILight.intensity() => 4;
    }
}
