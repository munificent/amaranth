using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Action to increase the max (and current) health of the <see cref="Entity"/>.
    /// </summary>
    public class GainHealthAction : Action
    {
        public GainHealthAction(Entity entity, Attack amount)
            : base(entity)
        {
            mAmount = amount;
        }

        protected override ActionResult OnProcess()
        {
            int amount = mAmount.Roll();

            Entity.Health.AddBonus(BonusType.Permanent, amount);
            Entity.Health.Current += amount;

            Log(LogType.PermanentGood, "{subject} feel[s] suffused with health!");

            return ActionResult.Done;
        }

        private Attack mAmount;
    }
}
