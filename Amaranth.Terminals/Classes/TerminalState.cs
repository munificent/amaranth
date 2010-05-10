using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class TerminalState : ITerminalState
    {
        public Vec   Cursor    { get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }

        public TerminalState(ITerminalState cloneFrom)
        {
            Cursor = cloneFrom.Cursor;
            ForeColor = cloneFrom.ForeColor;
            BackColor = cloneFrom.BackColor;
        }

        public TerminalState(Vec cursor, Color foreColor, Color backColor)
        {
            Cursor = cursor;
            ForeColor = foreColor;
            BackColor = backColor;
        }

        public TerminalState(Color foreColor, Color backColor)
            : this(new Vec(0, 0), foreColor, backColor)
        {
        }

        public TerminalState()
            : this(new Vec(0, 0), TerminalColors.White, TerminalColors.Black)
        {
        }
    }
}
