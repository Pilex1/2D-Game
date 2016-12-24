using Game.Core;
using Game.Terrains;
using Game.Util;
using OpenGL;

namespace Game.Entities {
    class Warder : Entity {

        private CooldownTimer tileTimer;

        private const int rangex = 8;
        private const int rangey = 6;

        public Warder(Vector2 pos) : base(EntityID.Warder, pos) {
            data.life = new BoundedFloat(100, 0, 100);
            tileTimer = new CooldownTimer(40);
        }

        public override void InitTimers() {
            CooldownTimer.AddTimer(tileTimer);
        }

        public override void Update() {
            DamageNatural(GameTime.DeltaTime);

            if (!tileTimer.Ready()) return;
            tileTimer.Reset();

            int x = (MathUtil.RandBool(Program.Rand) ? 1 : -1) * rangex;
            int y = MathUtil.RandInt(Program.Rand, -rangey, rangey);
            int rx = (int)data.pos.x + x;
            int ry = (int)data.pos.y + y;
            if (Terrain.TileAt(rx, ry).enumId == TileID.Air) {
                Terrain.SetTile(rx, ry, Tile.WardedTile);
            }
        }
    }
}
