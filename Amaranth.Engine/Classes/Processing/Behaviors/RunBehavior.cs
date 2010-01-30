using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class RunBehavior : HeroBehavior
    {
        public override bool NeedsUserInput
        {
            get
            {
                // need input if we've stopped
                return mDirection == Direction.None;
            }
        }

        public RunBehavior(Hero hero, Direction direction)
            : base(hero)
        {
            // make sure we actually can run
            if (!Hero.CanMove(direction)) direction = Direction.None;

            mDirection = direction;

            // check the floor type on each side of the hero (after he takes his first step)
            Vec pos = hero.Position + mDirection;
            mIsLeftOpen = hero.Dungeon.Tiles[pos + mDirection.RotateLeft90].IsPassable;
            mIsRightOpen = hero.Dungeon.Tiles[pos + mDirection.RotateRight90].IsPassable;
        }

        public override Action NextAction()
        {
            Dungeon dungeon = Hero.Dungeon;

            // get the current step
            Action action = new WalkAction(Hero, mDirection, true);

            // process the next step
            Vec pos = Hero.Position + mDirection.Offset;

            // try to turn to follow corridors
            if (!dungeon.Tiles[pos + mDirection.RotateLeft90].IsPassable &&  // wall on the left
                !dungeon.Tiles[pos + mDirection.RotateRight90].IsPassable && // wall on the right
                !(dungeon.Tiles[pos + mDirection].IsPassable &&              // blocked directly ahead
                  dungeon.Tiles[pos + mDirection + mDirection].IsPassable))  // or one step past that
            {
                // we're in a corridor that's about to either turn or dead end

                // first step before the turn
                // ###.#
                // ###.# dir = e
                // .@pn# p = pos
                // ##### n = pos + dir

                // second step halfway through the turn
                // ###.n
                // ###p# dir = ne
                // ..@.# p = pos
                // ##### n = pos + dir

                bool leftTurnOpen = dungeon.Tiles[pos + mDirection.Previous].IsPassable;
                bool rightTurnOpen = dungeon.Tiles[pos + mDirection.Next].IsPassable;

                if (leftTurnOpen && rightTurnOpen)
                {
                    // tees off, so don't decide
                }
                else if (leftTurnOpen)
                {
                    mDirection = mDirection.Previous;
                }
                else if (rightTurnOpen)
                {
                    mDirection = mDirection.Next;
                }
            }
            
            Vec nextPos = pos + mDirection.Offset;

            bool disturb = false;

            // stop if we're going to hit a wall
            if (!Hero.CanOccupy(nextPos)) disturb = true;

            // some things disturb when directly next to the hero,
            // others when they are one step away
            // .....
            // ..12.
            // o@12.
            // ..12.
            // .....
            // o: where hero is
            // @: where hero will be after step
            // 1: disturb if item, stairs, etc.
            // 2: disturb if monster

            // stop next to some things
            foreach (Vec testPos in GetLeadingTiles(pos, mDirection))
            {
                // items
                if (dungeon.Items.GetAt(testPos) != null)
                {
                    // found an item
                    disturb = true;
                    break;
                }

                // interesting floor features
                TileType tileType = dungeon.Tiles[testPos].Type;
                if ((tileType != TileType.Floor) &&
                    (tileType != TileType.Wall) &&
                    (tileType != TileType.LowWall) &&
                    (tileType != TileType.RoofDark) &&
                    (tileType != TileType.RoofLight))
                {
                    // found something other than wall or floor
                    disturb = true;
                    break;
                }
            }

            // stop one tile away from monsters
            foreach (Vec testPos in GetLeadingTiles(nextPos, mDirection))
            {
                //### bob: need to take into account visibility
                if (dungeon.Entities.GetAt(testPos) != null)
                {
                    // found a monster
                    disturb = true;
                    break;
                }
            }

            // stop if the wall on either side changed
            bool isLeftOpen = dungeon.Tiles[pos + mDirection.RotateLeft90].IsPassable;
            bool isRightOpen = dungeon.Tiles[pos + mDirection.RotateRight90].IsPassable;
            if ((isLeftOpen != mIsLeftOpen) || (isRightOpen != mIsRightOpen))
            {
                disturb = true;
            }

            // stop running if we found something interesting
            if (disturb)
            {
                mDirection = Direction.None;
            }

            return action;
        }

        public override void Disturb()
        {
            // stop running
            mDirection = Direction.None;
        }

        /// <summary>
        /// Called when the user has indicated they want the current Behavior to cancel.
        /// </summary>
        public override void Cancel()
        {
            // stop running
            mDirection = Direction.None;
        }

        /// <summary>
        /// Gets the collection of new tiles seen when moving to the given position
        /// from the given direction.
        /// </summary>
        private IList<Vec> GetLeadingTiles(Vec pos, Direction direction)
        {
            // see if we're moving straight or diagonal
            if (direction.Offset.RookLength == 2)
            {
                // moving diagonally, touches 5 new tiles:
                // ..yyy
                // .xx@y
                // .xxxy
                // .xxx.
                return new List<Vec> {  pos + direction.Previous.Previous,
                                        pos + direction.Previous,
                                        pos + direction,
                                        pos + direction.Next,
                                        pos + direction.Next.Next };
            }
            else
            {
                // moving straight, touches 3 new tiles:
                // .xxxy
                // .xx@y
                // .xxxy
                return new List<Vec> {  pos + direction.Previous,
                                        pos + direction,
                                        pos + direction.Next };
            }
        }

        private Direction mDirection;
        private bool mIsLeftOpen;
        private bool mIsRightOpen;
    }
}
