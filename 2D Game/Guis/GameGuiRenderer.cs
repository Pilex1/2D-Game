using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using Game.TitleScreen;
using Game.Core;

namespace Game.Interaction {
   static class GameGuiRenderer  {

        public static void Init() {
            Inventory.Init();
            Hotbar.Init();
            Hotbar.Update();
            Healthbar.Init(Player.Instance.MaxHealth);
        }

        public static void Render() {
            Gl.LineWidth(7);

            Gl.UseProgram(Gui.shader.ProgramID);

            //healthbar
            RenderInstance(new Vector2(((2 - Healthbar.BarWidth) / 2) - 1, 0.01 + Hotbar.SizeY*2 - 1), new Vector2(1, 1), new Vector3(1, 1, 1), Healthbar.texture, Healthbar.vao, BeginMode.TriangleStrip);

            //inventory and hotbar
            RenderInstance(new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2 + Hotbar.CurSelectedSlot * Hotbar.SizeX) - 1, -1), new Vector2(1, 1), new Vector3(1, 1, 1), Hotbar.CurSelectedTexture, Hotbar.CurSelected, BeginMode.LineLoop);
           RenderInstance(new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -1), new Vector2(1, 1), new Vector3(1, 1, 1), Hotbar.TexturedItemsTexture, Hotbar.TexturedItems, BeginMode.Triangles);
            RenderInstance(new Vector2(((2 - Inventory.InvColumns * Hotbar.SizeX) / 2) - 1, -1), new Vector2(1, 1), new Vector3(1, 1, 1), Hotbar.FrameTexture, Hotbar.Frame, BeginMode.Lines);

            Gl.UseProgram(0);

            Gl.LineWidth(1);
        }

        private static void RenderInstance(Vector2 position, Vector2 size, Vector3 colour, Texture texture, GuiVAO vao, BeginMode drawMode) {
            ShaderProgram shader = Gui.shader;
            shader["position"].SetValue(position);
            shader["size"].SetValue(size);
            shader["colour"].SetValue(colour);
            Gl.BindVertexArray(vao.ID);
            Gl.BindTexture(texture.TextureTarget, texture.TextureID);
            Gl.DrawElements(drawMode, vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(texture.TextureTarget, 0);
            Gl.BindVertexArray(0);
        }

        public static void Dispose() {
            Inventory.Dispose();
            Hotbar.Dispose();
            Healthbar.Dispose();
        }
    }
}
