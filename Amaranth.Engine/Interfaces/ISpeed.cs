using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Simple interface for a class with a Speed property. Used by Energy to access its
    /// Entity's speed.
    /// </summary>
    public interface ISpeed
    {
        int Speed { get; }
    }
}
