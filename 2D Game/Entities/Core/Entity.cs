using System;
using OpenGL;
using Game.Terrains;
using System.Collections.Generic;
using Game.Core;
using Game.Util;
using Game.Fluids;
using System.Diagnostics;

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
        protected Entity(EntityID entityId, Hitbox hitbox, Vector2 position)
            : this(entityId, hitbox, new EntityData() { pos = new BoundedVector2(new BoundedFloat(position.x, 0, Terrain.Tiles.GetLength(0) - 1), new BoundedFloat(position.y, 0, Terrain.Tiles.GetLength(1) - 1)) }) {
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
            switch (data.mvtState) {
                case MovementState.Ground:
                    data.vel.y = data.jumppower;
                    data.mvtState = MovementState.Air;
                    break;
                case MovementState.Air:
                    break;
                case MovementState.Fluid:
                    data.vel.y = data.jumppower;
                    break;
            }
        }
        public void Fall() {
            data.vel.y = -data.jumppower;
        }

        private void UpdateX(float x) {
            MathUtil.Clamp(ref x, -1, 1);
            int colx, coly;
            Vector2 offset = new Vector2(x, 0);
            Tile col = Terrain.CalcFutureCollision(this, offset, out colx, out coly);
            if (col.tileattribs is FluidAttribs) {
                data.mvtState = MovementState.Fluid;
                data.pos.val += offset;
            } else if (col.enumId != TileID.Air) {
                if (x >= 0) {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnEntityCollision(colx, coly, Direction.Right, this);

                    OnTerrainCollision(colx, coly, Direction.Right, col);
                } else {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnEntityCollision(colx, coly, Direction.Left, this);

                    OnTerrainCollision(colx, coly, Direction.Left, col);
                }
            } else {
                data.pos.val += offset;
            }
        }

        private void UpdateY(float y) {
            MathUtil.Clamp(ref y, -1, 1);
            int colx, coly;
            Vector2 offset = new Vector2(0, y);
            Tile col = Terrain.CalcFutureCollision(this, offset, out colx, out coly);

            if (col.tileattribs is FluidAttribs) {
                //collision with fluid
                data.mvtState = MovementState.Fluid;
                data.pos.val += offset;
            } else if (col.enumId != TileID.Air) {
                //collision with solid block
                if (y >= 0) {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnEntityCollision(colx, coly, Direction.Up, this);

                    OnTerrainCollision(colx, coly, Direction.Up, col);
                    data.mvtState = MovementState.Air;
                } else {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnEntityCollision(colx, coly, Direction.Down, this);

                    OnTerrainCollision(colx, coly, Direction.Down, col);
                    data.mvtState = MovementState.Ground;
                }
            } else {
                //no collision
                data.pos.val += offset;
                data.mvtState = MovementState.Air;
            }

        }

        public void UpdatePosition() {

            Vector2i gridPrev = EntityManager.GetGridArray(this);
            data.vel.y -= data.grav * GameTime.DeltaTime;
            data.vel.x *= (float)Math.Pow(data.airResis, GameTime.DeltaTime);

            UpdateX(data.vel.x * GameTime.DeltaTime);
            UpdateY(data.vel.y * GameTime.DeltaTime);

            Vector2i gridNow = EntityManager.GetGridArray(this);
            if (gridPrev != gridNow) {
                //recalc grid array
                EntityManager.EntityGrid[gridPrev.x, gridPrev.y].Remove(this);
                EntityManager.EntityGrid[gridNow.x, gridNow.y].Add(this);
            }

        }


        #endregion

        #region Collisions

        /// <summary>
        /// Returns true if the current entity is colliding with a non-air tile.
        /// </summary>
        /// <returns></returns>
        public bool Colliding() {
            return Terrain.CalcCollision(this).enumId != TileID.Air;
        }

        public Tile CalcTerrainCollision() {
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
