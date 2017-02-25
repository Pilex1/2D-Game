using Game.Core;
using Game.Guis.Renderers;
using Game.Main;
using Game.Main.Util;
using Game.Util;
using Pencil.Gaming.MathUtils;
using System;

namespace Game.Guis {
    class InGameTextbox : Textbox {

        public InGameTextbox(Vector2 pos, Vector2 size, TextFont font, float textsize) : base(pos, size, font, textsize) {

        }

        protected override GuiModel GenModel() {
            return GuiModel.CreateRectangle(size, TextureUtil.CreateTexture(0.2f, 0.2f, 0.2f, 0.8f));
        }

        public void Execute() {
            var s = ToString();
            try {
                object obj = CommandParser.Execute(s);
                if (obj != null) {
                    GameGuiRenderer.TextLog.AppendLine(CommandParser.Execute(s).ToString());
                }
                 GameGuiRenderer.TextLog.AppendLine("Succesfully executed command: \"" + s + "\"");
            } catch (Exception e) {
                 GameGuiRenderer.TextLog.AppendLine("Error while parsing: \"" + s + "\"");
                  GameGuiRenderer.TextLog.AppendLine(e.Message);
            }
            //GameGuiRenderer.TextLog.AppendLine(s);
            ClearText();
        }

        public override void Update() {
            if (!text.ToString().EndsWith("_"))
                text.AppendCharacter('_');

            if (!textcooldown.Ready())
                return;
            textcooldown.Reset();

            text.style.colour = disabled ? new ColourRGBA(0, 0, 0, 0) : new ColourRGBA(255,255,255, 1);
            colour = disabled ? new ColourRGBA(0, 0, 0, 0) : new ColourRGBA(51, 51, 51, 0.8f);
            if (disabled) return;

            HandleInputs();
        }

    }
}
