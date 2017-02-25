using Game.Items;
using Game.Main.Util;
using Game.Terrains.Lighting;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Terrains.Logics {

    [Serializable]
    class WireAttribs : PowerTransmitter, IMultiLight {

        protected bool state;

        public WireAttribs() : base(delegate () { return RawItem.Wire; }) {
            BoundedFloat p = new BoundedFloat(64);
            powerOut.SetPowerAll(p);
            powerIn.SetPowerAll(p);
            transparent = true;
        }

        protected override void UpdateMechanics(int x, int y) {
            BoundedFloat buffer = new BoundedFloat(0, 0, 256);
            CacheInputs();
            state = powerIn.GetPower(Direction.Left).val > 0 || powerIn.GetPower(Direction.Right).val > 0 || powerIn.GetPower(Direction.Up).val > 0 || powerIn.GetPower(Direction.Down).val > 0;
            powerIn.GivePowerAll(ref buffer);
            EmptyInputs();
            buffer -= dissipate;



            TransferPowerAll(x, y, ref buffer);
            
            CacheOutputs();
          //  EmptyOutputs();

            UpdateMultiLight(x, y, state ? 1 : 0, this);
            Terrain.TileAt(x, y).enumId = state ? TileID.WireOn : TileID.WireOff;
        }

        ILight[] IMultiLight.Lights() => new ILight[] { new CLight(0, new ColourRGB(0,0,0)), new CLight(4, new ColourRGB(51, 10, 26)) };
        int IMultiLight.State { get; set; }
    }
}
