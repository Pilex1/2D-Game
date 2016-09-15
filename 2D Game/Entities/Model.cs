using System;
using OpenGL;

namespace Game.Entities {
    abstract class Model {
        public VBO<Vector2> Vertices { get; set; }
        public VBO<int> Elements { get; set; }

        public BeginMode DrawingMode { get; set; }
        public PolygonMode PolyMode { get; set; }

        public Model(VBO<Vector2> vertices, VBO<int> elements, BeginMode drawingMode, PolygonMode polyMode) {
            Vertices = vertices;
            Elements = elements;
            DrawingMode = drawingMode;
            PolyMode = polyMode;
        }

        public virtual void Dispose() {
            Vertices.Dispose();
            Elements.Dispose();
        }
    }

    class TexturedModel : Model {
        public Texture Texture { get; set; }
        public VBO<Vector2> UVs { get; set; }

        public TexturedModel(VBO<Vector2> vertices, VBO<int> elements, VBO<Vector2> uvs, Texture texture, BeginMode drawingMode, PolygonMode polyMode) : base(vertices, elements, drawingMode, polyMode) {
            Texture = texture;
            UVs = uvs;
        }

        public override void Dispose() {
            base.Dispose();
            Texture.Dispose();
            UVs.Dispose();
        }
    }

    class LightingTexturedModel : TexturedModel {
        public VBO<float> Lightings { get; set; }

        public LightingTexturedModel(VBO<Vector2> vertices, VBO<int> elements, BeginMode drawingMode, PolygonMode polyMode, Texture texture, VBO<Vector2> uvs, VBO<float> lightings) : base(vertices, elements, uvs, texture, drawingMode, polyMode) {
            Lightings = lightings;
        }
    }

    class ColouredModel : Model {
        public VBO<Vector4> Colours { get; protected set; }
        public ColouredModel(VBO<Vector2> vertices, VBO<int> elements, VBO<Vector4> colours, BeginMode drawingMode, PolygonMode polyMode) : base(vertices, elements, drawingMode, polyMode) {
            Colours = colours;
        }

        public override void Dispose() {
            base.Dispose();
            Colours.Dispose();
        }

        public static ColouredModel CreateRectangle(Vector2 size, Vector4[] colours, PolygonMode polyMode) {
            VBO<Vector2> vertices = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, size.y),
                new Vector2(0, 0),
                new Vector2(size.x, size.y),
                new Vector2(size.x, 0)
            });
            VBO<int> elements = new VBO<int>(new int[] {
                0,1,2,3
            }, BufferTarget.ElementArrayBuffer);

            return new ColouredModel(vertices, elements, new VBO<Vector4>(colours), BeginMode.TriangleStrip, polyMode);
        }

        public static ColouredModel CreateRectangle(Vector2 size, Vector4 colour, PolygonMode polyMode) { return CreateRectangle(size, new Vector4[] { colour, colour, colour, colour }, polyMode); }

        public static ColouredModel CreateWireframeRectangle(Vector2 size, Vector4[] colours) {
            VBO<Vector2> vertices = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, size.y),
                new Vector2(0, 0),
                 new Vector2(size.x, 0),
                new Vector2(size.x, size.y)
            });
            VBO<int> elements = new VBO<int>(new int[] {
                0,1,2,3
            }, BufferTarget.ElementArrayBuffer);

            return new ColouredModel(vertices, elements, new VBO<Vector4>(colours), BeginMode.LineLoop, PolygonMode.Fill);
        }

        public static ColouredModel CreateWireframeRectangle(Vector2 size, Vector4 colour) {
            return CreateWireframeRectangle(size, new Vector4[] { colour, colour, colour, colour });
        }


    }
}
