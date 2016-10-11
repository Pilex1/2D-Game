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

namespace Game.Particles {
    abstract class StaffParticle : Particle {
        public StaffParticle(EntityModel model, Vector2 pos) : base(model, pos) {

        }

    }

    class StaffParticlePurple : StaffParticle {

        private static CooldownTimer cooldown;
        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    _model = EntityModel.CreateRectangle(new Vector2(0.5, 0.5), Color.DarkViolet);
                    _model.blend = true;
                }
                return _model;
            }
        }

        private float theta;
        private float radius;
        private Vector2 center;

        internal static new void Init() {
            cooldown = new CooldownTimer(20f);
        }

        public static void Create(Vector2 pos) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            int n = 100;
            for (int i = 0; i < n; i++) {
                pos += MathUtil.RandVector2(Program.Rand, new Vector2(-0.2, -0.2), new Vector2(0.2, 0.2));
                new StaffParticlePurple(pos, 10, (float)Math.PI * 2 * i / n);
            }
        }

        private StaffParticlePurple(Vector2 pos, float radius, float theta) : base(Model, pos) {
            this.center = pos;
            this.radius = radius;
            this.theta = theta;
            ParticleData pdata = (ParticleData)base.data;
            pdata.life = 20;
            pdata.vel.val = Vector2.Zero;
            base.data.AirResis = 1f;
        }

        public override void Update() {
            base.Update();
            base.data.Position.val = center + radius * MathUtil.Vec2FromAngle(theta);
        }

    }

    class StaffParticleRed : StaffParticle {

        private static float theta = 0;
        private static CooldownTimer cooldown;
        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    _model = EntityModel.CreateRectangle(new Vector2(0.5, 0.5), Color.IndianRed);
                    _model.blend = true;
                }
                return _model;
            }
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            vel *= MathUtil.Vec2FromAngle(theta);
            new StaffParticleRed(pos, vel);
            theta += 0.3f;
        }

        private StaffParticleRed(Vector2 pos, Vector2 vel) : base(Model, pos) {
            ParticleData pdata = (ParticleData)base.data;
            pdata.life = 100;
            pdata.rotfactor = 0.001f;
            pdata.vel.val = vel;
            base.data.AirResis = 1f;
            base.data.UseGravity = false;
        }

        public override void Update() {
            base.Update();
            List<Entity> colliding = this.EntityCollisions();
            foreach (Entity e in colliding) {
                Shooter shooter = e as Shooter;
                if (shooter != null) {
                    shooter.data.vel.y = 0.5f;
                    shooter.UpdatePosition();
                }

                Projectile proj = e as Projectile;
                if (proj != null) {
                    proj.data.vel.val *= -1.2f;
                    proj.UpdatePosition();
                }
            }
        }
    }

    class StaffParticleBlue : StaffParticle {

        private static CooldownTimer cooldown;
        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    _model = EntityModel.CreateRectangle(new Vector2(0.5, 0.5), Color.CadetBlue);
                    _model.blend = true;
                }
                return _model;
            }
        }

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
                ((ParticleData)data).curlife -= 2;
                //Entity.RemoveEntity(this);
            }
        }

        private StaffParticleBlue(Vector2 pos, Vector2 vel) : base(Model, pos) {
            ParticleData pdata = (ParticleData)base.data;
            pdata.life = 100;
            pdata.rotfactor = 0.001f;
            base.data.vel.val = vel;
            base.data.AirResis = 1f;
            base.data.UseGravity = false;
        }
    }

    class StaffParticleGreen : StaffParticle {

        private static CooldownTimer cooldown;
        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    _model = EntityModel.CreateRectangle(new Vector2(0.5, 0.5), Color.ForestGreen);
                    _model.blend = true;
                }
                return _model;
            }
        }

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
                Shooter shooter = e as Shooter;
                if (shooter != null)
                    Entity.RemoveEntity(shooter);

                Projectile proj = e as Projectile;
                if (proj != null)
                    Entity.RemoveEntity(proj);
            }
        }

        private StaffParticleGreen(Vector2 pos, Vector2 vel) : base(Model, pos) {
            ParticleData pdata = (ParticleData)base.data;
            pdata.life = 100;
            pdata.rotfactor = 0.001f;

            base.data.vel.val = vel;
            base.data.AirResis = 0.999f;
            base.data.Grav = 0.01f;
            base.data.UseGravity = true;
        }
    }

}
