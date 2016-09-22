using Game.Util;
using OpenGL;
using System.Drawing;

namespace Game.Entities {
    abstract class Model {
        private VBO<Vector2> vertices;
        public VBO<Vector2> Vertices {
            get {
                return vertices;
            }
            set {
                if (vertices != null) vertices.Dispose();
                vertices = value;
            }
        }

        private VBO<int> elements;
        public VBO<int> Elements {
            get {
                return elements;
            }
            set {
                if (elements != null) elements.Dispose();
                elements = value;
            }
        }

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
        private Texture texture;
        public Texture Texture {
            get { return texture; }
            set {
                if (texture != null) texture.Dispose();
                texture = value;
            }
        }

        private VBO<Vector2> uvs;
        public VBO<Vector2> UVs {
            get { return uvs; }
            set {
                if (uvs != null) uvs.Dispose();
                uvs = value;
            }
        }

        public TexturedModel(VBO<Vector2> vertices, VBO<int> elements, VBO<Vector2> uvs, Texture texture, BeginMode drawingMode, PolygonMode polyMode) : base(vertices, elements, drawingMode, polyMode) {
            Texture = texture;
            UVs = uvs;
        }

        public override void Dispose() {
            base.Dispose();
            Texture.Dispose();
            UVs.Dispose();
        }

        public static TexturedModel CreateWireframeRectangle(Vector2 size, Vector4 colour) {
            return CreateWireframeRectangle(size, new Vector4[] { colour, colour, colour, colour });
        }

        public static TexturedModel CreateWireframeRectangle(Vector2 size, Vector4[] colours) {
            var vertices = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, size.y),
                new Vector2(0, 0),
                 new Vector2(size.x, 0),
                new Vector2(size.x, size.y)
            });
            var uvs = new VBO<Vector2>(new Vector2[] {
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1)
            });
            Bitmap bmp = new Bitmap(2, 2);
            bmp.SetPixel(0, 1, ColourUtil.ColourFromVec4(colours[0]));
            bmp.SetPixel(0, 0, ColourUtil.ColourFromVec4(colours[1]));
            bmp.SetPixel(1, 0, ColourUtil.ColourFromVec4(colours[2]));
            bmp.SetPixel(1, 1, ColourUtil.ColourFromVec4(colours[3]));
            Texture texture = new Texture(bmp);
            var elements = new VBO<int>(new int[] {
                0,1,2,3
            }, BufferTarget.ElementArrayBuffer);

            return new TexturedModel(vertices, elements, uvs, texture, BeginMode.LineLoop, PolygonMode.Fill);
        }
    }

    class LightingTexturedModel : TexturedModel {

        private VBO<float> lightings;
        public VBO<float> Lightings {
            get { return lightings; }
            set {
                if (lightings != null) lightings.Dispose();
                lightings = value;
            }
        }

        public LightingTexturedModel(VBO<Vector2> vertices, VBO<int> elements, BeginMode drawingMode, PolygonMode polyMode, Texture texture, VBO<Vector2> uvs, VBO<float> lightings) : base(vertices, elements, uvs, texture, drawingMode, polyMode) {
            Lightings = lightings;
        }

        public override void Dispose() {
            base.Dispose();
            Lightings.Dispose();
        }
    }

    class ColouredModel : Model {

        private VBO<Vector4> colours;
        public VBO<Vector4> Colours {
            get { return colours; }
            set {
                if (colours != null) colours.Dispose();
                colours = value;
            }
        }
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
