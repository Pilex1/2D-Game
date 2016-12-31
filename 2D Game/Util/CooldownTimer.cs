using Game.Core;
using System;
using System.Collections.Generic;

namespace Game.Util {

    [Serializable]
    class CooldownTimer {

        private static HashSet<CooldownTimer> Timers = new HashSet<CooldownTimer>();

        private float cooldown;
        private float time = 0;

        public CooldownTimer(float cooldown) {
            this.cooldown = cooldown;
            Timers.Add(this);
        }

        public bool Ready() {
            return time >= cooldown;
        }

        public void Reset() {
            time = 0;
        }

        public void SetTime(float time) {
            this.time = time;
        }
        public float GetTime() {
            return time;
        }

        public float GetCooldown() {
            return cooldown;
        }
        public void SetCooldown(float f) {
            cooldown = f;
        }

        public override string ToString() {
            return string.Format("{0} / {1}", time, cooldown);
        }


        private void UpdateInstance() {
            time += GameTime.DeltaTime;
        }
        public static void AddTimer(CooldownTimer t) {
            Timers.Add(t);
        }

        public static void Update() {
            foreach (var t in Timers) t.UpdateInstance();
        }
    }
}
