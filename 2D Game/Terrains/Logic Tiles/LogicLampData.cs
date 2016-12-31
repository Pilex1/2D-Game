using System;
using Game.Util;
using Game.Items;
using OpenGL;

namespace Game.Terrains.Logics {

    [Serializable]
    class LogicLampAttribs : PowerDrainData, ILight {

        public bool state { get; protected set; }

        public LogicLampAttribs():base(delegate() { return RawItem.LogicLamp; }) {
            powerinL.max = powerinR.max = powerinU.max = powerinD.max = 16;
            cost = 2;
            state = false;
        }

        internal override void Update(int x, int y) {

            BoundedFloat buffer = new BoundedFloat(0, 0, cost);

            CachePowerLevels();

            BoundedFloat.MoveVals(ref powerinL, ref buffer, powerinL.val);
            BoundedFloat.MoveVals(ref powerinR, ref buffer, powerinR.val);
            BoundedFloat.MoveVals(ref powerinU, ref buffer, powerinU.val);
            BoundedFloat.MoveVals(ref powerinD, ref buffer, powerinD.val);

            EmptyInputs();

            state = buffer.IsFull();

            Terrain.TileAt(x, y).enumId = state ? TileID.LogicLampOn : TileID.LogicLampOff;
        }

        int ILight.Radius() => 6;

        float ILight.Strength() => 1f;

        Vector3 ILight.Colour() => state ? new Vector3(1, 0.9, 0.9) : Vector3.Zero;
    }
}
