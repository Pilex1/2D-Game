using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {

    [Serializable]
    class CooldownTimer {

        private static HashSet<CooldownTimer> Timers = new HashSet<CooldownTimer>();

        private float cooldown;

        [NonSerialized]
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

        public static void AddTimer(CooldownTimer t) {
            Timers.Add(t);
        }

        public static void Update() {
            foreach (var t in Timers) t.UpdateInstance();
        }

        public float GetTime() {
            return time;
        }

        public float GetCooldown() {
            return cooldown;
        }

        private void UpdateInstance() {
            time += GameTime.DeltaTime;
        }

        public override string ToString() {
            return String.Format("{0} / {1}", time, cooldown);
        }

    }
}
