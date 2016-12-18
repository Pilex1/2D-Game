using System;
using OpenGL;
using Game.Terrains;
using System.Diagnostics;
using System.Collections.Generic;
using Game.Core;
using Game.Util;
using Game.Particles;
using System.Drawing;

namespace Game.Entities {

    enum EntityID {
        //16 x 16
        None = 0, ShooterProjectile, ParticlePurple, ParticleRed, ParticleGreen, ParticleBlue, Squisher, ParticleYellow,

        //16 x 32
        Shooter = 128, Player, PlayerSimple
    }

    [Serializable]
    class EntityData {

        public const float maxRecentDmgTime = 10;
        [NonSerialized]
        public float recentDmg = 0;

        public float speed = 0;
        public float rot = 0;
        public bool useGravity = true;
        public float grav = 0.02f;
        public float airResis = 0.9f;
        public float jumppower = 0;
        public bool InAir = false;
        public bool calcTerrainCollisions = true;
        public BoundedFloat life = new BoundedFloat(0, 0, 0);
        public BoundedVector2 vel = new BoundedVector2(new BoundedFloat(0, -EntityManager.maxHorzSpeed, EntityManager.maxHorzSpeed), new BoundedFloat(0, -EntityManager.maxVertSpeed, EntityManager.maxVertSpeed));
        public BoundedVector2 pos = new BoundedVector2(new BoundedFloat(0, 0, Terrain.Tiles.GetLength(0) - 1), new BoundedFloat(0, 0, Terrain.Tiles.GetLength(1) - 1));
        public Vector4 colour = new Vector4(1, 1, 1, 1);

        public EntityID entityId = EntityID.None;
    }

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

        public void MoveLeft() {
            data.vel.x -= data.speed * GameTime.DeltaTime;
        }
        public void MoveRight() {
            data.vel.x += data.speed * GameTime.DeltaTime;
        }
        public void Jump() {
            if (data.useGravity) {
                if (!data.InAir) {
                    data.vel.y = data.jumppower;
                }
            } else {
                data.vel.y = data.jumppower;
            }
        }
        public void Fall() {
            if (!data.useGravity) {
                data.vel.y = -data.jumppower;
            }
        }

        private void UpdateX(float x) {
            Tile col;
            int colx, coly;
            Vector2 offset = new Vector2(x, 0);
            if (Terrain.WillCollide(this, offset, out col, out colx, out coly)) {
                if (x > 0) {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnTerrainIntersect(colx, coly, Direction.Right, this);

                    OnTerrainCollision(colx, coly, Direction.Right, col);
                } else if (x < 0) {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnTerrainIntersect(colx, coly, Direction.Left, this);

                    OnTerrainCollision(colx, coly, Direction.Left, col);
                }
            } else {
                data.pos.val += offset;
            }
        }

        private void UpdateY(float y) {
            Tile col;
            int colx, coly;
            Vector2 offset = new Vector2(0, y);
            if (Terrain.WillCollide(this, offset, out col, out colx, out coly)) {
                if (y > 0) {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnTerrainIntersect(colx, coly, Direction.Up, this);

                    OnTerrainCollision(colx, coly, Direction.Up, col);
                } else if (y < 0) {
                    if (data.calcTerrainCollisions)
                        col.tileattribs.OnTerrainIntersect(colx, coly, Direction.Down, this);

                    OnTerrainCollision(colx, coly, Direction.Down, col);
                }
            } else {
                data.pos.val += offset;
                if (y != 0)
                    data.InAir = true;
            }
        }

        public void UpdatePosition() {

            Vector2i gridPrev = EntityManager.GetGridArray(this);
            if (data.useGravity)
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
        public void HealFull() {
            data.life.Fill();
        }

        public void Damage(float dmg) {
            data.life.val -= dmg;
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

        public virtual Matrix4 ModelMatrix() {
            Vector2 size = Assets.Models.GetModel(entityId).size;
            return Matrix4.CreateScaling(new Vector3(size.x, size.y, 0)) * Matrix4.CreateRotationZ(data.rot) * Matrix4.CreateTranslation(new Vector3(data.pos.x, data.pos.y, 0));
        }

        public abstract void InitTimers();
        public abstract void Update();
        public abstract void UpdateHitbox();
        public virtual void OnTerrainCollision(int x, int y, Direction d, Tile t) { }
        public virtual void OnDeath() {
            EntityManager.RemoveEntity(this);
        }
    }
}
