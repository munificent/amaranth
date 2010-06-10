using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    public class Label : PositionControl
    {
        public Label(Vec position, string text)
            : base(position, text)
        {
        }

        protected override Rect GetBounds()
        {
            return new Rect(Position, Title.Length, 1);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            base.OnPaint(terminal);

            terminal.Write(Title);
        }
    }
}
