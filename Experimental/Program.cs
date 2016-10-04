using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenGL;
using Tao.FreeGlut;

namespace Experimental {
    static class Program {
        public static int ScreenWidth, ScreenHeight;
        public const int Width = 1600, Height = 900;

        static CustomVAO vao;
        static ShaderProgram program;

        static void Main() {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(Width, Height);

            ScreenWidth = Glut.glutGet(Glut.GLUT_SCREEN_WIDTH);
            ScreenHeight = Glut.glutGet(Glut.GLUT_SCREEN_HEIGHT);

            Glut.glutInitWindowPosition((ScreenWidth - Width) / 2, (ScreenHeight - Height) / 2);
            Glut.glutCreateWindow("");
            //Glut.glutGameModeString(Width+"x"+Height+":32@60");

            Glut.glutSetOption(Glut.GLUT_ACTION_ON_WINDOW_CLOSE, Glut.GLUT_ACTION_CONTINUE_EXECUTION);
            Gl.Viewport(0, 0, Width, Height);
            Glut.glutDisplayFunc(delegate () { });
            Glut.glutIdleFunc(MainGameLoop);

            Init();

            Glut.glutMainLoop();
        }

        private static void Init() {

            Gl.ClearColor(0, 1, 1, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            program = new ShaderProgram(
                @"
#version 450 core;
in vec3 vpos;
out vec3 fpos;

void main(void) {
    gl_Position = vec4(vpos, 1.0);
    fpos=vpos;
}                 

",
                @"

#version 450 core;

in vec3 fpos;
out vec4 frag;

void main(void) {
    frag = vec4(fpos, 1.0);
}

");
            Vector3[] vertices = new Vector3[] {
                new Vector3(-1,1,0),
                new Vector3(-1,-1,0),
                new Vector3(1,-1,0),
                new Vector3(1,1,0)
            };
            int[] elements = new int[] {
                0,1,2, 2, 3, 0
            };
            vao = new CustomVAO(program, vertices, "vpos", elements);
        }

        private static void MainGameLoop() {
            Gl.UseProgram(program.ProgramID);
            Gl.BindVertexArray(vao.vaoID);
            Gl.DrawElements(BeginMode.Triangles, vao.count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }
    }
}
