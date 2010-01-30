using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class FreezeCompleteAction : Action
    {
        public FreezeCompleteAction(Entity entity) : base(entity) { }

        protected override ActionResult OnProcess()
        {
            Entity.Speed.SetBonus(BonusType.Freeze, 0);

            Log(LogType.WearOff, "{subject} warm[s] back up.");

            return ActionResult.Done;
        }
    }
}
