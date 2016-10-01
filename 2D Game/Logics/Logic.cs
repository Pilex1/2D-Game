using Game.Assets;
using Game.Terrains;
using Game.Util;

namespace Game.Logics {

    abstract class Logic : Tile {


        protected Logic(int x, int y, TileID id) : base(x, y, id) {

            LogicManager.AddLogic(this);
        }

        internal abstract void Update();

    }

    internal abstract class PowerSource : Logic {

        //power available to draw from each side
        protected BoundedFloat powerL = BoundedFloat.Zero;
        protected BoundedFloat powerR = BoundedFloat.Zero;
        protected BoundedFloat powerU = BoundedFloat.Zero;
        protected BoundedFloat powerD = BoundedFloat.Zero;

        public PowerSource(int x, int y, TileID id) : base(x, y, id) {
        }

    }

    internal abstract class PowerTransmitter : Logic {

        //power on each side
        protected BoundedFloat powerL = BoundedFloat.Zero;
        protected BoundedFloat powerR = BoundedFloat.Zero;
        protected BoundedFloat powerU = BoundedFloat.Zero;
        protected BoundedFloat powerD = BoundedFloat.Zero;

        internal BoundedFloat transLevel = BoundedFloat.Zero;

        public PowerTransmitter(int x, int y, TileID id) : base(x, y, id) {
        }
    }

    internal abstract class PowerDrain : Logic {

        //power received
        internal BoundedFloat power = BoundedFloat.Zero;

        public PowerDrain(int x, int y, TileID id) : base(x, y, id) {

        }
    }
}
