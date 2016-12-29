using System;
using Game.Entities;
using Game.Util;
using OpenGL;

namespace Game.Terrains.Fluids {

    [Serializable]
    class BounceFluidAttribs : FlowFluidAttribs, ILight {
        public BounceFluidAttribs(int increments = 4) : base(increments, 4, Tile.BounceFluid) {
            mvtFactor = 0.1f;
        }

        public override void OnEntityCollision(int x, int y, Direction side, Entity e) {
            e.data.vel.val *= 1.01f;
        }

        int ILight.Radius() => 4;

        Vector4 ILight.Colour() => new Vector4(0.1, 0.5, 0.3, 1);
    }
}
