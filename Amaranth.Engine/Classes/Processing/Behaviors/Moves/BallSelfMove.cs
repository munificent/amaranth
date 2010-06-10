using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Move"/> for creating an element ball centered on the <see cref="Monster"/>.
    /// Will only be used if the target is within range.
    /// </summary>
    public class BallSelfMove : Move
    {
        public override string Description
        {
            get { return "It ^ofires " + Info.Noun.NounText + "^-."; }
        }

        public override bool WillUseMove(Monster monster, Entity target)
        {
            // make sure target is in range
            if (!Vec.IsDistanceWithin(target.Position, monster.Position, Info.Radius)) return false;

            return true;
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            Attack attack = new Attack(Info.Damage, Info.Element, Info.Verb);
            return new ElementBallAction(monster, monster.Position, Info.Radius, Info.Noun, attack);
        }

        public override float GetExperience(Race race)
        {
            // start with the damage
            float exp = Info.Damage.Average;

            // modify by the element
            exp *= Info.Element.AttackExperience();

            // a larger radius is a little more dangerous
            exp *= (1.0f + Info.Radius / 10.0f);

            return exp;
        }
    }
}
