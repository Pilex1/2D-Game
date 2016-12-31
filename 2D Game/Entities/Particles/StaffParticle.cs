using System;
using System.Collections.Generic;
using OpenGL;
using Game.Util;
using Game.Core;
using Game.Entities;
using Game.Terrains;
using Game.Items;

namespace Game.Entities.Particles {

    [Serializable]
    abstract class StaffParticle : Particle {
        public StaffParticle(EntityID model, Vector2 pos, Vector2 vel)
            : base(model, pos) {
            data.vel.val = vel;
            deltaRot = 0.01f;
        }

        public override void Update() {
            base.Update();
            data.colour.w = data.life.val / data.life.max;
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            EntityManager.RemoveEntity(this);
        }
    }

    [Serializable]
    class SParc_Place : StaffParticle {

        private static CooldownTimer cooldown;

        private SParc_Place(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleYellow, pos, vel) {
            data.life = new BoundedFloat(100, 0, 100);
            data.airResis = 1f;
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }
        public override void InitTimers() {
            CooldownTimer.AddTimer(cooldown);
        }
        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new SParc_Place(pos, vel);
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            base.OnTerrainCollision(x, y, d, t);
            var slot = PlayerInventory.Instance.CurSelectedSlot + 1;
            if (slot >= PlayerInventory.Instance.Items.GetLength(0)) slot = 0;
            var item = PlayerInventory.Instance.Items[slot, 0];
            var attribs = item.rawitem.attribs;
            if (!(attribs is ItemAttribs_Tile)) return;
            Vector2 v = Input.RayCast();
            switch (d) {
                case Direction.Up:
                    attribs.Use(PlayerInventory.Instance, new Vector2i(slot, 0), new Vector2(x, y - 1), v);
                    break;
                case Direction.Right:
                    attribs.Use(PlayerInventory.Instance, new Vector2i(slot, 0), new Vector2(x - 1, y), v);
                    break;
                case Direction.Down:
                    attribs.Use(PlayerInventory.Instance, new Vector2i(slot, 0), new Vector2(x, y + 1), v);
                    break;
                case Direction.Left:
                    attribs.Use(PlayerInventory.Instance, new Vector2i(slot, 0), new Vector2(x + 1, y), v);
                    break;
            }
        }

        public override void Update() {
            base.Update();
        }

    }

    [Serializable]
    class SParc_Water : StaffParticle {

        private static CooldownTimer cooldown;

        private SParc_Water(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleBlue, pos, vel) {
            data.life = new BoundedFloat(100, 0, 100);
            data.airResis = 1f;
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }
        public override void InitTimers() {
            CooldownTimer.AddTimer(cooldown);
        }
        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new SParc_Water(pos, vel);
        }

        private void PlaceWater() {
            int x = (int)data.pos.x;
            int y = (int)data.pos.y;

            Terrain.SetTile(x - 1, y, Tile.Water());
            Terrain.SetTile(x + 1, y, Tile.Water());
            Terrain.SetTile(x, y + 1, Tile.Water());
            Terrain.SetTile(x, y - 1, Tile.Water());

            EntityManager.RemoveEntity(this);
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            PlaceWater();
        }

        public override void OnDeath() {
            PlaceWater();
        }
    }

    [Serializable]
    class SParc_Speed : StaffParticle {

        private static CooldownTimer cooldown;

        private SParc_Speed(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleGreen, pos, vel) {
            data.life = new BoundedFloat(100, 0, 100);
            data.airResis = 1f;
            data.grav = 0;
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(0.4f);
        }

        public override void InitTimers() {
            CooldownTimer.AddTimer(cooldown);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new SParc_Speed(pos, vel);
        }


        public override void Update() {
            base.Update();
            List<Entity> colliding = this.GetEntityCollisions();
            foreach (Entity e in colliding) {
                // if (e is Player) continue;
                e.data.vel.x += data.vel.val.x / 20;
                e.data.vel.y += data.vel.val.y / 100;
            }
            if (colliding.Count > 0) EntityManager.RemoveEntity(this);
        }
    }

    [Serializable]
    class SParc_Destroy : StaffParticle {

        private static CooldownTimer cooldown;

        private SParc_Destroy(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticleRed, pos, vel) {
            data.airResis = 1f;
            data.grav = 0;
            data.life = new BoundedFloat(100, 0, 100);
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(0.4f);
        }
        public override void InitTimers() {
            CooldownTimer.AddTimer(cooldown);
        }
        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new SParc_Destroy(pos, vel);
        }

        public override void OnTerrainCollision(int x, int y, Direction d, Tile t) {
            t.tileattribs.Destroy(x, y, PlayerInventory.Instance);
            data.life.val -= 10;
        }
    }

    [Serializable]
    class SParc_Damage : StaffParticle {

        private static CooldownTimer cooldown;

        private SParc_Damage(Vector2 pos, Vector2 vel)
            : base(EntityID.ParticlePurple, pos, vel) {
            data.airResis = 0.999f;
            data.grav = 0.01f;
            data.life = new BoundedFloat(100, 0, 100);
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(1f);
        }
        public override void InitTimers() {
            CooldownTimer.AddTimer(cooldown);
        }
        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new SParc_Damage(pos, vel);
        }

        public override void Update() {
            base.Update();
            List<Entity> colliding = GetEntityCollisions();
            bool flag = false;
            foreach (Entity e in colliding) {
                if (e is Player) continue;
                if (e is Particle) continue;
                e.Damage(10f);
                e.data.vel.val *= -10;
                e.UpdatePosition();
                e.data.vel.val /= -10;
                flag = true;
            }
            if (flag) EntityManager.RemoveEntity(this);
        }
    }
}
