using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// An <see cref="ElementConeMove"/> where the damage is bound to the Health of
    /// the <see cref="Entity"/> breathing.
    /// </summary>
    public class BreatheMove : ElementConeMove
    {
        public override string Description
        {
            get { return "It ^obreathes " + Info.Element.ToString().ToLower() + "^-."; }
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            Attack attack = new Attack(Roller.Fixed(monster.Health.Current), Info.Element, Info.Verb, Info.Effect);
            return new ElementConeAction(monster, target.Position, Info.Radius, Info.Noun, attack);
        }

        public override float GetExperience(Race race)
        {
            // start with the damage
            // reduce by half because the breath only does full damage when the monster has full health
            float exp = race.Health.Average * 0.5f;

            // modify by the element
            exp *= Info.Element.AttackExperience();

            // a larger radius is a little more dangerous
            exp *= (1.0f + Info.Radius / 12.0f);

            return exp;
        }
    }
}
