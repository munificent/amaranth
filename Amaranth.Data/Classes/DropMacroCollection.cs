using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public class DropMacroCollection<T> : Dictionary<string, IDrop<T>>
    {
    }
}
