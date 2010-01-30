using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// The person of a <see cref="Noun"/>. "Second" is second person: i.e. "You do this." First person
    /// is not used by the game, since it would indicate what the computer itself is doing.
    /// </summary>
    public enum Person
    {
        Second,
        Third
    }
}
