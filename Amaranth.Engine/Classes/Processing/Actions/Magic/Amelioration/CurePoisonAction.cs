using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class CurePoisonAction : Action
    {
        /// <summary>
        /// Initializes a new CurePoisonAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being cured.</param>
        public CurePoisonAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            // see if the entity actually is poisoned
            if (Entity.Conditions.Poison.IsActive)
            {
                Log(LogType.TemporaryGood, "{subject} feel[s] much better.");

                AddAction(Entity.Conditions.Poison.Deactivate());
            }
            else
            {
                Log(LogType.DidNotWork, "{subject} [are|is] not poisoned.");
            }

            return ActionResult.Done;
        }
    }
}
