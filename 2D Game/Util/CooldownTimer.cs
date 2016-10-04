using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Util {
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

        public static void Update() {
            foreach (var t in Timers) t.UpdateInstance();
        }


        private void UpdateInstance() {
            time += GameLogic.DeltaTime;
        }

    }
}
