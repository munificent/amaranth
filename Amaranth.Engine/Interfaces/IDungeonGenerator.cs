using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public interface IDungeonGenerator
    {
        void Create(Dungeon dungeon, bool isDescending, int depth, object options);
    }
}
