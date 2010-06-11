using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;

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
            terminal = terminal[TermColor.Gray, TermColor.DarkGray].CreateWindow();

            terminal.Clear();

            // write the navigation text
            terminal[-Screen.UI.Title.Length, 0].Write(Screen.UI.Title);
            terminal[0, 0].Write(Screen.Title);
        }
    }
}
