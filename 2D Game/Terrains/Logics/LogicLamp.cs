using Game.Items;
using Game.Terrains.Lighting;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Terrains.Logics {

    [Serializable]
    class LogicLampAttribs : PowerDrain, IMultiLight {

        protected bool state;

        public LogicLampAttribs() : base(delegate () { return RawItem.LogicLamp; }) {
            powerIn.SetPowerAll(new BoundedFloat(16));
            cost = 2;
            state = false;
        }

        protected override void UpdateMechanics(int x, int y) {
            BoundedFloat buffer = new BoundedFloat(0, 0, cost);
            CacheInputs();
            powerIn.GivePowerAll(ref buffer);
            EmptyInputs();
            state = buffer.IsFull();
            UpdateMultiLight(x, y, state ? 1 : 0, this);
            Terrain.TileAt(x, y).enumId = state ? TileID.LogicLampOn : TileID.LogicLampOff;
        }

        ILight[] IMultiLight.Lights() => new ILight[] { new CLight(0, 0, Vector3.Zero), new CLight(6, 1f, new Vector3(1, 0.9f, 0.9f)) };
        int IMultiLight.State { get; set; }
    }
}
