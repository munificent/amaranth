using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IPathfinder"/> for a <see cref="Monster"/> that tries to naively
    /// run straight towards the target with only minimal obstacle avoidance.
    /// </summary>
    [Serializable]
    public class StraightTowardsPathfinder : IPathfinder
    {
        public StraightTowardsPathfinder(Pursue pursue)
        {
            mPursue = pursue;
        }

        #region IPathfinder Members

        public Direction GetDirection(Monster monster, Entity target)
        {
            Vec relative = target.Position - monster.Position;

            // walk towards player
            Direction direction = Direction.Towards(relative);

            // move erratically
            switch (mPursue)
            {
                case Pursue.Closely:
                    // do nothing
                    break;

                case Pursue.SlightlyErratically:
                    while (Rng.OneIn(3))
                    {
                        if (Rng.OneIn(2)) direction = direction.Next;
                        else direction = direction.Previous;
                    }
                    break;

                case Pursue.Erratically:
                    while (Rng.OneIn(2))
                    {
                        if (Rng.OneIn(2)) direction = direction.Next;
                        else direction = direction.Previous;
                    }
                    break;

                case Pursue.VeryErratically:
                    int turns = Rng.Int(3);
                    for (int i = 0; i < turns; i++)
                    {
                        if (Rng.OneIn(2)) direction = direction.Next;
                        else direction = direction.Previous;
                    }
                    break;
            }

            // don't walk through walls
            if (!monster.CanMove(direction) || monster.IsOccupiedByOtherMonster(direction.Offset, target))
            {
                // try to go around obstacle
                Direction firstTry = direction.Previous;
                Direction secondTry = direction.Next;

                // don't always try the same order
                if (Rng.OneIn(2))
                {
                    Math2.Swap(ref firstTry, ref secondTry);
                }

                if (monster.CanMove(firstTry) && !monster.IsOccupiedByOtherMonster(firstTry.Offset, target))
                {
                    direction = firstTry;
                }
                else if (monster.CanMove(secondTry) && !monster.IsOccupiedByOtherMonster(secondTry.Offset, target))
                {
                    direction = secondTry;
                }
                else
                {
                    // give up
                    direction = Direction.None;
                }
            }

            return direction;
        }

        #endregion

        private Pursue mPursue;
    }
}
