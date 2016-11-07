using Game.Interaction;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrains {

    [Serializable]
    class ExplosionData : TileAttribs {

        public float radius, error;

        public object HotbarExplosion { get; private set; }

        public override void Interact(int x, int y) {
            if (Hotbar.CurrentlySelectedItem() == Assets.Item.Igniter)
                Terrain.Explode(x, y, radius, error);
        }
    }

}
