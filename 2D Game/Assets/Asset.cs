using Game.Util;
using OpenGL;
using System;

namespace Game.Assets {
    static class Asset {

        #region Textures
        private const string _file_TerrainTexture = "Assets/TerrainTextures.png";
        private const string _file_ItemTexture = "Assets/ItemTextures.png";
        private const string _file_FluidTexture = "Assets/FluidTextures.png";
        private const string _file_EntityTexture = "Assets/EntityTextures.png";

        private static Texture _itemTexture;
        public static Texture ItemTexture {
            get {
                if (_itemTexture == null) {
                    _itemTexture = new Texture(_file_ItemTexture);
                    SetTextureParamsNearest(_itemTexture);
                }
                return _itemTexture;
            }
        }

        private static Texture _fluidTexture;
        public static Texture FluidTexture {
            get {
                if (_fluidTexture == null) {
                    _fluidTexture = new Texture(_file_FluidTexture);
                    SetTextureParamsNearest(_fluidTexture);
                }
                return _fluidTexture;
            }
        }

        private static Texture _terrainTexture;
        public static Texture TerrainTexture {
            get {
                if (_terrainTexture == null) {
                    _terrainTexture = new Texture(_file_TerrainTexture);
                    SetTextureParamsNearest(_terrainTexture);
                }
                return _terrainTexture;
            }
        }

        private static Texture _entitytexture;
        public static Texture Texture {
            get {
                if (_entitytexture == null) {
                    _entitytexture = new Texture(_file_EntityTexture);
                    SetTextureParamsNearest(_entitytexture);
                }
                return _entitytexture;
            }
        }

        private static void SetTextureParamsLinear(Texture tex) {
            Gl.BindTexture(tex.TextureTarget, tex.TextureID);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(tex.TextureTarget, 0);
        }

        private static void SetTextureParamsNearest(Texture tex) {
            Gl.BindTexture(tex.TextureTarget, tex.TextureID);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(tex.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.BindTexture(tex.TextureTarget, 0);
        }
        #endregion

        #region Shaders
        private const string _file_EntityFrag = "Shaders/Entity.frag";
        private const string _file_EntityVert = "Shaders/Entity.vert";
        private const string _file_GuiFrag = "Shaders/Gui.frag";
        private const string _file_GuiVert = "Shaders/Gui.vert";
        private const string _file_TerrainFrag = "Shaders/Terrain.frag";
        private const string _file_TerrainVert = "Shaders/Terrain.vert";

        public static string EntityFrag { get { return FileUtil.LoadFile(_file_EntityFrag); } }
        public static string EntityVert { get { return FileUtil.LoadFile(_file_EntityVert); } }
        public static string GuiFrag { get { return FileUtil.LoadFile(_file_GuiFrag); } }
        public static string GuiVert { get { return FileUtil.LoadFile(_file_GuiVert); } }
        public static string TerrainFrag { get { return FileUtil.LoadFile(_file_TerrainFrag); } }
        public static string TerrainVert { get { return FileUtil.LoadFile(_file_TerrainVert); } }

        #endregion
    }
}
