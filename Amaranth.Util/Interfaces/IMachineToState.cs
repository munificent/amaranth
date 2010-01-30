using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// The interface between a <see cref="Machine"/> and a <see cref="State"/> it owns.
    /// </summary>
    internal interface IMachineToState
    {
        void Init(IStateToMachine machine);
        void Start();
        void Stop();
        void End();

        void ReceiveMessage<TSender, TMessage>(TSender sender, TMessage message);
    }
}
