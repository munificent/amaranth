using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Amaranth.Reports
{
    public interface IStatRow
    {
        bool IsTall { get; }
        string Name { get; }
        Color Color { get; }
        int Max(int x);
        IEnumerable<int> Values { get; }
    }
}
