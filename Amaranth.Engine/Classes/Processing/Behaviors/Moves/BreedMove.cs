using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class BreedMove : Move<BreedMoveInfo>
    {
        public override string Description
        {
            get { return "It ^obreeds^-."; }
        }

        public override float GetExperience(Race race)
        {
            //### bob: totally random
            return 10.0f;
        }

        protected override bool WillUseMove(Monster monster, Entity target, BreedMoveInfo info)
        {
            // the odds of breeding decays exponentially
            int chance = info.Generation * info.Generation;

            // random chance to breed
            return Rng.OneIn(chance);
        }

        protected override Action GetAction(Monster monster, Entity target, BreedMoveInfo info)
        {
            return new BreedAction(monster, info);
        }
    }

    [Serializable]
    public class BreedMoveInfo
    {
        public int Generation = 1;
    }
}
