using Game.Assets;
using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core {

    class EntityModel {

        public EntityVAO vao;
        public bool blend;
        public BeginMode drawmode;
        public Vector2 size;

        private const int EntityTextureSize = 16;

        public EntityModel(EntityVAO vao, BeginMode drawmode, Vector2 size, bool blend = false) {
            this.vao = vao;
            this.drawmode = drawmode;
            this.size = size;
            this.blend = blend;
        }

        public void DisposeAll() {
            vao.DisposeAll();
        }

        public static EntityModel CreateRectangle(Vector2 size, EntityID texid) {
            Vector2[] vertices = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            int[] elements = new int[] { 0, 1, 2, 3, 0 };
            Vector2[] uvs;
            int t = (int)texid;
            if (t < 128) {
                //16 x 16
                float x = ((float)(t % EntityTextureSize)) / EntityTextureSize;
                float y = ((float)(t / EntityTextureSize)) / EntityTextureSize;
                float s = 1f / EntityTextureSize;
                float h = 1f / (EntityTextureSize * EntityTextureSize * 2);
                uvs = new Vector2[] { new Vector2(x + h, y + s - h), new Vector2(x + h, y + h), new Vector2(x + s - h, y + h), new Vector2(x + s - h, y + s - h) };
            } else {
                //16 x 32
                float x = ((float)(t % EntityTextureSize)) / EntityTextureSize;
                float y = ((float)(t / EntityTextureSize)) / EntityTextureSize;
                float s = 1f / EntityTextureSize;
                float h = 1f / (EntityTextureSize * EntityTextureSize * 2);
                uvs = new Vector2[] { new Vector2(x + h, y + 2 * s - h), new Vector2(x + h, y + h), new Vector2(x + s - h, y + h), new Vector2(x + s - h, y + 2 * s - h) };
            }
            EntityVAO vao = new EntityVAO(vertices, elements, uvs);
            return new EntityModel(vao, BeginMode.TriangleStrip, size);
        }
    }

    class GuiModel {
        public GuiVAO vao;
        public Texture texture;
        public BeginMode drawmode;
        public Vector2 size;

        public GuiModel(GuiVAO vao, Texture texture, BeginMode drawmode, Vector2 size) {
            this.vao = vao;
            this.texture = texture;
            this.drawmode = drawmode;
            this.size = size;
        }

        public void DisposeVao() {
            vao.DisposeAll();
        }

        public void DisposeTexture() {
            texture.Dispose();
        }

        public void DisposeAll() {
            DisposeVao();
            DisposeTexture();
        }

        public static GuiModel CreateWireRectangleTopLeft(Vector2 size, Color colour, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            int[] elements = new int[] { 0, 1, 2, 3, 0 };
            Vector2[] uvs = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            GuiVAO vao = new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
            return new GuiModel(vao, TextureUtil.CreateTexture(colour), BeginMode.LineLoop, size);
        }

        public static GuiModel CreateRectangleTopLeft(Vector2 size, Texture texture, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            int[] elements = new int[] { 0, 1, 2, 3, 0 };
            Vector2[] uvs = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            GuiVAO vao = new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
            return new GuiModel(vao, texture, BeginMode.TriangleStrip, size);
        }

        public static GuiModel CreateRectangleTopLeft(Vector2 size, Color colour, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            return CreateRectangleTopLeft(size, TextureUtil.CreateTexture(colour), verticeshint, uvhint);
        }

        public static GuiModel CreateWireRectangle(Vector2 size, Color colour, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] { new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1) };
            int[] elements = new int[] { 0, 1, 2, 3, 0 };
            Vector2[] uvs = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            GuiVAO vao = new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
            return new GuiModel(vao, TextureUtil.CreateTexture(colour), BeginMode.LineLoop, size);
        }

        public static GuiModel CreateRectangle(Vector2 size, Texture texture, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] { new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1) };
            int[] elements = new int[] { 0, 1, 2, 3, 0 };
            Vector2[] uvs = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            GuiVAO vao = new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
            return new GuiModel(vao, texture, BeginMode.TriangleStrip, size);
        }

        public static GuiModel CreateRectangle(Vector2 size, Color colour, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            return CreateRectangle(size, TextureUtil.CreateTexture(colour), verticeshint, uvhint);
        }
    }
}
