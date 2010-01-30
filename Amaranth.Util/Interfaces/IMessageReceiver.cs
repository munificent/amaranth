using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public interface IMessageReceiver<TSender, TMessage>
    {
        void Receive(TSender sender, TMessage message);
    }
}
