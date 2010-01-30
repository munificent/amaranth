using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Causes the Freeze condition usually as a result of a Cold attack.
    /// </summary>
    public class FreezeAction : Action
    {
        /// <summary>
        /// Initializes a new FreezeAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being frozen.</param>
        /// <param name="duration">How much longer the Entity should stay frozen.</param>
        public FreezeAction(Entity entity, int duration)
            : base(entity)
        {
            mDuration = duration;
        }

        protected override ActionResult OnProcess()
        {
            Log(LogType.BadState, "{subject} [are|is] frozen solid!");

            // set the speed
            Entity.Speed.SetBonus(BonusType.Freeze, -3);

            Entity.Conditions.Freeze.AddDuration(mDuration);

            return ActionResult.Done;
        }

        private int mDuration;
    }
}
