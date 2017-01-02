using System;
using Pencil.Gaming.MathUtils;
using Game.Terrains;
using System.Collections.Generic;
using Game.Core;
using Game.Util;
using System.Linq;
using Game.Terrains.Fluids;

namespace Game.Entities {

    [Serializable]
    abstract class Entity {

        public EntityID entityId { get; private set; }
        public Hitbox hitbox { get; private set; }
        public EntityData data { get; private set; }

        #region Initialisation
        protected Entity(EntityID entityId, EntityData data) {
            this.data = data;
            this.entityId = entityId;
            CalculateHitbox();
        }
        protected Entity(EntityID entityId, Vector2 position, Vector2 size) : this(entityId, new EntityData()) {
            data.pos.val = position;
            data.size = size;
            CalculateHitbox();
        }

        private void CalculateHitbox() {
            hitbox = new RectangularHitbox(data.pos.val, data.size - MathUtil.Epsilon * Vector2.One);
        }

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
            var col = Array.FindAll(GetTerrainCollisions(), t => t.Item2.tileattribs is FluidAttribs);
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
            data.vel.y *= -1;
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

            if (Array.Exists(GetTerrainCollisions(), t => t.Item2.tileattribs is FluidAttribs)) {
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

        public Tuple<Vector2i, Tile>[] GetTerrainCollisions() {
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

        /// <summary>
        /// Reduces life by the specified amount with no physical indication
        /// </summary>
        /// <param name="dmg"></param>
        public void DamageNatural(float dmg) {
            if (data.invulnerable) return;
            data.life.val -= dmg;
        }

        /// <summary>
        /// Reduces life by the specified amount and adds a physical indication
        /// </summary>
        /// <param name="dmg"></param>
        public void Damage(float dmg) {
            DamageNatural(dmg);
            if (data.invulnerable) return;
            data.recentDmg = EntityData.maxRecentDmgTime;
        }

        /// <summary>
        /// Heals life by specified amount up to the maximum life
        /// </summary>
        /// <param name="dmg"></param>
        public void Heal(float dmg) {
            data.life.val += dmg;
        }

        /// <summary>
        /// Sets the maximum life to the specified value and heals fully
        /// </summary>
        /// <param name="life"></param>
        public void SetMaxLifeFull(float life) {
            data.life = new BoundedFloat(life, 0, life);
        }

        /// <summary>
        /// Decreases maximum life by specified value and updates current health if required
        /// </summary>
        /// <param name="decr"></param>
        public void DecrMaxLife(float decr) {
            data.life.max -= decr;
            if (data.life.max < 0) data.life.max = 0;
            data.life.val += 0;
        }

        /// <summary>
        /// Increases maximum life by specified value
        /// </summary>
        /// <param name="incr"></param>
        public void IncrMaxLife(float incr) {
            data.life.max += incr;
        }

        #endregion

        public Matrix ModelMatrix() => MathUtil.CalculateModelMatrix(data.size, data.rot, data.pos);

        /// <summary>
        /// Called when the entity is being deserialized to add any cooldown timers to the CooldownTimer class
        /// </summary>
        public virtual void InitTimers() { }

        /// <summary>
        /// Executed every frame.
        /// Don't forget to call UpdatePosition() if required
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Executed every frame to update the entity's hitbox. By default, the hitbox position is set to the entity's position as defined by its entity data.
        /// </summary>
        public virtual void UpdateHitbox() => hitbox.Position = data.pos;

        /// <summary>
        /// Executed when the entity collides with terrain
        /// </summary>
        /// <param name="x">The x coordinate of the terrain collision</param>
        /// <param name="y">The y coordinate of the terrain collision</param>
        /// <param name="d">The direction in which the entity collided on</param>
        /// <param name="t">The tile in which the entity collided with</param>
        public virtual void OnTerrainCollision(int x, int y, Direction d, Tile t) { }

        /// <summary>
        /// Executed on entity's death. By default, the entity is removed.
        /// </summary>
        public virtual void OnDeath() => EntityManager.RemoveEntity(this);
    }
}
