using System;
using OpenGL;
using System.Collections.Generic;
using Game.Entities;

namespace Game.Core {
    class Background {

        public TexturedModel model;

        //public static HashSet<Background> BackgroundSet = new HashSet<Background>();
        public static Background landscape;

        public static void Init() {
            var vertices = new VBO<Vector2>(new Vector2[] {
                new Vector2(-1,1),
                new Vector2(-1,-1),
                new Vector2(1,1),
                new Vector2(1,-1)
            });
            var elements = new VBO<int>(new int[] {
                0, 1, 2, 3,0
            });
            var uvs = new VBO<Vector2>(new Vector2[] {
              new Vector2(-1,1),
                new Vector2(-1,-1),
                new Vector2(1,1),
                new Vector2(1,-1)
            });
            landscape = new Background(new TexturedModel(vertices, elements, uvs, new Texture("Assets/BackgroundLandscape.png"), BeginMode.TriangleStrip, PolygonMode.Fill));
        }

        public Background(TexturedModel model) {
            this.model = model;

        }
    }
}
