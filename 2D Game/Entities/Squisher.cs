using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Entities {
    class Squisher : Entity {

        private CooldownTimer trackTimer;
        private CooldownTimer dmgTimer;

        private Vector2 initialVel = Vector2.Zero;

        public Squisher(Vector2 position) : base(EntityID.Squisher, position) {
            trackTimer = new CooldownTimer(20);
            dmgTimer = new CooldownTimer(50);
            base.data.speed = 0;
            base.data.jumppower = 0;
            base.data.UseGravity = true;
            base.data.AirResis = 0.99f;
            base.data.life = new BoundedFloat(10, 0, 10);
            base.CorrectTerrainCollision();
        }

        public override void Update() {
            base.Update();

            if (Player.Intersecting(this) && dmgTimer.Ready()) {
                Player.Instance.Damage(2);
                dmgTimer.Reset();
            }

            Vector2 toPlayer;
            float x;

            if (base.data.InAir) {
                if (Math.Abs(base.data.vel.x) == 0) {
                    base.data.vel.x = initialVel.x;
                } else {
                    trackTimer.Reset();
                }
                return;
            } else {
                base.data.vel.x = 0;
            }
            if (!trackTimer.Ready()) {
                return;
            }

            trackTimer.Reset();
            toPlayer = Player.ToPlayer(base.data.Position);
            x = toPlayer.x;
            x += MathUtil.RandFloat(Program.Rand, -0.5, 0.5);
            x /= 7;
            Vector2 vel = new Vector2(x, 0.5);
            initialVel = vel;
            base.data.vel.val = vel;
        }
    }
}
