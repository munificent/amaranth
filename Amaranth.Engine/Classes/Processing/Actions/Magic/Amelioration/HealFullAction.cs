using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class HealFullAction : Action
    {
        public HealFullAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            Entity.Health.Current = Entity.Health.Max;

            Log(LogType.TemporaryGood, "{subject} feel[s] fully rejuvenated.");

            return ActionResult.Done;
        }
    }
}
