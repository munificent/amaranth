using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Defines the object used by <see cref="MonsterBehavior"/> to determine how a <see cref="Monster"/>
    /// tries to reach its target. Each pathfinder roughly corresponds to the intelligence of a monster.
    /// </summary>
    public interface IPathfinder
    {
        Direction GetDirection(Monster monster, Entity target);
    }
}
