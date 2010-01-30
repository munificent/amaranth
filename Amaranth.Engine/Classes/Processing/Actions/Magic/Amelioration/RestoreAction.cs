using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Action"/> for restoring a single one of the <see cref="Hero"/>'s <see cref="Stats"/>.
    /// </summary>
    public class RestoreAction : Action
    {
        public RestoreAction(Hero hero, Stat stat)
            : base(hero)
        {
            mStat = stat;
        }

        protected override ActionResult OnProcess()
        {
            Hero hero = (Hero)Entity;

            if (mStat.Restore())
            {
                if (mStat == hero.Stats.Strength) Log(LogType.TemporaryGood, "{subject} feel[s] {possessive} strength returning.");
                else if (mStat == hero.Stats.Agility) Log(LogType.TemporaryGood, "{subject} feel[s] {possessive} dexterity is restored.");
                else if (mStat == hero.Stats.Stamina) Log(LogType.TemporaryGood, "{subject} feel[s] {possessive} endurance returning.");
                else if (mStat == hero.Stats.Will) Log(LogType.TemporaryGood, "{subject} feel[s] your {possessive} harden.");
                else if (mStat == hero.Stats.Intellect) Log(LogType.TemporaryGood, "{subject} feel[s] {possessive} wisdom is restored.");
                else if (mStat == hero.Stats.Charisma) Log(LogType.TemporaryGood, "{subject} feel[s] {possessive} beauty is restored.");
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
