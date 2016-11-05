using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
    class ShaderProgram {

        public uint vertexShader { get; private set; }
        public uint fragmentShader { get; private set; }

        public uint ID { get; private set; }

        private Dictionary<string, int> UniformLocations;

        public ShaderProgram(string vertfile, string fragfile) {

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, LoadFromFile(vertfile));
            GL.ShaderSource(fragmentShader, LoadFromFile(fragfile));

            GL.CompileShader(vertexShader);
            string vertexlog = GL.GetShaderInfoLog((int)vertexShader);
            if (vertexlog != "") {
                Debug.WriteLine("Vertex shader [" + vertfile + "] did not compile successfully:");
                Debug.WriteLine(vertexlog);
            }

            GL.CompileShader(fragmentShader);
            string fragmentlog = GL.GetShaderInfoLog((int)fragmentShader);
            if (fragmentlog != "") {
                Debug.WriteLine("Fragment shader [" + fragfile + "] did not compile successfully:");
                Debug.WriteLine(fragmentlog);
            }

            ID = GL.CreateProgram();

            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);

            GL.LinkProgram(ID);
            GL.ValidateProgram(ID);

            string programlog = GL.GetShaderInfoLog((int)ID);
            if (programlog != "") {
                Debug.WriteLine("Program did not compile successfully:");
                Debug.WriteLine(programlog);
            }

            UniformLocations = new Dictionary<string, int>();
        }

        public void AddUniform(string s) {
            int uniformLocation = GL.GetUniformLocation(ID, s);
            if (uniformLocation == -1)
                Debug.WriteLine(String.Format("Uniform name not used or not found: {0}", s));
            UniformLocations[s] = GL.GetUniformLocation(ID, s);
        }

        public void SetUniform1b(string s, bool b) {
            GL.Uniform1(GetUniform(s), b ? 1 : 0);
        }
        public void SetUniform1i(string s, int vec) {
            GL.Uniform1(GetUniform(s), vec);
        }
        public void SetUniform1f(string s, float vec) {
            GL.Uniform1(GetUniform(s), vec);
        }
        public void SetUniform1d(string s, double vec) {
            GL.Uniform1(GetUniform(s), vec);
        }
        public void SetUniform2f(string s, Vector2 vec) {
            GL.Uniform2(GetUniform(s), vec);
        }
        public void SetUniform2d(string s, Vector2d vec) {
            GL.Uniform2(GetUniform(s), vec.x, vec.y);
        }
        public void SetUniform3f(string s, Vector3 vec) {
            GL.Uniform3(GetUniform(s), vec);
        }
        public void SetUniform4f(string s, Vector4 vec) {
            GL.Uniform4(GetUniform(s), vec);
        }
        public void SetUniform4c(string s, Color4 vec) {
            GL.Uniform4(GetUniform(s), vec);
        }
        public void SetUniform4m(string s, Matrix mat) {
            GL.UniformMatrix4(GetUniform(s), false, ref mat);
        }

        private int GetUniform(string s) {
            return UniformLocations[s];
        }

        private static string LoadFromFile(string source) {
            StreamReader reader = new StreamReader(source);
            StringBuilder sb = new StringBuilder();
            string s;
            while ((s = reader.ReadLine()) != null) {
                sb.Append(s);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
