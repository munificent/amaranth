using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Interface for a class that can generate a set of rooms for a dungeon.
    /// </summary>
    public interface IRoomPlacement
    {
        IEnumerable<Rect> Generate(Rect bounds);
        object Options { get; }
    }
}
