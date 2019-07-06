using System;
using Game.Items;
using Game.Main.Util;
using Pencil.Gaming.MathUtils;

namespace Game.Terrains.Lightings {

	[Serializable]
	class LightAttribs : TileAttribs, ILight {

		protected int radius;
		protected Vector3 colour;
		protected float strength;

		public LightAttribs(int radius, Vector3 colour, float strength, Func<RawItem> dropItem) : base (dropItem) {
			this.radius = radius;
			this.colour = colour;
			this.strength = strength;
		}

		public LightAttribs(int radius, ColourRGB colour, float strength, Func<RawItem> dropItem) : this (radius, colour.ToVec3 (), strength, dropItem) {
		}


		int ILight.Radius() => radius;

		Vector3 ILight.Colour() => colour;

		float ILight.Strength() => strength;

	}
}
