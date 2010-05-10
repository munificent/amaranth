using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class TerminalWriter : TerminalWriterBase
    {
        public TerminalWriter(TerminalBase terminal, Vec size, ITerminalState state)
            : base(state)
        {
            mTerminal = terminal;
            mSize = size;
        }

        private TerminalBase mTerminal;
        private Vec mSize;

        protected override TerminalBase GetTerminal() { return mTerminal; }
        protected override Vec GetSize() { return mSize; }
    }
}
