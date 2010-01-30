using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    public class TitleBar : Control
    {
        public TitleBar()
            : base()
        {
        }

        protected override Rect GetBounds()
        {
            return new Rect(0, 0, Parent.Bounds.Width, 1);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            using (new DisposableTerminalState(terminal))
            {
                terminal.State.ForeColor = TerminalColors.White;
                terminal.State.BackColor = TerminalColors.DarkGray;

                terminal.Clear();

                // write the navigation text
                terminal[-Screen.UI.Title.Length, 0][TerminalColors.Gray].Write(Screen.UI.Title);
                terminal[0, 0][TerminalColors.Gray].Write(Screen.Title);
            }
        }
    }
}
