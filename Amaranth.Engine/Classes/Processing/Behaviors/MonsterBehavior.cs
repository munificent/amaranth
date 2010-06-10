using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for a <see cref="Behavior"/> performed by a <see cref="Monster"/>.
    /// </summary>
    [Serializable]
    public class MonsterBehavior : Behavior
    {
        public Monster Monster { get { return mMonster; } }

        public bool IsAwake { get { return mIsAwake; } }

        public MonsterBehavior(NotNull<Monster> monster, IPathfinder pathfinder)
        {
            if (pathfinder == null) throw new ArgumentNullException("pathfinder");

            mMonster = monster;
            mPathfinder = pathfinder;
        }
        
        public override Action NextAction()
        {
            Entity target = Monster.Dungeon.Game.Hero;
            Vec distance = target.Position - Monster.Position;

            // wake up or fall asleep
            if (!mIsAwake)
            {
                // if close enough to the hero, attempt to wake up
                if ((distance.KingLength < WakeUpDistance) && Rng.OneIn(WakeUpChance))
                {
                    WakeUp();
                }
            }
            else
            {
                // if far enough, try to fall back asleep
                if ((distance.KingLength > FallAsleepDistance) && Rng.OneIn(FallAsleepChance))
                {
                    FallAsleep();
                }
            }

            if (mIsAwake)
            {
                // consider performing a move
                foreach (Move move in Monster.Race.Moves)
                {
                    // see if it's possible and the odds match
                    if (move.WillUseMove(Monster, target) && move.ShouldAttempt())
                    {
                        // use this move
                        return move.GetAction(Monster, target);
                    }
                }

                // walk
                Direction direction = mPathfinder.GetDirection(Monster, target);

                return new WalkAction(Monster, direction);
            }

            // otherwise, just stand still
            return new WalkAction(Monster, Direction.None);
        }

        public override void Disturb()
        {
            WakeUp();
        }

        private void WakeUp()
        {
            mIsAwake = true;
        }

        protected bool IsOccupiedByOtherMonster(Vec move)
        {
            Entity occupier = Monster.Dungeon.Entities.GetAt(Monster.Position + move);

            if ((occupier != null) && (occupier != Monster) && (occupier != Monster.Dungeon.Game.Hero))
            {
                return true;
            }

            return false;
        }

        private void FallAsleep()
        {
            mIsAwake = false;
        }

        private const int WakeUpDistance = 8;
        private const int WakeUpChance = 2;

        private const int FallAsleepDistance = 24;
        private const int FallAsleepChance = 4;

        private bool mIsAwake;
        private Monster mMonster;

        private IPathfinder mPathfinder;
    }
}
