using Game.Items;
using Game.Main.Util;
using Game.Terrains.Lighting;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Terrains.Logics {

	[Serializable]
	class LogicBridgeAttribs : PowerTransmitter, IMultiLight {

		protected bool stateHorz;
		protected bool stateVert;

		public LogicBridgeAttribs() : base (delegate () {
				return RawItem.WireBridge;
			}) {
			BoundedFloat p = new BoundedFloat (0, 0, 64);
			powerOut.SetPowerAll (p);
			powerIn.SetPowerAll (p);
			stateHorz = stateVert = false;
		}

		protected override void UpdateMechanics(int x, int y) {

			BoundedFloat bufferHorz = new BoundedFloat (powerIn.GetPower (Direction.Left).max + powerIn.GetPower (Direction.Right).max);
			BoundedFloat bufferVert = new BoundedFloat (powerIn.GetPower (Direction.Up).max + powerIn.GetPower (Direction.Down).max);

			CacheInputs ();

			powerIn.GivePower (Direction.Left, ref bufferHorz);
			powerIn.GivePower (Direction.Right, ref bufferHorz);
			powerIn.GivePower (Direction.Up, ref bufferVert);
			powerIn.GivePower (Direction.Down, ref bufferVert);

			EmptyInputs ();

			bufferHorz -= dissipate;
			bufferVert -= dissipate;

			int neighbourHorz = (IsTransmitter (x, y, Direction.Left) ? 1 : 0) + (IsDrain (x, y, Direction.Left) ? 1 : 0) + (IsTransmitter (x, y, Direction.Right) ? 1 : 0) + (IsDrain (x, y, Direction.Right) ? 1 : 0);
			int neighbourVert = (IsTransmitter (x, y, Direction.Up) ? 1 : 0) + (IsDrain (x, y, Direction.Up) ? 1 : 0) + (IsTransmitter (x, y, Direction.Down) ? 1 : 0) + (IsDrain (x, y, Direction.Down) ? 1 : 0);

			if (neighbourHorz != 0) {
				float val = bufferHorz / neighbourHorz;

				if (IsTransmitter (x, y, Direction.Left) || IsDrain (x, y, Direction.Left)) {
					powerOut.TakePower (Direction.Left, ref bufferHorz, val);
				}

				if (IsTransmitter (x, y, Direction.Right) || IsDrain (x, y, Direction.Right)) {
					powerOut.TakePower (Direction.Right, ref bufferHorz, val);
				}

			}
			if (neighbourVert != 0) {
				float val = bufferHorz / neighbourHorz;

				if (IsTransmitter (x, y, Direction.Up) || IsDrain (x, y, Direction.Up)) {
					powerOut.TakePower (Direction.Up, ref bufferHorz, val);
				}

				if (IsTransmitter (x, y, Direction.Down) || IsDrain (x, y, Direction.Down)) {
					powerOut.TakePower (Direction.Down, ref bufferHorz, val);
				}
			}

			CacheOutputs ();

			stateHorz = powerOut.GetPower (Direction.Left) > 0 || powerOut.GetPower (Direction.Right) > 0;
			stateVert = powerOut.GetPower (Direction.Up) > 0 || powerOut.GetPower (Direction.Down) > 0;

			int state = 0;
			if (stateHorz ^ stateVert)
				state = 1;
			else if (stateHorz && stateVert)
				state = 2;
			UpdateMultiLight (x, y, state, this);

			//TransferPowerAll(x, y);

			Terrain.TileAt (x, y).enumId = stateHorz ? (stateVert ? TileID.WireBridgeHorzVertOn : TileID.WireBridgeHorzOn) : (stateVert ? TileID.WireBridgeVertOn : TileID.WireBridgeOff);
		}

		ILight[] IMultiLight.Lights() => new ILight[] { new CLight(0, 0, new ColourRGB(0, 0, 0)), new CLight(4, 0.1f, new ColourRGB(26, 5, 13)), new CLight(4, 0.2f, new ColourRGB(51, 10, 26)) };

		int IMultiLight.State { get; set; }
	}
}
	;