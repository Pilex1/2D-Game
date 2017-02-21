using Game.Items;
using Game.Util;
using System;

namespace Game.Terrains.Logics {

    [Serializable]
    class NotGateAttribs : PowerTransmitter {

        public NotGateAttribs() : base(delegate () { return RawItem.GateNot; }) {
            powerOut.SetPowerAll(BoundedFloat.Zero);
            powerOut.SetPower(Direction.Right, new BoundedFloat(256));

            powerIn.SetPowerAll(BoundedFloat.Zero);
            powerIn.SetPower(Direction.Left, new BoundedFloat(256));
        }

        protected override void UpdateMechanics(int x, int y) {

            CacheInputs();

            EmptyOutputs();

            if (powerIn.GetPower(Direction.Left) > 0) {
                powerOut.SetPower(Direction.Right, 0);
            } else {
                powerOut.SetPower(Direction.Right, 256);
            }

            EmptyInputs();

            CacheOutputs();

            //TransferPowerAll(x, y);
        }
    }
}
