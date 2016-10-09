using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Drawing;
using Game.Util;
using Game.Core;
using Game.Entities;

namespace Game.Particles {
    abstract class StaffParticle : Particle {
        public StaffParticle(EntityModel model, Vector2 pos) : base(model, pos) {

        }

    }

    class StaffParticleGreen : StaffParticle {

        private static CooldownTimer cooldown;
        private static EntityModel _model;
        private static EntityModel Model {
            get {
                if (_model == null) {
                    Texture texture = TextureUtil.CreateTexture(Color.ForestGreen);
                    Gl.BindTexture(texture.TextureTarget, texture.TextureID);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                    Gl.TexParameteri(texture.TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                    Gl.BindTexture(texture.TextureTarget, 0);
                    _model = EntityModel.CreateRectangle(new Vector2(0.5, 0.5), texture);
                    _model.blend = true;
                }
                return _model;
            }
        }

        internal static new void Init() {
            cooldown = new CooldownTimer(0.4f);
        }

        public static void Create(Vector2 pos, Vector2 vel) {
            if (!cooldown.Ready()) return;
            cooldown.Reset();
            new StaffParticleGreen(pos, vel);
        }

        private StaffParticleGreen(Vector2 pos, Vector2 vel) : base(Model, pos) {
            ParticleData pdata = (ParticleData)base.data;
            pdata.life = 50;
            pdata.rotfactor = 0.001f;
            pdata.vel.x = vel.x;
            pdata.vel.y = vel.y;
        }
    }

}
