using Game.Terrains;
using Game.Util;
using OpenGL;
using System;

namespace Game.Entities {

    [Serializable]
    class EntityData {

        public const float maxRecentDmgTime = 10;
        [NonSerialized]
        public float recentDmg = 0;
        public float speed = 0;
        public float rot = 0;
        public float grav = 0.02f;
        public float airResis = 0.85f;
        public float jumppower = 0;
        public bool InAir = false;
        public bool calcTerrainCollisions = true;
        public BoundedFloat life = new BoundedFloat(1, 0, 1);
        public BoundedVector2 vel = new BoundedVector2(new BoundedFloat(0, -EntityManager.maxHorzSpeed, EntityManager.maxHorzSpeed), new BoundedFloat(0, -EntityManager.maxVertSpeed, EntityManager.maxVertSpeed));
        public BoundedVector2 pos = new BoundedVector2(new BoundedFloat(0, 0, Terrain.Tiles.GetLength(0) - 1), new BoundedFloat(0, 0, Terrain.Tiles.GetLength(1) - 1));
        public Vector4 colour = new Vector4(1, 1, 1, 1);
        public bool invulnerable = false;
    }
}
