using System;
using System.Collections.Generic;
using OpenGL;
using Tao.FreeGlut;
using System.Diagnostics;
using Game.Entities;

namespace Game {
    static class GameLogic {
        public static List<Entity> Entities { get; private set; } = new List<Entity>();
        private static List<Entity> BatchEntities = new List<Entity>();
        private static List<Entity> BatchRemoveEntities = new List<Entity>();

        public static void Init() {
            Terrain.Init();
            Player.Init();
            Renderer.Init();
            Healthbar.Init(20);
            for (int i = 1;i <= 1; i++) {
                AddEntity(new Shooter(new Vector2(i*20, 0), 50, 300));
            }
            
        }

        public static void Update() {
            //new entities to be added to the world
            Entities.AddRange(BatchEntities);
            BatchEntities.Clear();
            foreach (Entity entity in BatchRemoveEntities) {
                Entities.Remove(entity);
            }
            BatchRemoveEntities.Clear();

            //update the player movement
            Player.Instance.Update();
            Player.Heal(0.0005f);

            //update entities
            foreach (Entity entity in Entities) {
                entity.Update();
            }

            //update terrain
            Terrain.Update();

        }

        public static void AddEntity(Entity entity) {
            BatchEntities.Add(entity);
        }

        public static void RemoveEntity(Entity entity) {
            BatchRemoveEntities.Add(entity);
        }

        public static void RemoveAllEntities() {
            Entities.Clear();
        }

        public static void Render() {
            Renderer.Render();
        }

        public static void Deinit() {
            Update();
            foreach (Entity entity in Entities) {
                entity.Model.Dispose();
            }
            Entities.Clear();
        }
    }
}
