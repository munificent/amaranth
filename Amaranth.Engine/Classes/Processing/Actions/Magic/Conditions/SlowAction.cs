using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class SlowAction : Action
    {
        public SlowAction(Entity entity, int damage)
            : base(entity)
        {
            mDamage = damage;
        }

        protected override ActionResult OnProcess()
        {
            Log(LogType.BadState, "{subject} feel[s] sluggish.");

            // set the speed
            Entity.Speed.SetBonus(BonusType.Slow, -3);
            Entity.Conditions.Slow.AddDuration(20 + (mDamage * 4));

            return ActionResult.Done;
        }

        private int mDamage;
    }
}
