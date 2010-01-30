using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Action"/> for restoring all of the <see cref="Hero"/>'s <see cref="Stats"/>.
    /// </summary>
    public class RestoreAllAction : Action
    {
        public RestoreAllAction(Hero hero)
            : base(hero)
        {
        }

        protected override ActionResult OnProcess()
        {
            Hero hero = (Hero)Entity;

            hero.Stats.Strength.Restore();
            hero.Stats.Agility.Restore();
            hero.Stats.Stamina.Restore();
            hero.Stats.Will.Restore();
            hero.Stats.Intellect.Restore();
            hero.Stats.Charisma.Restore();

            Log(LogType.TemporaryGood, "{subject} feel[s] {possessive} abilities returning.");

            return ActionResult.Done;
        }
    }
}
