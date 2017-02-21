using Game.Items;
using Game.Util;
using System;

namespace Game.Terrains.Logics {

    [Serializable]
    class SwitchAttribs : PowerSource {

        [NonSerialized]
        private CooldownTimer cooldown;

        public BoolSwitch state { get; private set; }

        public SwitchAttribs() : base(() => RawItem.Switch) {
            powerOut.SetPowerAll(new BoundedFloat(256));
            state = new BoolSwitch(false);
        }

        public override void OnInteract(int x, int y) {
            if (cooldown == null)
                cooldown = new CooldownTimer(20);

            if (cooldown.Ready()) {
                state.Toggle();
                cooldown.Reset();
            }
        }

        protected override void UpdateMechanics(int x, int y) {
            BoundedFloat power = new BoundedFloat(state ? 256 : 0);
            power.Fill();
            powerOut.TakePowerAll(ref power);
            CacheOutputs();
            TransferPowerAll(x, y, ref power);
            
            Terrain.TileAt(x, y).enumId = state ? TileID.SwitchOn : TileID.SwitchOff;
        }
    }
}
