using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class PoisonDamageAction : Action
    {
        /// <summary>
        /// Initializes a new PoisonDamageAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being poisoned.</param>
        /// <param name="damage">The amount of damage from the hit that caused the disease.</param>
        public PoisonDamageAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            Log(LogType.BadState, "{subject} [are|is] harmed by poison.");

            Entity.Health.Current--;
            Entity.Behavior.Disturb();

            return ActionResult.Done;
        }
    }
}
