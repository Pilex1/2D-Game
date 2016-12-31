using System;
using OpenGL;
using Game.Terrains;
using System.Collections.Generic;
using Game.Core;
using Game.Util;
using System.Linq;
using Game.Terrains.Fluids;

namespace Game.Entities {

    [Serializable]
    abstract class Entity {

        public EntityID entityId;
        public Hitbox hitbox { get; protected set; }
        public EntityData data = new EntityData { };

        #region Initialisation
        protected Entity(EntityID entityId, Hitbox hitbox, EntityData data) {
            this.data = data;
            this.entityId = entityId;
            this.hitbox = hitbox;
            this.hitbox.Position = data.pos;
            this.hitbox.Size.x -= MathUtil.Epsilon;
            this.hitbox.Size.y -= MathUtil.Epsilon;
        }
        protected Entity(EntityID entityId, Hitbox hitbox, Vector2 position) : this(entityId, hitbox, new EntityData()) {
            data.pos.val = position;
        }
        protected Entity(EntityID entityId, Vector2 position) : this(entityId, new RectangularHitbox(position, Assets.Models.GetModel(entityId).size), position) { }

        #endregion

        #region Movement

        public void Teleport(Vector2 v) {
            Vector2i gridPrev = EntityManager.GetGridArray(this);
            data.pos.val = v;
            Vector2i gridNow = EntityManager.GetGridArray(this);
            if (gridPrev != gridNow) {
                //recalc grid array
                EntityManager.EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityManager.EntityGrid[gridNow.x, gridNow.y].Add(this);
            }
        }

        public void MoveLeft() {
            data.vel.x -= data.speed * GameTime.DeltaTime;
        }
        public void MoveRight() {
            data.vel.x += data.speed * GameTime.DeltaTime;
        }
        public void Jump() {
            var col = Array.FindAll(CalcTerrainCollision(), t => t.Item2.tileattribs is FluidAttribs);
            float mvt = 0;
            if (col.Length > 0) {
                mvt = col.Average(t => ((FluidAttribs)t.Item2.tileattribs).mvtFactor);
                data.mvtState = MovementState.Fluid;
            }

            switch (data.mvtState) {
                case MovementState.Ground:
                    data.vel.y = data.jumppower;
                    data.mvtState = MovementState.Air;
                    break;
                case MovementState.Air:
                    break;
                case MovementState.Fluid:
                    data.vel.y += data.jumppower * mvt;
                    break;
            }
        }
        public void Fall() {
            data.vel.y = -data.jumppower;
        }

        public void ReboundX() {
            if (data.reboundedX) return;
            data.reboundedX = true;
            data.vel.x *= -1;
        }

        public void ReboundY() {
            if (data.reboundedY) return;
            data.reboundedY = true;
            data.vel.y *= -1;
        }

        private void UpdateX(float x) {
            x = MathUtil.Clamp(x, -1, 1);
            Vector2 offset = new Vector2(x, 0);
            Tuple<Vector2i, Tile>[] futureCollision = Terrain.CalcFutureCollision(this, offset);

            if (futureCollision.Length == 0) {
                //in air
                data.pos.val += offset;
                data.mvtState = MovementState.Air;

            } else if (Array.Exists(futureCollision, t => !(t.Item2.tileattribs is FluidAttribs))) {
                //collision with non-fluid tiles

                foreach (var tuple in futureCollision) {
                    Vector2i pos = tuple.Item1;
                    Tile tile = tuple.Item2;
                    if (x >= 0) {
                        if (data.calcTerrainCollisions)
                            tile.tileattribs.OnEntityCollision(pos.x, pos.y, Direction.Right, this);

                        OnTerrainCollision(pos.x, pos.y, Direction.Right, tile);
                    } else {
                        if (data.calcTerrainCollisions)
                            tile.tileattribs.OnEntityCollision(pos.x, pos.y, Direction.Left, this);

                        OnTerrainCollision(pos.x, pos.y, Direction.Left, tile);
                    }
                }

            } else {
                //collision with fluid

                data.pos.val += offset;
                data.mvtState = MovementState.Fluid;
            }
        }

        private void UpdateY(float y) {
            y = MathUtil.Clamp(y, -1, 1);
            Vector2 offset = new Vector2(0, y);
            Tuple<Vector2i, Tile>[] futureCollision = Terrain.CalcFutureCollision(this, offset);

            if (futureCollision.Length == 0) {
                //in air
                data.pos.val += offset;
                data.mvtState = MovementState.Air;

            } else if (Array.Exists(futureCollision, t => !(t.Item2.tileattribs is FluidAttribs))) {
                //collision with non-fluid tiles

                foreach (var tuple in futureCollision) {
                    Vector2i pos = tuple.Item1;
                    Tile tile = tuple.Item2;
                    if (y >= 0) {
                        if (data.calcTerrainCollisions)
                            tile.tileattribs.OnEntityCollision(pos.x, pos.y, Direction.Up, this);

                        OnTerrainCollision(pos.x, pos.y, Direction.Right, tile);
                    } else {
                        if (data.calcTerrainCollisions)
                            tile.tileattribs.OnEntityCollision(pos.x, pos.y, Direction.Down, this);

                        OnTerrainCollision(pos.x, pos.y, Direction.Left, tile);
                        data.mvtState = MovementState.Ground;
                    }
                }

            } else {
                //collision with fluid

                data.pos.val += offset;
                data.mvtState = MovementState.Fluid;
            }
        }

