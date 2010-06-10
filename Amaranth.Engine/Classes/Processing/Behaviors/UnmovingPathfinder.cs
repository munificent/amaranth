using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IPathfinder"/> that a <see cref="Monster"/> that does not move at all, but will
    /// attack the target if next to it.
    /// </summary>
    [Serializable]
    public class UnmovingPathfinder : IPathfinder
    {
        #region IPathfinder Members

        public Direction GetDirection(Monster monster, Entity target)
        {
            // default to not moving
            Direction direction = Direction.None;

            // if next to the target, attack
            Vec distance = target.Position - monster.Position;
            if (distance.KingLength == 1)
            {
                direction = Direction.Towards(distance);
            }

            return direction;
        }

        #endregion
    }
}
