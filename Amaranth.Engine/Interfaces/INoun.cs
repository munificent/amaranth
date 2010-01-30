using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public interface INoun
    {
        string NounText { get; }
        Person Person { get; }
        string Pronoun { get; }
        string Possessive { get; }
    }
}
