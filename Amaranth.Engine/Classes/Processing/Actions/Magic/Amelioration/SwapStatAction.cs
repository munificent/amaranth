using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Action"/> for taking a point from one of the <see cref="Hero"/>'s <see cref="Stats"/> and adding it to
    /// a given one.
    /// </summary>
    public class SwapStatAction : Action
    {
        public SwapStatAction(Hero hero, Stat stat)
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
                // pick a stat to drain
                int i = Rng.Int(hero.Stats.Count - 1);
                if (hero.Stats[i] == mStat)
                {
                    // picked the stat being raised, so skip it
                    i++;
                }

                // drain one
                hero.Stats[i].Base--;

                // to raise another
                mStat.Base++;

                if (mStat == hero.Stats.Strength) Log(LogType.PermanentGood, "{subject} feel[s] filled with brute strength!");
                else if (mStat == hero.Stats.Agility) Log(LogType.PermanentGood, "{subject} feel[s] thin and nimble!");
                else if (mStat == hero.Stats.Stamina) Log(LogType.PermanentGood, "{subject} feel[s] solid as a rock!");
                else if (mStat == hero.Stats.Will) Log(LogType.PermanentGood, "{subject} feel[s] filled with blind courage!");
                else if (mStat == hero.Stats.Intellect) Log(LogType.PermanentGood, "{subject} feel[s] clever!");
                else if (mStat == hero.Stats.Charisma) Log(LogType.PermanentGood, "{subject} feel[s] pretty!");
                else throw new Exception("Unknown stat \"" + mStat.Name + "\".");
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
