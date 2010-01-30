using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class HealAction : Action
    {
        public HealAction(Entity entity, Attack amount)
            : base(entity)
        {
            mAmount = amount;
        }

        protected override ActionResult OnProcess()
        {
            Entity.Health.Current += mAmount.Roll();

            Log(LogType.TemporaryGood, "{subject} feel[s] better.");

            return ActionResult.Done;
        }

        private Attack mAmount;
    }
}
