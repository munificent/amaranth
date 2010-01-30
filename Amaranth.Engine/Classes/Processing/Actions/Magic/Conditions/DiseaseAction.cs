using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class DiseaseAction : Action
    {
        /// <summary>
        /// Initializes a new DiseaseAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being diseased.</param>
        /// <param name="damage">The amount of damage from the hit that caused the disease.</param>
        public DiseaseAction(Entity entity, int damage)
            : base(entity)
        {
            mDamage = damage;
        }

        protected override ActionResult OnProcess()
        {
            //### bob: chance to resist based on stamina

            // figure out how much worse to make it
            int disease = Math.Max(1, mDamage / 6);

            Entity.Health.AddBonus(BonusType.Disease, -disease);

            Log(LogType.BadState, "{subject} [are|is] racked with disease!");

            return ActionResult.Done;
        }

        private int mDamage;
    }
}
