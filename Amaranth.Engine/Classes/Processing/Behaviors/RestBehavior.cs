using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class RestBehavior : HeroBehavior
    {
        public override bool NeedsUserInput
        {
            get
            {
                //### bob: this code is also in RunBehavior, refactor out
                //### bob: eventually should stop when one comes into view
                // stop if we're next to a monster
                foreach (Direction direction in Direction.Clockwise)
                {
                    if (Hero.Dungeon.Entities.GetAt(Hero.Position + direction.Offset) != null)
                    {
                        mResting = false;
                        break;
                    }
                }

                // stop if we're all healed up
                if (mResting && Hero.Health.IsMax)
                {
                    mResting = false;
                }

                return !mResting;
            }
        }

        public RestBehavior(Hero hero)
            : base(hero)
        {
            mResting = true;
        }

        public override Action NextAction()
        {
            return new WalkAction(Hero, Direction.None, true);
        }

        public override void Disturb()
        {
            mResting = false;
        }

        /// <summary>
        /// Called when the user has indicated they want the current Behavior to cancel.
        /// </summary>
        public override void Cancel()
        {
            mResting = false;
        }

        private bool mResting;
    }
}
