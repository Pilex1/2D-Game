using System;

namespace Game.Util {

    [Serializable]
    class Switch<T> {
        protected CooldownTimer timer;
        protected T val;

        public Switch(T t, float cooldown) {
            this.val = t;
            timer = new CooldownTimer(cooldown);
        }
        public Switch(T t) : this(t, 0) { }

        public void AddTimer() {
            CooldownTimer.AddTimer(timer);
        }

        /// <summary>
        /// Sets "val" if the cooldown timer is ready
        /// </summary>
        /// <param name="val"></param>
        public void Set(T val) {
            if (timer.Ready()) {
                this.val = val;
                timer.Reset();
            }
        }

        public T Get() {
            return val;
        }

        /// <summary>
        /// Sets "val" regardless of cooldown timer
        /// </summary>
        /// <param name="val"></param>
        public void ForceSet(T val) {
            this.val = val;
        }

        public void SetCooldown(float cooldown) {
            timer.SetCooldown(cooldown);
        }
    }

    [Serializable]
    class BoolSwitch : Switch<bool> {

        public BoolSwitch(bool val, float cooldown) : base(val, cooldown) {
        }
        public BoolSwitch(bool val) : this(val, 0) { }

        /// <summary>
        /// Toggles value if the cooldown timer is ready
        /// </summary>
        public void Toggle() {
            if (timer.Ready()) {
                val = !val;
                timer.Reset();
            }
        }

        /// <summary>
        /// Toggles value regardless of cooldown timer
        /// </summary>
        public void ForceToggle() {
            val = !val;
        }

        public static implicit operator bool(BoolSwitch bs) {
            return bs.val;
        }
    }
}
