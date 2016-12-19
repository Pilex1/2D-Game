using Game.Terrains;
using Game.Util;
using System.Collections.Generic;

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
                    Terrain.TileAt(v.x, v.y).enumId = switchdata.state ? TileID.SwitchOn : TileID.SwitchOff;
                } else if (wiredata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = wiredata.state ? TileID.WireOn : TileID.WireOff;
                } else if (tilepusherdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = tilepusherdata.state ? TileID.TilePusherOn : TileID.TilePusherOff;
                } else if (logiclampdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = logiclampdata.state ? TileID.LogicLampLit : TileID.LogicLampUnlit;
                } else if (logicbridgedata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = logicbridgedata.stateHorz ? (logicbridgedata.stateVert ? TileID.WireBridgeHorzVertOn : TileID.WireBridgeHorzOn) : (logicbridgedata.stateVert ? TileID.WireBridgeVertOn : TileID.WireBridgeOff);
                } else if (tilepullerdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = tilepullerdata.state ? TileID.TilePullerOn : TileID.TilePullerOff;
                } else if (singletilepusherdata != null) {
                    Terrain.TileAt(v.x, v.y).enumId = singletilepusherdata.state ? TileID.SingleTilePusherOn : TileID.SingleTilePusherOff;
                }
            }
        }
    }
}
