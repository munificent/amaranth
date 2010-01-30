using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class Pair<T>
    {
        public T A;
        public T B;

        public Pair(T a, T b)
        {
            A = a;
            B = b;
        }
    }

    /// <summary>
    /// Interface for an algorithm that connects a collection of rooms together.
    /// </summary>
    public interface IRoomConnector
    {
        IEnumerable<Pair<Rect>> Connect(IList<Rect> rooms);
    }
}
