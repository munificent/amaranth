using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// The interface between a <see cref="State"/> and the <see cref="Machine"/> that owns it.
    /// </summary>
    internal interface IStateToMachine
    {
        void Push(NotNull<State> state);
        void Pop(NotNull<State> state);
    }
}
