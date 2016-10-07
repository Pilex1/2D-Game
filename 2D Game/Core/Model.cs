using Game.Util;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core {
    class GuiModel : IDisposable {
        public GuiVAO vao;
        public Texture texture;
        public BeginMode drawmode;

        public GuiModel(GuiVAO vao, Texture texture, BeginMode drawmode) {
            this.vao = vao;
            this.texture = texture;
            this.drawmode = drawmode;
        }

        public void Dispose() {
            vao.Dispose();
            texture.Dispose();
        }

        public static GuiModel CreateWireRectangle(Vector2 size, Color colour, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,size.y),
                new Vector2(size.x,size.y),
                new Vector2(size.x,0)
            };
            int[] elements = new int[] {
               0,1,2,3,0
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1)
            };
            GuiVAO vao = new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
            return new GuiModel(vao, TextureUtil.CreateTexture(colour), BeginMode.LineLoop);
        }

        public static GuiModel CreateRectangle(Vector2 size, Texture texture, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            Vector2[] vertices = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,size.y),
                new Vector2(size.x,size.y),
                new Vector2(size.x,0)
            };
            int[] elements = new int[] {
                0,1,2,0,3
            };
            Vector2[] uvs = new Vector2[] {
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1)
            };
            GuiVAO vao = new GuiVAO(vertices, elements, uvs, verticeshint, uvhint);
            return new GuiModel(vao, texture, BeginMode.TriangleStrip);
        }

        public static GuiModel CreateRectangle(Vector2 size, Color colour, BufferUsageHint verticeshint = BufferUsageHint.StaticDraw, BufferUsageHint uvhint = BufferUsageHint.StaticDraw) {
            return CreateRectangle(size, TextureUtil.CreateTexture(colour), verticeshint, uvhint);
        }
    }
}
