using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class TerminalWriter : TerminalWriterBase
    {
        public TerminalWriter(TerminalBase terminal, Vec size, ITerminalState state)
        {
            mTerminal = terminal;
            mSize = size;
            mState = state;
        }

        private TerminalBase mTerminal;
        private Vec mSize;
        private ITerminalState mState;

        protected override TerminalBase GetTerminal() { return mTerminal; }
        protected override ITerminalState GetState() { return mState; }
        protected override Vec GetSize() { return mSize; }
    }
}
