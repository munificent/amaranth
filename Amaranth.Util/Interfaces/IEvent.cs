using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public interface IEvent<TSender, TArgs>
    {
        void Add(Action<TSender, TArgs> handler);
        void Remove(Action<TSender, TArgs> handler);
    }
}
