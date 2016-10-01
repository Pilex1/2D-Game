using System;
using System.Collections.Generic;
using OpenGL;
using Tao.FreeGlut;
using System.Diagnostics;
using Game.Entities;
using Game.Terrains;
using Game.Core;

namespace Game {
    static class GameLogic {
        public static HashSet<Entity> Entities { get; private set; }
        private static HashSet<Entity> BatchEntities = new HashSet<Entity>();
        private static HashSet<Entity> BatchRemoveEntities = new HashSet<Entity>();

        private static Stopwatch Watch = new Stopwatch();
        public static float DeltaTime { get; private set; }
        private static float ActualDeltaTime;
        private static float PrevTime;
        private static float CountTime;
        private static int Count;

        public static void Init() {
            Entities = new HashSet<Entity>();

            Terrain.Init();
            Player.Init();
            GameRenderer.Init();
            for (int i = 1; i <= 1; i++) {
                //  AddEntity(new Shooter(new Vector2(475, 0), 50, 150));
            }
            Background.Init();
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
            if (CountTime > 1f / interval) {
                Glut.glutSetWindowTitle("Plexico 2D Game - Copyright Alex Tan 2016 FPS: " + (Count * interval).ToString());
                CountTime = 0;
                Count = 0;
            }

            //new entities to be added to the world
            foreach (Entity e in BatchEntities) Entities.Add(e);
            BatchEntities.Clear();

            foreach (Entity e in BatchRemoveEntities) Entities.Remove(e);
            BatchRemoveEntities.Clear();



            //update the player movement
            Player.Instance.Update();
            Player.Heal(0.0005f * DeltaTime);
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

        public static void CleanUp() {
            Update();
            foreach (Entity entity in Entities) {
                entity.Model.CleanUp();
            }
            Entities.Clear();
        }
    }
}
