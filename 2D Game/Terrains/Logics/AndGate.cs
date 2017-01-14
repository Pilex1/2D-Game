using Game.Items;
using Game.Util;
using System;

namespace Game.Terrains.Logics {

    //inputs from top and bottom
    //output from right iff top > 0 && bottom > 0
    [Serializable]
    class AndGateAttribs : PowerTransmitter {

        public AndGateAttribs() : base(() => RawItem.GateAnd) {
            BoundedFloat p = new BoundedFloat(64);

            powerOut.SetPowerAll(BoundedFloat.Zero);
            powerOut.SetPower(Direction.Right, p);

            powerIn.SetPowerAll(BoundedFloat.Zero);
            powerIn.SetPower(Direction.Up, p);
            powerIn.SetPower(Direction.Down, p);
        }

        protected override void UpdateMechanics(int x, int y) {
            BoundedFloat buffer = new BoundedFloat(0, 0, 128);
            base.CacheInputs();
            bool cond = powerIn.GetPower(Direction.Up) > 0 && powerIn.GetPower(Direction.Down) > 0;
            powerIn.GivePowerAll(ref buffer);
            EmptyInputs();
            EmptyOutputs();
            if (cond) {
                powerIn.TakePower(Direction.Right, ref buffer);
            }
            CacheOutputs();
            TransferPowerAll(x, y);
        }
    }
}
