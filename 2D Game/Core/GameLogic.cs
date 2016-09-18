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
        private static float ActualDeltaTime;
        private static float PrevTime;
        private static float CountTime;
        private static int Count;

        public static void Init() {
            Entities = new List<Entity>();

            Terrain.Init();
            Player.Init();
            GameRenderer.Init();
            for (int i = 1;i <= 1; i++) {
             //  AddEntity(new Shooter(new Vector2(475, 0), 50, 150));
            }
            
        }

        public static void Update() {
            Watch.Stop();
            ActualDeltaTime = (Watch.ElapsedMilliseconds - PrevTime) / 1000;
            PrevTime = Watch.ElapsedMilliseconds;
            Watch.Start();

            DeltaTime = ActualDeltaTime * 60;
            CountTime += ActualDeltaTime;
            Count++;
            int interval = 1;
            if (CountTime > 1f/ interval) {
                Glut.glutSetWindowTitle("Plexico 2D Game - Copyright Alex Tan 2016 FPS: " + (Count* interval).ToString());
                CountTime = 0;
                Count = 0;
            }

            //new entities to be added to the world
            Entities.AddRange(BatchEntities);
            BatchEntities.Clear();
            foreach (Entity entity in BatchRemoveEntities) {
                Entities.Remove(entity);
            }
            BatchRemoveEntities.Clear();

            //update the player movement
            Player.Instance.Update();
            Player.Heal(0.0005f*DeltaTime);
            //Debug.WriteLine(Player.Instance.Position);

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
            GameRenderer.Render();
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
