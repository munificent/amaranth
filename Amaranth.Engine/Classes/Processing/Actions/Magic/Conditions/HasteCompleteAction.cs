using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class HasteCompleteAction : Action
    {
        public HasteCompleteAction(Entity entity) : base(entity) { }

        protected override ActionResult OnProcess()
        {
            Entity.Speed.SetBonus(BonusType.Haste, 0);

            Log(LogType.WearOff, "{subject} return[s] to normal speed.");

            return ActionResult.Done;
        }
    }
}
