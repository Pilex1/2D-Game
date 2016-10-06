using System;
using System.Diagnostics;
using Tao.FreeGlut;
using OpenGL;
using Game.Entities;
using Game.Interaction;
using Game.Assets;
using Game.Terrains;
using Game.Util;
using Game.Core;

namespace Game {

    [Serializable]
    class PlayerData {
        public BoolSwitch flying;
        public float maxHealth;
        public EntityData entitydata;
    }

    class Player : Entity {
        public const int StartX = 580, StartY = 70;

        public BoolSwitch Flying { get; private set; } = new BoolSwitch(true, 20);
        public float MaxHealth { get; private set; } = 20;

        public static Player Instance { get; private set; }

        private Player(EntityVAO vao, Texture texture, Hitbox hitbox, Vector2 position) : base(vao, texture, hitbox, position) {
            base.data.speed = 0.08f;
            base.data.jumppower = 0.5f;
        }

        public static PlayerData ToPlayerData() {
            return new PlayerData { flying = Instance.Flying, maxHealth = Instance.MaxHealth, entitydata = Instance.data };
        }

        private static Player DefaultPlayer() {
            Vector2 position = new Vector2(StartX, StartY);
            EntityVAO vao = EntityVAO.CreateRectangle(new Vector2(1, 2));
            Texture texture = TextureUtil.CreateTexture(new Vector3[,] {
              {new Vector3(1, 0, 0), new Vector3(0, 1, 0)},
              {new Vector3(0, 0, 1), new Vector3(1, 0, 1)}
            });
            Gl.BindTexture(texture.TextureTarget, texture.TextureID);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(TextureTarget.Texture2D, 0);
            Hitbox hitbox = new RectangularHitbox(position, new Vector2(1, 2));
            return new Player(vao, texture, hitbox, position);
        }

        public static new void Init() {
            Instance = DefaultPlayer();

            try {
                PlayerData playerdata = Serialization.LoadPlayer();
                Instance.Flying = playerdata.flying;
                Instance.MaxHealth = playerdata.maxHealth;
                Instance.data = playerdata.entitydata;
            } catch (Exception) { }

            Instance.CorrectTerrainCollision();

            Healthbar.Init(20);
            Inventory.Init();
        }

        public static new void CleanUp() {
            Serialization.SavePlayer(ToPlayerData());
        }

        public override void Update() {
            bool[] Keys = Input.Keys;
            bool[] Mouse = Input.Mouse;
            int dir = Input.MouseScroll;
            int MouseX = Input.MouseX, MouseY = Input.MouseY;

            if (Keys['a']) {
                Instance.MoveLeft();
            }
            if (Keys['d']) {
                Instance.MoveRight();
            }
            if (Keys['w']) {
                Instance.Jump();
            }
            if (Flying) {
                Instance.data.UseGravity = false;
                if (Keys['s']) Instance.Fall();
            } else {
                Instance.data.UseGravity = true;
            }

            if (Instance.UpdatePosition()) {
                Terrain.UpdateMesh = true;
            }

            if (Keys['l']) {
                Terrain.UpdateLighting.Toggle();
                Terrain.UpdateMesh = true;
            }
            if (Keys['f']) {
                Flying.Toggle();
            }


            if (Keys['1']) Hotbar.CurSelectedSlot = 0;
            if (Keys['2']) Hotbar.CurSelectedSlot = 1;
            if (Keys['3']) Hotbar.CurSelectedSlot = 2;
            if (Keys['4']) Hotbar.CurSelectedSlot = 3;
            if (Keys['5']) Hotbar.CurSelectedSlot = 4;
            if (Keys['6']) Hotbar.CurSelectedSlot = 5;
            if (Keys['7']) Hotbar.CurSelectedSlot = 6;
            if (Keys['8']) Hotbar.CurSelectedSlot = 7;
            if (Keys['9']) Hotbar.CurSelectedSlot = 8;


            if (Mouse[Input.MouseLeft]) {
                Vector2 v = RayCast(MouseX, MouseY);
                Terrain.BreakTile((int)v.x, (int)v.y);
            }
            if (Mouse[Input.MouseRight]) {
                Vector2 v = RayCast(MouseX, MouseY);
                int x = (int)v.x, y = (int)v.y;
                if (Terrain.TileAt(x, y).enumId == TileEnum.Air) {
                    ItemInteract.Interact(Hotbar.CurrentlySelectedItem(), x, y);
                } else {
                    Terrain.TileAt(x, y).tileattribs.Interact(x, y);
                }
            }
            if (Mouse[Input.MouseMiddle]) {
                Vector2 v = RayCast(MouseX, MouseY);
                int x = (int)v.x, y = (int)v.y;
                TileEnum tile = Terrain.TileAt(x, y).enumId;

            }

            if (dir < 0) Hotbar.IncrSlot();
            if (dir > 0) Hotbar.DecrSlot();


            Hitbox.Position = data.Position.val;

        }

        public static void Damage(float hp) {
            Healthbar.Damage(hp);
        }

        public static void Heal(float hp) {
            Healthbar.Heal(hp);
        }


        private static Vector2 RayCast(int mx, int my) {
            float x = (2.0f * mx) / Program.Width - 1.0f;
            float y = 1.0f - (2.0f * my) / Program.Height;
            Vector2 normalizedCoords = new Vector2(x, y);
            Vector4 clipCoords = new Vector4(normalizedCoords.x, normalizedCoords.y, -1, 1);
            Vector4 eyeCoords = GameRenderer.projectionMatrix.Inverse() * clipCoords;
            eyeCoords.z = -1;
            eyeCoords.w = 0;
            Matrix4 inverseViewMatrix = GameRenderer.viewMatrix.Inverse();
            Vector4 rayWorldTemp = inverseViewMatrix * eyeCoords;
            Vector2 rayWorld = new Vector2(rayWorldTemp.x, rayWorldTemp.y);

            Vector2 intersectTerrain = Instance.data.Position.val - rayWorld * GameRenderer.zoom - new Vector2(0, 1);
            return intersectTerrain;
        }

        public static Vector2 ToPlayer(Vector2 pos) { return new Vector2(Instance.data.Position.x - pos.x, Instance.data.Position.y - pos.y).Normalize(); }

        public static bool InRange(Entity entity, float maxDist) {
            float x = entity.data.Position.x, y = entity.data.Position.y;
            return (Instance.data.Position.x - x) * (Instance.data.Position.x - x) + (Instance.data.Position.y - y) * (Instance.data.Position.y - y) <= maxDist;
        }

        public static bool Intersecting(Entity entity) {
            return Instance.Hitbox.Intersecting(entity.Hitbox);
        }


    }
}
