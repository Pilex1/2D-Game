using Game.Guis;
using Game.Util;
using OpenGL;
using System;

namespace Game.Assets {

    enum EntityTexture {
        ShooterProjectile
    }

    static class Asset {

        public static Texture ItemTexture, FluidTexture, TerrainTexture, EntityTexture;
        public static Texture ButtonTex, LabelTex, TextboxTex;
        public static Texture TitleBackgroundTex, GameBackgroundTex, DesertBackgroundTex, NightBackgroundTex;
        public static Texture FontCenturyGothicTex, FontChillerTex, FontDialogInputTex, FontLucidaConsoleTex;
        public static string FontCenturyGothicFnt, FontChillerFnt, FontDialogInputFnt, FontLucidaConsoleFnt;
        public static string EntityFrag, EntityVert, GuiFrag, GuiVert, TerrainFrag, TerrainVert;

        public static void Init() {
            ItemTexture = TextureUtil.CreateTexture("Assets/Textures/ItemTextures.png", TextureUtil.TextureInterp.Nearest);
            //FluidTexture = TextureUtil.CreateTexture("Assets/Textures/FluidTextures.png", TextureUtil.TextureInterp.Nearest);
            TerrainTexture = TextureUtil.CreateTexture("Assets/Textures/TerrainTextures.png", TextureUtil.TextureInterp.Nearest);
            EntityTexture = TextureUtil.CreateTexture("Assets/Textures/EntityTextures.png", TextureUtil.TextureInterp.Nearest);

            ButtonTex = TextureUtil.CreateTexture("Assets/Textures/Button.png", TextureUtil.TextureInterp.Nearest);
            LabelTex = TextureUtil.CreateTexture("Assets/Textures/Label.png", TextureUtil.TextureInterp.Nearest);
            TextboxTex = TextureUtil.CreateTexture("Assets/Textures/Textbox.png", TextureUtil.TextureInterp.Nearest);

            GameBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/GameBackground.png", TextureUtil.TextureInterp.Linear);
            DesertBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/DesertBackground.png", TextureUtil.TextureInterp.Linear);
            NightBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/NightBackground.png", TextureUtil.TextureInterp.Linear);
            TitleBackgroundTex = TextureUtil.CreateTexture("Assets/Textures/TitleScreenBackground.png", TextureUtil.TextureInterp.Linear);

            FontCenturyGothicTex = TextureUtil.CreateTexture("Assets/Fonts/CenturyGothic.png", TextureUtil.TextureInterp.Nearest);
            FontChillerTex = TextureUtil.CreateTexture("Assets/Fonts/Chiller.png", TextureUtil.TextureInterp.Nearest);
            FontDialogInputTex = TextureUtil.CreateTexture("Assets/Fonts/DialogInput.png", TextureUtil.TextureInterp.Nearest);
            FontLucidaConsoleTex = TextureUtil.CreateTexture("Assets/Fonts/LucidaConsole.png", TextureUtil.TextureInterp.Nearest);

            FontCenturyGothicFnt = FileUtil.LoadFile("Assets/Fonts/CenturyGothic.fnt");
            FontChillerFnt = FileUtil.LoadFile("Assets/Fonts/Chiller.fnt");
            FontDialogInputFnt = FileUtil.LoadFile("Assets/Fonts/DialogInput.fnt");
            FontLucidaConsoleFnt = FileUtil.LoadFile("Assets/Fonts/LucidaConsole.fnt");

            EntityFrag = FileUtil.LoadFile("Assets/Shaders/Entity.frag");
            EntityVert = FileUtil.LoadFile("Assets/Shaders/Entity.vert");
            GuiFrag = FileUtil.LoadFile("Assets/Shaders/Gui.frag");
            GuiVert = FileUtil.LoadFile("Assets/Shaders/Gui.vert");
            TerrainFrag = FileUtil.LoadFile("Assets/Shaders/Terrain.frag");
            TerrainVert = FileUtil.LoadFile("Assets/Shaders/Terrain.vert");
        }
    }
}
