using Game.Items;
using Game.Terrains.Lighting;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Terrains.Logics {

    [Serializable]
    class WireAttribs : PowerTransmitterData, IMultiLight {

        protected bool state;

        public WireAttribs() : base(delegate () { return RawItem.Wire; }) {
            poweroutL.max = poweroutR.max = poweroutU.max = poweroutD.max = 64;
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 64;
            transparent = true;
        }

        internal override void Update(int x, int y) {

            //power input
            BoundedFloat buffer = new BoundedFloat(0, 0, powerinL.max + powerinR.max + powerinU.max + powerinD.max);
            CacheInputs();
            MovePowerIn(ref buffer);
            EmptyInputs();
            buffer -= dissipate;

            //power output
            EmptyOutputs();
            int neighbouring = NeighbouringLogics(x, y);
            if (neighbouring != 0) {
                float transval = buffer.val / neighbouring;

                if (IsLogicL(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutL, transval);

                if (IsLogicR(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutR, transval);

                if (IsLogicU(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutU, transval);

                if (IsLogicD(x, y))
                    BoundedFloat.MoveVals(ref buffer, ref poweroutD, transval);
            }
            CacheOutputs();
            state = poweroutL > 0 || poweroutR > 0 || poweroutU > 0 || poweroutD > 0;
            UpdateMultiLight(x, y, state ? 1 : 0, this);
            UpdateAll(x, y);
            Terrain.TileAt(x, y).enumId = state ? TileID.WireOn : TileID.WireOff;
        }

        ILight[] IMultiLight.Lights() => new ILight[] { new CLight(0, 0, Vector3.Zero), new CLight(4, 0.2f, new Vector3(1, 0.2f, 0.5f)) };
        int IMultiLight.State { get; set; }
    }
}
