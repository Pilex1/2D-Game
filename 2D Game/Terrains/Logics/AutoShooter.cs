﻿using Game.Core;
using Game.Entities;
using Game.Items;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Terrains.Logics {

    [Serializable]
    class AutoShooterAttribs : PowerDrain {

        [NonSerialized]
        private CooldownTimer cooldown;

        private const int range = 16;
        public bool state;

        public AutoShooterAttribs() : base(delegate () { return RawItem.AutoShooter; }) {
            powerIn.SetPowerAll(new BoundedFloat(16));
            cost = 8;
            state = false;
        }

        protected override void UpdateMechanics(int x, int y) {

            if (cooldown == null) cooldown = new CooldownTimer(30);

            BoundedFloat buffer = new BoundedFloat(0, 0, cost);

            CacheInputs();

            if (powerIn.TotalPower() >= cost) {
                powerIn.GivePowerAll(ref buffer);
            }

            EmptyInputs();

            if (buffer.IsFull()) {

                state = true;

                var pos = new Vector2(x - range, y - range);
                var size = new Vector2(2 * range, 2 * range);
                var entities = EntityManager.GetEntitiesAt(pos, size, e => !(e is AutoDart || e is Player || e.data.invulnerable));

                if (cooldown.Ready()) {
                    if (entities.Length > 0) {
                        cooldown.Reset();

                        var e = entities[MathUtil.RandInt(Program.Rand, 0, entities.Length - 1)];
                        var darvel = e.data.pos - new Vector2(x, y);
                        darvel.Normalize();
                        var dartpos = new Vector2(x, y);
                        dartpos += 1.5f * darvel;
                        darvel /= 10;
                        var ad = new AutoDart(dartpos, darvel);
                        if (!ad.Colliding())
                            EntityManager.AddEntity(ad);
                    } else {
                        cooldown.SetTime(-100);
                    }
                }
            } else {
                state = false;
            }

            Terrain.TileAt(x, y).enumId = state ? TileID.AutoShooterOn : TileID.AutoShooterOff;

        }
    }
}
