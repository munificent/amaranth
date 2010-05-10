using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public interface IReadableTerminal
    {
        event EventHandler<CharacterEventArgs> CharacterChanged;

        Vec Size { get; }

        Character Get(Vec pos);
        Character Get(int x, int y);
    }
}
