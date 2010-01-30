using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;
using Amaranth.UI;

namespace Amaranth.TermApp
{
    public class HelpScreen : Screen, IInputHandler
    {
        public HelpScreen()
            : base("Help")
        {
            Controls.Add(new StatusBar());
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal[WindowBounds].Clear();
            terminal[WindowBounds][TerminalColors.Yellow].DrawBox(false, false);
            terminal[WindowBounds.TopLeft.Offset(2, 0)].Write("Amaranth Help");

            // walking
            terminal[WindowBounds.TopLeft.Offset(2, 2)][TerminalColors.Yellow].Write("i o p");
            terminal[WindowBounds.TopLeft.Offset(2, 4)][TerminalColors.Yellow].Write("k   ;");
            terminal[WindowBounds.TopLeft.Offset(2, 6)][TerminalColors.Yellow].Write(", . /");
            terminal[WindowBounds.TopLeft.Offset(3, 3)][TerminalColors.Gray].Write(Glyph.Backslash);
            terminal[WindowBounds.TopLeft.Offset(4, 3)][TerminalColors.Gray].Write(Glyph.ArrowUp);
            terminal[WindowBounds.TopLeft.Offset(5, 3)][TerminalColors.Gray].Write(Glyph.Slash);
            terminal[WindowBounds.TopLeft.Offset(3, 4)][TerminalColors.Gray].Write(Glyph.ArrowLeft);
            terminal[WindowBounds.TopLeft.Offset(4, 4)][TerminalColors.Flesh].Write(Glyph.Face);
            terminal[WindowBounds.TopLeft.Offset(5, 4)][TerminalColors.Gray].Write(Glyph.ArrowRight);
            terminal[WindowBounds.TopLeft.Offset(3, 5)][TerminalColors.Gray].Write(Glyph.Slash);
            terminal[WindowBounds.TopLeft.Offset(4, 5)][TerminalColors.Gray].Write(Glyph.ArrowDown);
            terminal[WindowBounds.TopLeft.Offset(5, 5)][TerminalColors.Gray].Write(Glyph.Backslash);

            terminal[WindowBounds.TopLeft.Offset(8, 2)].Write("Walk one step in that direction");
            terminal[WindowBounds.TopLeft.Offset(8, 3)].Write("(Hold shift to run)");

            // resting
            terminal[WindowBounds.TopLeft.Offset(2, 8)].Write("    ^yl^- Rest one turn");
            terminal[WindowBounds.TopLeft.Offset(8, 9)].Write("(Hold shift to rest until healed)");
        }

        private Rect WindowBounds
        {
            get
            {
                return new Rect(50, 30).CenterIn(Bounds);
            }
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Back", new KeyInfo(Key.Escape));
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            if (key.Key == Key.Escape)
            {
                UI.PopScreen();
                return true;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion
    }
}
