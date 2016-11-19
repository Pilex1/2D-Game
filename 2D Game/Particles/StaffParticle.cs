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
        public StaffParticle(EntityID model, Vector2 pos)
            : base(model, pos) {

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

        private StaffParticlePurple(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticlePurple, pos) {
            base.data.life = new BoundedFloat(100, 0, 100);
            base.data.vel.val = vel;
            base.data.AirResis = 1f;
        }

        public override void Update() {
            base.Update();
            if (Terrain.IsColliding(base.Hitbox) || ((ParticleData)data).life <= 0) {
                int x = (int)base.data.Position.x;
                int y = (int)base.data.Position.y;

                Terrain.SetTile(x - 1, y, Tile.Water);
                Terrain.SetTile(x + 1, y, Tile.Water);
                Terrain.SetTile(x, y + 1, Tile.Water);
                Terrain.SetTile(x, y - 1, Tile.Water);

                Entity.RemoveEntity(this);
            }
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

        private StaffParticleRed(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleRed, pos) {
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
                // if (e is Player) continue;
                e.data.vel.x *= 1.0001f;
            }
            if (colliding.Count > 0) Entity.RemoveEntity(this);
            base.RemoveIfNoLife();
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
                base.data.vel.val *= 0.9f;
                //Entity.RemoveEntity(this);
            }
            base.RemoveIfNoLife();
        }

        private StaffParticleBlue(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleBlue, pos) {
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
            bool flag = false;
            foreach (Entity e in colliding) {
                if (e is Player) continue;
                if (e is Particle) continue;
                e.Damage(1f);
                e.data.vel.val *= -10;
                e.UpdatePosition();
                e.data.vel.val /= -10;
                flag = true;
            }
            if (flag) Entity.RemoveEntity(this);
            base.RemoveIfNoLife();
        }

        private StaffParticleGreen(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleGreen, pos) {
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
