using Game.Assets;
using Game.Terrains;
using Game.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Logics {

    static class LogicManager {

        public static void Update() {

            var logicDict = Terrain.LogicDict;
            var list = new List<Vector2i>(logicDict.Keys);
            foreach (Vector2i v in list) {
                LogicData logic;
                logicDict.TryGetValue(v, out logic);
                if (logic == null) continue;
                logic.Update(v.x, v.y);

                SwitchData switchdata = logic as SwitchData;
                WireData wiredata = logic as WireData;
                StickyTilePusherData tilepusherdata = logic as StickyTilePusherData;
                LogicLampData logiclampdata = logic as LogicLampData;
                LogicBridgeData logicbridgedata = logic as LogicBridgeData;

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
                }
            }
        }
    }
}
