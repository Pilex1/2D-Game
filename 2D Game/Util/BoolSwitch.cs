using System;

namespace Game.Util {

    [Serializable]
    class BoolSwitch {

        public bool val;

        private CooldownTimer cooldowntimer;

        public BoolSwitch(bool val, float cooldown) {
            this.val = val;
            this.cooldowntimer = new CooldownTimer(cooldown);
        }

        public BoolSwitch(bool val) : this(val, 0) {
        }

        private BoolSwitch() { }

        public void AddTimer() {
            CooldownTimer.AddTimer(cooldowntimer);
        }

        public void Toggle() {
            if (cooldowntimer.Ready()) {
                val = !val;
                cooldowntimer.Reset();
            }
        }
        public static implicit operator bool(BoolSwitch bs) {
            return bs.val;
        }

        internal void ResetTimer() {
            CooldownTimer.AddTimer(cooldowntimer);
        }
    }
}
