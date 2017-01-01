using System;
using Game.Entities;
using Game.Util;
using Pencil.Gaming.MathUtils;

namespace Game.Terrains.Fluids {

    [Serializable]
    class BounceFluidAttribs : FlowFluidAttribs, ILight {
        public BounceFluidAttribs(int increments = 4) : base(increments, 4, Tile.BounceFluid) {
            mvtFactor = 0.2f;
        }

        public override void OnEntityCollision(int x, int y, Direction side, Entity e) {
            e.data.vel.val *= 1.01f;
        }

        int ILight.Radius() => 4;
        Vector3 ILight.Colour() => new Vector3(0.2f, 1, 0.6f);
        float ILight.Strength() => 0.04f;
    }
}
