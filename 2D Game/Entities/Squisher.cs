using Game.Util;
using OpenGL;
using System;
using Game.Core;

namespace Game.Entities {

    [Serializable]
    class Squisher : Entity {

        private CooldownTimer trackTimer;
        private CooldownTimer dmgTimer;

        private Vector2 initialVel = Vector2.Zero;

        public Squisher(Vector2 position) : base(EntityID.Squisher, position) {
            trackTimer = new CooldownTimer(20);
            dmgTimer = new CooldownTimer(50);
            data.speed = 0;
            data.jumppower = 0;
            data.airResis = 0.99f;
            data.life = new BoundedFloat(50, 0, 50);
        }

        public override void InitTimers() {
            CooldownTimer.AddTimer(trackTimer);
            CooldownTimer.AddTimer(dmgTimer);
        }

        public override void Update() {
            UpdatePosition();
            if (Player.Intersecting(this) && dmgTimer.Ready()) {
                Player.Instance.Damage(2);
                dmgTimer.Reset();
            }

            if (data.mvtState == MovementState.Air) {
                if (Math.Abs(data.vel.x) == 0) {
                    data.vel.x = initialVel.x;
                } else {
                    trackTimer.Reset();
                }
                return;
            } else {
                data.vel.x = 0;
            }

            if (!trackTimer.Ready()) {
                return;
            }
            trackTimer.Reset();



            float x = Player.ToPlayer(data.pos).x;
            x += MathUtil.RandFloat(Program.Rand, -0.5, 0.5);
            x /= 7;
            Vector2 vel = new Vector2(x, 0.5);
            initialVel = vel;
            data.vel.val = vel;


        }
    }
}
