using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public interface IRoomDecorator
    {
        void AddDecoration(Vec pos);
        void AddInsideRoom(Vec pos);
        void AddDoor(Vec pos);
    }
}
