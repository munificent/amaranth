using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public interface ITerminalState
    {
        Vec Cursor { get; set; }
        Color ForeColor { get; set; }
        Color BackColor { get; set; }
    }
}
