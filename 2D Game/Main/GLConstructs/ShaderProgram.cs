using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Game.Main.GLConstructs
{
	class ShaderProgram : IDisposable
	{

		public uint VertID { get; private set; }

		public uint FragID { get; private set; }

		public uint ID { get; private set; }

		private Dictionary<string, int> UniformLocations;

		public ShaderProgram (string vertsrc, string fragsrc)
		{

			VertID = GL.CreateShader (ShaderType.VertexShader);
			FragID = GL.CreateShader (ShaderType.FragmentShader);

			GL.ShaderSource (VertID, vertsrc);
			GL.ShaderSource (FragID, fragsrc);

			GL.CompileShader (VertID);
			string vertexlog = GL.GetShaderInfoLog ((int)VertID);
			if (vertexlog != "") {
				Debug.WriteLine (vertsrc + " - Vertex shader did not compile successfully:");
				Debug.WriteLine (vertexlog);
			}

			GL.CompileShader (FragID);
			string fragmentlog = GL.GetShaderInfoLog ((int)FragID);
			if (fragmentlog != "") {
				Debug.WriteLine (fragsrc + " - Fragment shader did not compile successfully:");
				Debug.WriteLine (fragmentlog);
			}

			if (fragmentlog != "" || vertexlog != "") {
				throw new Exception ();
			}

			ID = GL.CreateProgram ();

			GL.AttachShader (ID, VertID);
			GL.AttachShader (ID, FragID);

			GL.LinkProgram (ID);
			GL.ValidateProgram (ID);

			//string programlog = GL.GetShaderInfoLog((int)ID);
			//if (programlog != "") {
			//    Debug.WriteLine("Program did not compile successfully:");
			//    Debug.WriteLine(programlog);
			//}

			UniformLocations = new Dictionary<string, int> ();

			ResourceManager.Resources.Add (this);
		}

		public void AddUniform (string s)
		{
			int uniformLocation = GL.GetUniformLocation (ID, s);
			if (uniformLocation == -1)
				Debug.WriteLine (String.Format ("Uniform name not used or not found: {0}", s));
			UniformLocations [s] = GL.GetUniformLocation (ID, s);
		}

		public void SetUniform1b (string s, bool b)
		{
			GL.Uniform1 (GetUniform (s), b ? 1 : 0);
		}

		public void SetUniform1i (string s, int vec)
		{
			GL.Uniform1 (GetUniform (s), vec);
		}

		public void SetUniform1f (string s, float vec)
		{
			GL.Uniform1 (GetUniform (s), vec);
		}

		public void SetUniform1d (string s, double vec)
		{
			GL.Uniform1 (GetUniform (s), vec);
		}

		public void SetUniform2f (string s, Vector2 vec)
		{
			GL.Uniform2 (GetUniform (s), vec);
		}

		public void SetUniform3f (string s, Vector3 vec)
		{
			GL.Uniform3 (GetUniform (s), vec);
		}

		public void SetUniform4f (string s, Vector4 vec)
		{
			GL.Uniform4 (GetUniform (s), vec);
		}

		public void SetUniform4c (string s, Color4 vec)
		{
			GL.Uniform4 (GetUniform (s), vec);
		}

		public void SetUniform4m (string s, Matrix mat)
		{
			GL.UniformMatrix4 (GetUniform (s), false, ref mat);
		}

		private int GetUniform (string s)
		{
			if (!UniformLocations.ContainsKey (s)) {
				throw new ArgumentException ("Uniform name not found: " + s);
			}
			return UniformLocations [s];
		}

		public void Dispose ()
		{
			GL.DetachShader (ID, VertID);
			GL.DetachShader (ID, FragID);

			GL.DeleteShader (VertID);
			GL.DeleteShader (FragID);
			GL.DeleteShader (ID);
		}
	}
}
