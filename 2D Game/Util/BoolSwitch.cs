using System;

namespace Game.Util {
    class BoolSwitch {
        private bool Val;
        public BoolSwitch(bool val) {
            Val = val;
        }
        public bool Toggle() {
            Val = !Val;
            return Val;
        }
        public bool Value() {
            return Val;
        }
    }
}
