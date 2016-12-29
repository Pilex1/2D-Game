using Game.Entities;
using Game.Items;
using Game.Util;
using OpenGL;
using System;
using System.Text;

namespace Game.Terrains.Logics {

    [Serializable]
    class EntitySpawnerAttribs : PowerDrainData {

        [NonSerialized]
        private CooldownTimer cooldown;

        private const int range = 4;
        internal bool state;
        public EntityCage entityCage;

        public EntitySpawnerAttribs() : base(delegate () { return RawItem.EntitySpawner; }) {
            powerinL.max = powerinR.max = powerinD.max = 16;
            powerinU.max = 0;
            cost = 8;
            state = false;
        }

        protected override void OnDestroy(int x, int y, Inventory inv) {
            base.OnDestroy(x, y, inv);
            if (entityCage != null) {
                entityCage.Release();
                EntityManager.RemoveEntity(entityCage);
            }

            entityCage = null;
        }

        internal override void Update(int x, int y) {
            if (entityCage == null) {
                entityCage = new EntityCage(new Vector2(x, y + 1));
                EntityManager.AddEntity(entityCage);
            }

            if (cooldown == null)
                cooldown = new CooldownTimer(100);

            BoundedFloat buffer = new BoundedFloat(0, 0, cost);

            CachePowerLevels();

            if (powerinL + powerinR + powerinD >= buffer.max) {
                BoundedFloat.MoveVals(ref powerinL, ref buffer, powerinL);
                BoundedFloat.MoveVals(ref powerinR, ref buffer, powerinR);
                BoundedFloat.MoveVals(ref powerinD, ref buffer, powerinD);
            }

            EmptyInputs();

            if (buffer.IsFull()) {
                state = true;
                if (cooldown.Ready()) {
                    Spawn(x, y);
                    cooldown.Reset();
                }
            } else {
                state = false;
            }

            Terrain.TileAt(x, y).enumId = state ? TileID.EntitySpawnerOn : TileID.EntitySpawnerOff;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine("Captured Entity: " + (entityCage.CapturedEntity == null ? "None" : entityCage.CapturedEntity.GetType().ToString()));
            return sb.ToString();
        }

        private void Spawn(int x, int y) {
            if (entityCage.CapturedEntity == null) return;
            int xoffset = MathUtil.RandInt(Program.Rand, -range, range);
            int yoffset = MathUtil.RandInt(Program.Rand, -range, range);

            var pos = new Vector2(x + xoffset, y + yoffset);

            var s = entityCage.CapturedEntity;

            if (s is Shooter) {
                SpawnEntity(new Shooter(pos));
            } else if (s is Squisher) {
                SpawnEntity(new Squisher(pos));
            }
        }

        private void SpawnEntity(Entity e) {
            if (e.Colliding()) return;
            EntityManager.AddEntity(e);
        }
    }
}
