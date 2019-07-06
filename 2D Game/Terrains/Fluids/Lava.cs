using Game.Entities;
using Game.Main.Util;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Terrains.Fluids {
	[Serializable]
	class LavaAttribs : FlowFluidAttribs, ILight {

		private float damage;

		public LavaAttribs(int increments = 8) : base (increments, 8, Tile.Lava) {
			mvtFactor = 0.02f;
			damage = 0.04f;
		}

		protected override void UpdateFinal(int x, int y) {
			MorphStone (x, y, x, y + 1);
			MorphStone (x, y, x, y - 1);
			MorphStone (x, y, x - 1, y);
			MorphStone (x, y, x + 1, y);
		}

		private void MorphStone(int srcx, int srcy, int x, int y) {
			if (Terrain.TileAt (x, y).tileattribs is WaterAttribs) {
				Terrain.BreakTile (srcx, srcy);
				Terrain.SetTile (srcx, srcy, Tile.Obsidian);
			}
		}

		public override void OnEntityCollision(int x, int y, Direction side, Entity e) {
			e.Damage (damage);
		}

		int ILight.Radius() => 12;

		Vector3 ILight.Colour() => new ColourRGB(255, 100, 50).ToVec3();

		float ILight.Strength() => 0.1f;
	}
}
