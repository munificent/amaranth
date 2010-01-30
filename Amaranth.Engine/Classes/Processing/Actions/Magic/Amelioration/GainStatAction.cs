using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Action"/> for raising a single one of the <see cref="Hero"/>'s <see cref="Stats"/>.
    /// </summary>
    public class GainStatAction : Action
    {
        public GainStatAction(Hero hero, Stat stat)
            : base(hero)
        {
            mStat = stat;
        }

        protected override ActionResult OnProcess()
        {
            Hero hero = (Hero)Entity;

            // restore to max
            mStat.Restore();

            if (mStat.Base < Stat.BaseMax)
            {
                if (mStat == hero.Stats.Strength) Log(LogType.PermanentGood, "{subject} feel[s] mighty!");
                else if (mStat == hero.Stats.Agility) Log(LogType.PermanentGood, "{subject} feel[s] nimble!");
                else if (mStat == hero.Stats.Stamina) Log(LogType.PermanentGood, "{subject} feel[s] your endurance increase!");
                else if (mStat == hero.Stats.Will) Log(LogType.PermanentGood, "{subject} feel[s] your resolve harden!");
                else if (mStat == hero.Stats.Intellect) Log(LogType.PermanentGood, "{subject} feel[s] your wisdom deepen!");
                else if (mStat == hero.Stats.Charisma) Log(LogType.PermanentGood, "{subject} feel[s] beautiful!");
                else throw new Exception("Unknown stat \"" + mStat.Name + "\".");

                mStat.Base++;
            }
            else
            {
                if (mStat == hero.Stats.Strength) Log(LogType.DidNotWork, "{subject} do[es]n't feel any stronger.");
                else if (mStat == hero.Stats.Agility) Log(LogType.DidNotWork, "{subject} do[es]n't feel any more graceful.");
                else if (mStat == hero.Stats.Stamina) Log(LogType.DidNotWork, "{subject} do[es]n't feel any tougher.");
                else if (mStat == hero.Stats.Will) Log(LogType.DidNotWork, "{subject} do[es]n't feel any more courageous.");
                else if (mStat == hero.Stats.Intellect) Log(LogType.DidNotWork, "{subject} do[es]n't feel any smarter.");
                else if (mStat == hero.Stats.Charisma) Log(LogType.DidNotWork, "{subject} do[es]n't feel any prettier.");
                else throw new Exception("Unknown stat \"" + mStat.Name + "\".");
            }

            return ActionResult.Done;
        }

        private Stat mStat;
    }
}
