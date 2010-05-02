using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    /// <summary>
    /// A DisposableTerminalState is a TerminalState that automatically pushes itself on the given
    /// Terminal and then pops itself when it is disposed. Makes it easy to use TerminalStates with
    /// using() blocks.
    /// </summary>
    public class DisposableTerminalState : TerminalState, IDisposable
    {
        public DisposableTerminalState(ITerminal terminal)
        {
            mTerminal = terminal;
            mTerminal.PushState(this);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (mTerminal == null) throw new ObjectDisposedException("DisposableTerminalState");

            mTerminal.PopState();
            mTerminal = null;
        }

        #endregion

        private ITerminal mTerminal;
    }
}
