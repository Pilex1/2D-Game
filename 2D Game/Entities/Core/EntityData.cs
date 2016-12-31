using Game.Terrains.Terrain_Generation;
using Game.Util;
using OpenGL;
using System;

namespace Game.Entities {

    enum MovementState {
        Ground, Air, Fluid
    }

    [Serializable]
    class EntityData {
        [NonSerialized]
        public float recentDmg = 0;
        public bool reboundedX;
        public bool reboundedY;

        public const float maxRecentDmgTime = 10;
        public float speed = 0;
        public float rot = 0;
        public float grav = 0.02f;
        public float airResis = 0.85f;
        public float jumppower = 0;
        public MovementState mvtState = MovementState.Ground;
        public bool calcTerrainCollisions = true;
        public BoundedFloat life = new BoundedFloat(1, 0, 1);
        public BoundedVector2 vel = new BoundedVector2(new BoundedFloat(0, -EntityManager.maxHorzSpeed, EntityManager.maxHorzSpeed), new BoundedFloat(0, -EntityManager.maxVertSpeed, EntityManager.maxVertSpeed));
        public BoundedVector2 pos = new BoundedVector2(new BoundedFloat(0, 0, TerrainGen.SizeX-1), new BoundedFloat(0, 0, TerrainGen.SizeY-1));
        public Vector4 colour = new Vector4(1, 1, 1, 1);
        public bool invulnerable = false;
    }
}
