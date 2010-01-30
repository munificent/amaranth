using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Move"/> for a cone of elemental damage.
    /// </summary>
    public class ElementConeMove : Move
    {
        public override string Description
        {
            get { return "It ^ofires " + Info.Noun.NounText + "^-."; }
        }

        public override bool WillUseMove(Monster monster, Entity target)
        {
            Los los = new Los(monster.Dungeon, monster.Position, target.Position);

            // don't try if the target is too far away
            if (!Vec.IsDistanceWithin(target.Position, monster.Position, Info.Radius)) return false;

            // see if we can see a clear path to the target
            return los.HitsEntity(target, true);
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            Attack attack = new Attack(Info.Damage, Info.Element, Info.Verb);
            return new ElementConeAction(monster, target.Position, Info.Radius, Info.Noun, attack);
        }

        public override float GetExperience(Race race)
        {
            // start with the damage
            float exp = Info.Damage.Average;

            // modify by the element
            exp *= Info.Element.AttackExperience();

            // a larger radius is a little more dangerous
            exp *= (1.0f + Info.Radius / 12.0f);

            return exp;
        }
    }
}
