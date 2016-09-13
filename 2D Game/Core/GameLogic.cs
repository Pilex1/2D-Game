using System;
using System.Collections.Generic;
using OpenGL;
using Tao.FreeGlut;
using System.Diagnostics;
using Game.Entities;
using Game.Terrains;

namespace Game {
    static class GameLogic {
        public static List<Entity> Entities { get; private set; }
        private static List<Entity> BatchEntities = new List<Entity>();
        private static List<Entity> BatchRemoveEntities = new List<Entity>();

        private static Stopwatch Watch = new Stopwatch();
        public static float DeltaTime { get; private set; }
        private static float PrevTime;

        public static void Init() {
            Entities = new List<Entity>();

            Terrain.Init();
            Player.Init();
            Renderer.Init();
            for (int i = 1;i <= 1; i++) {
                //AddEntity(new Shooter(new Vector2(520, 0), 50, 300));
            }
            
        }

        public static void Update() {

            Watch.Stop();
            DeltaTime = (Watch.ElapsedMilliseconds - PrevTime) / 1000 * 60;
            PrevTime = Watch.ElapsedMilliseconds;
            Watch.Start();

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
