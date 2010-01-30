using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class PoisonAction : Action
    {
        /// <summary>
        /// Initializes a new PoisonAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being poisoned.</param>
        /// <param name="damage">The amount of damage from the hit that caused the disease.</param>
        public PoisonAction(Entity entity, int damage)
            : base(entity)
        {
            //### bob: needs tuning.
            mDuration = damage;
        }

        protected override ActionResult OnProcess()
        {
            //### bob: chance to resist based on stamina
            Log(LogType.BadState, "{subject} [are|is] poisoned!");

            // only poison up to 1/5 the entity's health
            Entity.Conditions.Poison.SetDuration(Math.Min(Entity.Conditions.Poison.TurnsRemaining + mDuration, Entity.Health.Max / 5));

            return ActionResult.Done;
        }

        private int mDuration;
    }
}
