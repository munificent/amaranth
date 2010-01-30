using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class CureDiseaseAction : Action
    {
        /// <summary>
        /// Initializes a new CureDiseaseAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being cured.</param>
        public CureDiseaseAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            // see if the entity actually is diseased
            if (Entity.Health.HasBonus(BonusType.Disease))
            {
                Entity.Health.SetBonus(BonusType.Disease, 0);

                Log(LogType.TemporaryGood, "{subject} feel[s] much better.");
            }
            else
            {
                Log(LogType.DidNotWork, "{subject} [are|is] not ill.");
            }

            return ActionResult.Done;
        }
    }
}
