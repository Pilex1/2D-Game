using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Entities {
    enum EntityID {
        //16 x 16
        None = 0, ShooterProjectile, ParticlePurple, ParticleRed, ParticleGreen, ParticleBlue, Squisher, ParticleYellow, HitboxOutline, EntityCage,

        //16 x 32
        Shooter = 128, Player, PlayerSimple
    }
}
