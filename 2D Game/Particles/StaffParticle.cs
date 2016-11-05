using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Drawing;
using Game.Util;
using Game.Core;
using Game.Entities;
using System.Diagnostics;
using Game.Terrains;
using Game.Assets;

namespace Game.Particles {
    abstract class StaffParticle : Particle {
        public StaffParticle(EntityID model, Vector2 pos) : base(model, pos) {

        }

    }

    class StaffParticlePurple : StaffParticle {

        private static CooldownTimer cooldown;

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new StaffParticlePurple(pos, vel);
        }

        private StaffParticlePurple(Vector2 pos, Vector2 vel) : base(EntityID.ParticlePurple, pos) {
            base.data.life = new BoundedFloat(5, 0, 5);
            base.data.vel.val = vel;
            base.data.AirResis = 1f;
        }

        public override void Update() {
            base.Update();
            Player.Instance.data.vel.y += 0.001f;
           
        }

    }

    class StaffParticleRed : StaffParticle {

        private static CooldownTimer cooldown;

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new StaffParticleRed(pos, vel);
        }

        private StaffParticleRed(Vector2 pos, Vector2 vel) : base(EntityID.ParticleRed, pos) {
            base.data.life = new BoundedFloat(100, 0, 100);
            base.data.vel.val = vel;
            base.data.AirResis = 1f;
            base.data.UseGravity = false;
            ParticleData pdata = (ParticleData)base.data;
            pdata.rotfactor = 0.001f;
        }

        public override void Update() {
            base.Update();
            List<Entity> colliding = this.EntityCollisions();
            foreach (Entity e in colliding) {
                if (e is Player) continue;
                Vector2 offset = base.data.vel.val / 150;
                e.data.vel.val += offset;
            }
        }
    }

    class StaffParticleBlue : StaffParticle {

        private static CooldownTimer cooldown;

        internal static new void Init() {
            cooldown = new CooldownTimer(0.4f);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new StaffParticleBlue(pos, vel);
        }

        public override void Update() {
            base.Update();
            if (Terrain.IsColliding(this)) {
                for (int i = (int)Hitbox.Position.x; i <= (int)Math.Ceiling(Hitbox.Position.x + Hitbox.Width); i++) {
                    for (int j = (int)Hitbox.Position.y; j < (int)Math.Ceiling(Hitbox.Position.y + Hitbox.Height); j++) {
                        Terrain.BreakTile(i, j);
                    }
                }
                base.data.life.val -= 2;
                //Entity.RemoveEntity(this);
            }
        }

        private StaffParticleBlue(Vector2 pos, Vector2 vel) : base(EntityID.ParticleBlue, pos) {
            base.data.vel.val = vel;
            base.data.AirResis = 1f;
            base.data.UseGravity = false;
            base.data.life = new BoundedFloat(100, 0, 100);
            ParticleData pdata = (ParticleData)base.data;
            pdata.rotfactor = 0.001f;
        }
    }

    class StaffParticleGreen : StaffParticle {

        private static CooldownTimer cooldown;

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new StaffParticleGreen(pos, vel);
        }

        public override void Update() {
            base.Update();
            List<Entity> colliding = this.EntityCollisions();
            foreach (Entity e in colliding) {
                if (e is Player) continue;
                if (e is Particle) continue;
                e.Damage(1f);
                e.data.vel.val *= -10;
                e.UpdatePosition();
                e.data.vel.val /= -10;
                Entity.RemoveEntity(this);
            }
        }

        private StaffParticleGreen(Vector2 pos, Vector2 vel) : base(EntityID.ParticleGreen, pos) {
            base.data.vel.val = vel;
            base.data.AirResis = 0.999f;
            base.data.Grav = 0.01f;
            base.data.UseGravity = true;
            base.data.life = new BoundedFloat(100, 0, 100);
            ParticleData pdata = (ParticleData)base.data;
            pdata.rotfactor = 0.001f;
        }
    }
}
