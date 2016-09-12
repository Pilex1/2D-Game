using System;
using OpenGL;

namespace Game.Entities {
    abstract class Model {
        public VBO<Vector2> Vertices { get; set; }
        public VBO<int> Elements { get; set; }

        public BeginMode DrawingMode { get; set; }

        public Model(VBO<Vector2> vertices, VBO<int> elements, BeginMode drawingMode) {
            Vertices = vertices;
            Elements = elements;
            DrawingMode = drawingMode;
        }

        public virtual void Dispose() {
            Vertices.Dispose();
            Elements.Dispose();
        }
    }

    class TexturedModel :Model {
        public Texture Texture { get; set; }
        public VBO<Vector2 > UVs { get; set; }

        public TexturedModel(VBO<Vector2> vertices, VBO<int> elements, BeginMode drawingMode, Texture texture, VBO<Vector2> uvs):base(vertices,elements,drawingMode) {
            Texture = texture;
            UVs = uvs;
        }

        public override void Dispose() {
            base.Dispose();
            Texture.Dispose();
            UVs.Dispose();
        }
    }

    class LightingTexturedModel :TexturedModel {
        public VBO<float> Lightings { get; set; }

        public LightingTexturedModel(VBO<Vector2> vertices, VBO<int> elements, BeginMode drawingMode, Texture texture, VBO<Vector2> uvs, VBO<float> lightings) : base(vertices, elements, drawingMode, texture, uvs) {
            Lightings = lightings;
        }
    }

    class ColouredModel : Model {
        public VBO<Vector4> Colours { get; protected set; }
        public ColouredModel(VBO<Vector2> vertices, VBO<int> elements, VBO<Vector4> colours, BeginMode drawingMode):base(vertices,elements,drawingMode) {
            Colours = colours;
        }

        public override void Dispose() {
            base.Dispose();
            Colours.Dispose();
        }
    }
}
