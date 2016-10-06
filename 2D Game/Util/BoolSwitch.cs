using System;

namespace Game.Util {

    [Serializable]
    class BoolSwitch {

        private bool val;

        [NonSerialized]
        private CooldownTimer cooldown;

        public BoolSwitch(bool val, float cooldown) {
            this.val = val;
            this.cooldown = new CooldownTimer(cooldown);
        }

        public BoolSwitch(bool val) : this(val, 0) {
        }
        public void Toggle() {
            if (cooldown.Ready()) {
                val = !val;
                cooldown.Reset();
            }
        }
        public static implicit operator bool(BoolSwitch bs) {
            return bs.val;
        }
        public static implicit operator BoolSwitch(bool b) {
            return new BoolSwitch(b);
        }

    }
}
