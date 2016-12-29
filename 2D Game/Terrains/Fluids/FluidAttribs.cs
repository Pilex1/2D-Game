using Game.Items;
using System;
using System.Text;
using Game.Entities;
using Game.Util;

namespace Game.Terrains.Fluids {

    [Serializable]
    abstract class FluidAttribs : TileAttribs {

        public float mvtFactor;
        internal int maxIncrements;
        private int _increments;
        public int increments {
            get { return _increments; }
            set {
                if (_increments != value) updateNext = true;
                _increments = value;
            }
        }
        private bool updateNext;

        protected FluidAttribs(int increments, int maxIncrements) : base(delegate () { return RawItem.None; }) {
            this.increments = increments;
            this.maxIncrements = maxIncrements;
            mvtFactor = 0.02f;

            transparent = true;
            solid = false;
        }
        protected FluidAttribs(int maxIncrements, float mvtFactor) : this(maxIncrements, maxIncrements) { }

        public override void OnEntityCollision(int x, int y, Direction side, Entity e) {

        }

        //flow downwrds, flowing outwards
        public virtual void Update(int x, int y) {

            updateNext = false;

            TileID below = Terrain.TileAt(x, y - 1).enumId;
            FluidAttribs fluid = Terrain.TileAt(x, y - 1).tileattribs as FluidAttribs;

            if (below == TileID.Air) {
                FallAir(x, y);
            } else if (fluid != null && fluid.increments < fluid.maxIncrements) {
                FallFluid(x, y);
            } else {
                //bias to going left if there is not enough fluid to cover both left and right
                SpreadLeft(x, y);
                SpreadRight(x, y);
            }
            UpdateFinal(x, y);

            if (!updateNext) FluidManager.Instance.RemoveUpdate(x, y);
        }

        protected abstract void FallAir(int x, int y);
        protected abstract void FallFluid(int x, int y);
        protected abstract void SpreadLeft(int x, int y);
        protected abstract void SpreadRight(int x, int y);
        protected abstract void UpdateFinal(int x, int y);

        public float GetHeight() {
            return (float)increments / maxIncrements;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Amount: " + increments + " / " + maxIncrements);
            return sb.ToString();
        }

    }



}
