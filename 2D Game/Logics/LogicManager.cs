using Game.Assets;
using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Logics {

    static class LogicManager {

        private static CooldownTimer cooldown = new CooldownTimer(1000f / 60);

        public static void Update() {
            if (!cooldown.Ready()) return;
            cooldown.Reset();

            var logicDict = Terrain.LogicDict;
            var list = new List<Vector2i>(logicDict.Keys);
            foreach (Vector2i v in list) {
                LogicAttribs logic;
                logicDict.TryGetValue(v, out logic);
                if (logic == null) continue;
                logic.Update(v.x, v.y);

                SwitchAttribs switchdata = logic as SwitchAttribs;
                WireAttribs wiredata = logic as WireAttribs;
                StickyTilePusherAttribs tilepusherdata = logic as StickyTilePusherAttribs;
                StickyTilePullerAttribs tilepullerdata = logic as StickyTilePullerAttribs;
                LogicLampAttribs logiclampdata = logic as LogicLampAttribs;
                LogicBridgeAttribs logicbridgedata = logic as LogicBridgeAttribs;
                SingleTilePusherAttribs singletilepusherdata = logic as SingleTilePusherAttribs;

                if (switchdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = switchdata.state ? TileEnum.SwitchOn : TileEnum.SwitchOff;
                } else if (wiredata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = wiredata.state ? TileEnum.WireOn : TileEnum.WireOff;
                } else if (tilepusherdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = tilepusherdata.state ? TileEnum.TilePusherOn : TileEnum.TilePusherOff;
                } else if (logiclampdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = logiclampdata.state ? TileEnum.LogicLampLit : TileEnum.LogicLampUnlit;
                } else if (logicbridgedata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = logicbridgedata.stateHorz ? (logicbridgedata.stateVert ? TileEnum.LogicBridgeHorzVertOn : TileEnum.LogicBridgeHorzOn) : (logicbridgedata.stateVert ? TileEnum.LogicBridgeVertOn : TileEnum.LogicBridgeOff);
                } else if (tilepullerdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = tilepullerdata.state ? TileEnum.TilePullerOn : TileEnum.TilePullerOff;
                } else if (singletilepusherdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = singletilepusherdata.state ? TileEnum.SingleTilePusherOn : TileEnum.SingleTilePusherOff;
                }
            }
        }
    }
}
