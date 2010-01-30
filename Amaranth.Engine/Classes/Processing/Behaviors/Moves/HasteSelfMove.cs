using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Move"/> for speeding self up. Will only be used if not already hasted.
    /// </summary>
    public class HasteSelfMove : Move
    {
        public override string Description
        {
            get { return "It ^ohastes^o itself."; }
        }

        public override bool WillUseMove(Monster monster, Entity target)
        {
            // don't haste redundantly
            if (monster.Conditions.Haste.IsActive) return false;

            return true;
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            return new HasteAction(monster, 20, 1);
        }

        public override float GetExperience(Race race)
        {
            //### bob: total guesswork
            return 20;
        }
    }
}
