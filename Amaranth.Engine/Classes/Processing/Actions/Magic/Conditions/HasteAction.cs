using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class HasteAction : Action
    {
        /// <summary>
        /// Initializes a new HasteAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being hasted.</param>
        /// <param name="duration">How long the Entity should stay hasted.</param>
        /// <param name="boost">How much the Entity should stay hasted by.</param>
        public HasteAction(Entity entity, int duration, int boost)
            : base(entity)
        {
            mDuration = duration;
            mBoost = boost;
        }

        protected override ActionResult OnProcess()
        {
            if (!Entity.Conditions.Haste.IsActive)
            {
                Log(LogType.TemporaryGood, "{subject} start[s] moving faster!");
            }
            else if (Entity.Conditions.Haste.IsActive && (mBoost > Entity.Speed.GetBonus(BonusType.Haste)))
            {
                Log(LogType.TemporaryGood, "{subject} start[s] moving even faster!");
            }

            // set the speed
            Entity.Speed.SetBonus(BonusType.Haste, mBoost);

            Entity.Conditions.Haste.SetDuration(mDuration);

            return ActionResult.Done;
        }

        private int mDuration;
        private int mBoost;
    }
}