        public void UpdatePosition() {

            Vector2i gridPrev = EntityManager.GetGridArray(this);

            data.vel.y -= data.grav * GameTime.DeltaTime;
            data.vel.x *= (float)Math.Pow(data.airResis, GameTime.DeltaTime);

            if (Array.Exists(CalcTerrainCollision(), t => t.Item2.tileattribs is FluidAttribs)) {
                data.mvtState = MovementState.Fluid;
            }

            UpdateX(data.vel.x * GameTime.DeltaTime);
            UpdateY(data.vel.y * GameTime.DeltaTime);

            Vector2i gridNow = EntityManager.GetGridArray(this);
            if (gridPrev != gridNow) {
                //recalc grid array
                EntityManager.EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityManager.EntityGrid[gridNow.x, gridNow.y].Add(this);
            }

            data.reboundedX = data.reboundedY = false;
        }


        #endregion

        #region Collisions

        /// <summary>
        /// Returns true if the current entity is colliding with a non-air tile.
        /// </summary>
        /// <returns></returns>
        public bool Colliding() {
            return Terrain.CalcCollision(this).Length != 0;
        }

        public Tuple<Vector2i, Tile>[] CalcTerrainCollision() {
            return Terrain.CalcCollision(this);
        }

        public void CorrectTerrainCollision() {
            Vector2i gridPrev = EntityManager.GetGridArray(this);
            data.pos.val = Terrain.CorrectTerrainCollision(this);
            Vector2i gridNow = EntityManager.GetGridArray(this);
            if (gridPrev != gridNow) {
                //recalc grid array
                EntityManager.EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityManager.EntityGrid[gridNow.x, gridNow.y].Add(this);
            }
        }

        public bool IsStuck() {
            int x1 = (int)Math.Floor(hitbox.Position.x);
            int x2 = (int)Math.Floor(hitbox.Position.x + hitbox.Size.x);

            int y1 = (int)Math.Floor(hitbox.Position.y);
            int y2 = (int)Math.Floor(hitbox.Position.y + hitbox.Size.y);
            for (int i = x1; i <= x2; i++) {
                for (int j = y1; j <= y2; j++) {
                    if (Terrain.TileAt(i, j).tileattribs.solid) {
                        return true;
                    }
                }
            }
            return false;
        }

        protected List<Entity> GetEntityCollisions() {
            Vector2i grid = EntityManager.GetGridArray(this);
            HashSet<Entity> set = EntityManager.EntityGrid[grid.x, grid.y];
            List<Entity> list = new List<Entity>(set);
            List<Entity> colliding = new List<Entity>();
            foreach (Entity e in list) {
                if (e == this)
                    continue;
                if (hitbox.Intersecting(e.hitbox))
                    colliding.Add(e);
            }
            return colliding;
        }

        #endregion

        #region Health
        public bool IsDead() {
            return data.life.IsEmpty();
        }

        public bool IsFullyHealed() {
            return data.life.IsFull();
        }

        public void HealFull() {
            data.life.Fill();
        }

        public void DamageNatural(float dmg) {
            if (data.invulnerable) return;
            data.life.val -= dmg;
        }

        public void Damage(float dmg) {
            DamageNatural(dmg);
            if (data.invulnerable) return;
            data.recentDmg = EntityData.maxRecentDmgTime;
        }

        public void Heal(float dmg) {
            data.life.val += dmg;
        }

        public void DecrMaxLife(float decr) {
            data.life.max -= decr;
            if (data.life.max < 0) data.life.max = 0;
            data.life.val += 0;
        }

        public void IncrMaxLife(float incr) {
            data.life.max += incr;
        }

        #endregion

        public virtual Matrix4 ModelMatrix() { return MathUtil.ModelMatrix(Assets.Models.GetModel(entityId).size, data.rot, data.pos); }
        public virtual void InitTimers() { }
        public abstract void Update();
        public virtual void UpdateHitbox() { hitbox.Position = data.pos; }
        public virtual void OnTerrainCollision(int x, int y, Direction d, Tile t) { }
        public virtual void OnDeath() { EntityManager.RemoveEntity(this); }
    }
}
