using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class PoisonCompleteAction : Action
    {
        public PoisonCompleteAction(Entity entity) : base(entity) { }

        protected override ActionResult OnProcess()
        {
            Log(LogType.WearOff, "{subject} [are|is] no longer poisoned.");

            return ActionResult.Done;
        }
    }
}
