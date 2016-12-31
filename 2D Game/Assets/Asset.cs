using Game.Core;
using Game.Util;
using OpenGL;
using Game.Entities;

namespace Game.Assets {

    static class Textures {
        public static Texture ItemTexture, TerrainTexture, EntityTexture;
        public static Texture ButtonTex, LabelTex, TextboxTex;
        public static Texture TitleBackgroundTex, GameBackgroundTex, DesertBackgroundTex, NightBackgroundTex;
        public static Texture FontCenturyGothicTex, FontChillerTex, FontDialogInputTex, FontLucidaConsoleTex;
        public static Texture HealthbarTexture;

        internal static void Init() {
            ButtonTex = TextureUtil.CreateTexture("Assets/Textures/Button.png", TextureUtil.TextureInterp.Nearest);
            LabelTex = TextureUtil.CreateTexture("Assets/Textures/Label.png", TextureUtil.TextureInterp.Nearest);
            TextboxTex = TextureUtil.CreateTexture("Assets/Textures/Textbox.png", TextureUtil.TextureInterp.Nearest);

            GameBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/GameBackground.png", TextureUtil.TextureInterp.Linear);
            DesertBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/DesertBackground.png", TextureUtil.TextureInterp.Linear);
            NightBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/NightBackground.png", TextureUtil.TextureInterp.Linear);

            TitleBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/TitleScreenBackground2.png", TextureUtil.TextureInterp.Linear);

            FontCenturyGothicTex = TextureUtil.CreateTexture("Assets/Fonts/CenturyGothic.png", TextureUtil.TextureInterp.Nearest);
            FontChillerTex = TextureUtil.CreateTexture("Assets/Fonts/Chiller.png", TextureUtil.TextureInterp.Nearest);
            FontDialogInputTex = TextureUtil.CreateTexture("Assets/Fonts/DialogInput.png", TextureUtil.TextureInterp.Nearest);
            FontLucidaConsoleTex = TextureUtil.CreateTexture("Assets/Fonts/LucidaConsole.png", TextureUtil.TextureInterp.Nearest);

            ItemTexture = TextureUtil.CreateTexture("Assets/Textures/ItemTextures.png", TextureUtil.TextureInterp.Nearest);
            TerrainTexture = TextureUtil.CreateTexture("Assets/Textures/TerrainTextures.png", TextureUtil.TextureInterp.Nearest);
            EntityTexture = TextureUtil.CreateTexture("Assets/Textures/EntityTextures.png", TextureUtil.TextureInterp.Nearest);
            HealthbarTexture = TextureUtil.CreateTexture("Assets/Textures/Healthbar.png", TextureUtil.TextureInterp.Nearest);
        }

        internal static void Dispose() {
            ItemTexture.Dispose();
            TerrainTexture.Dispose();
            EntityTexture.Dispose();

            ButtonTex.Dispose();
            LabelTex.Dispose();
            TextboxTex.Dispose();

            GameBackgroundTex.Dispose();
            DesertBackgroundTex.Dispose();
            NightBackgroundTex.Dispose();
            TitleBackgroundTex.Dispose();

            FontCenturyGothicTex.Dispose();
            FontChillerTex.Dispose();
            FontDialogInputTex.Dispose();
            FontLucidaConsoleTex.Dispose();
        }
    }

    static class Fonts {
        public static string FontCenturyGothicFnt, FontChillerFnt, FontDialogInputFnt, FontLucidaConsoleFnt;

        internal static void Init() {
            FontCenturyGothicFnt = FileUtil.LoadFile("Assets/Fonts/CenturyGothic.fnt");
            FontChillerFnt = FileUtil.LoadFile("Assets/Fonts/Chiller.fnt");
            FontDialogInputFnt = FileUtil.LoadFile("Assets/Fonts/DialogInput.fnt");
            FontLucidaConsoleFnt = FileUtil.LoadFile("Assets/Fonts/LucidaConsole.fnt");
        }
    }

    static class Shaders {
        public static string EntityFrag, EntityVert, GuiFrag, GuiVert, TerrainFrag, TerrainVert;

        internal static void Init() {
            EntityFrag = FileUtil.LoadFile("Assets/Shaders/Entity.frag");
            EntityVert = FileUtil.LoadFile("Assets/Shaders/Entity.vert");
            GuiFrag = FileUtil.LoadFile("Assets/Shaders/Gui.frag");
            GuiVert = FileUtil.LoadFile("Assets/Shaders/Gui.vert");
            TerrainFrag = FileUtil.LoadFile("Assets/Shaders/Terrain.frag");
            TerrainVert = FileUtil.LoadFile("Assets/Shaders/Terrain.vert");
        }
    }

    static class Models {

        private static EntityModel[] EntityModels;

        internal static void Init() {
            EntityModels = new EntityModel[255];

            EntityModels[(int)EntityID.BlackOutline] = EntityModel.CreateHitboxRectangle();
            Add(new Vector2(1, 1), EntityID.ShooterProjectile);
            Add(new Vector2(0.5, 0.5), EntityID.ParticlePurple);
            Add(new Vector2(0.5, 0.5), EntityID.ParticleRed);
            Add(new Vector2(0.5, 0.5), EntityID.ParticleGreen);
            Add(new Vector2(0.5, 0.5), EntityID.ParticleBlue);
            Add(new Vector2(0.5, 0.5), EntityID.ParticleYellow);
            Add(new Vector2(1, 1), EntityID.Squisher);
            Add(new Vector2(1, 2), EntityID.Shooter);
            Add(new Vector2(1, 2), EntityID.Player);
            Add(new Vector2(1, 2), EntityID.PlayerSimple);
            Add(new Vector2(1, 2), EntityID.EntityCage);
            Add(new Vector2(1, 1), EntityID.AutoDart);
            Add(new Vector2(1, 2), EntityID.Warder);
            Add(new Vector2(0.4, 0.4), EntityID.WhiteFill);

        }

        private static void Add(Vector2 size, EntityID id) {
            EntityModels[(int)id] = EntityModel.CreateRectangle(size, id);
        }

        public static EntityModel GetModel(EntityID id) {
            return EntityModels[(int)id];
        }
    }

    static class AssetsManager {

        public static void Init() {
            Textures.Init();
            Fonts.Init();
            Shaders.Init();
        }

        public static void InitInGame() {
            Models.Init();
        }

        public static void Dispose() {
            Textures.Dispose();
        }
    }
}
