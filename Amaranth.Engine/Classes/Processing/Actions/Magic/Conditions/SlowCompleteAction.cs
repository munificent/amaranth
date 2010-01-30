using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class SlowCompleteAction : Action
    {
        public SlowCompleteAction(Entity entity) : base(entity) { }

        protected override ActionResult OnProcess()
        {
            Entity.Speed.SetBonus(BonusType.Slow, 0);

            Log(LogType.WearOff, "{subject} speed[s] back up.");

            return ActionResult.Done;
        }
    }
}
