using System;

namespace Game.Util {
    class BoolSwitch {
        private bool val;
        public BoolSwitch(bool val) {
            this.val = val;
        }
        public void Toggle() {
            val = !val;
        }
        public static implicit operator bool(BoolSwitch bs) {
            return bs.val;
        }
        public static implicit operator BoolSwitch(bool b) {
            return new BoolSwitch(b);
        }

    }
}
