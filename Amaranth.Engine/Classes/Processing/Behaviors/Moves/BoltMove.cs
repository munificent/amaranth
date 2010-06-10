using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Move"/> for firing an elemental bolt.
    /// </summary>
    public class BoltMove : Move
    {
        public override string Description
        {
            get { return "It ^ofires " + Info.Noun.NounText + "^-."; }
        }

        public override bool WillUseMove(Monster monster, Entity target)
        {
            // don't try if too far
            if (!Vec.IsDistanceWithin(monster.Position, target.Position, Info.Radius)) return false;

            // see if we can see a clear path to the target
            Los los = new Los(monster.Dungeon, monster.Position, target.Position);

            return los.HitsEntity(target);
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            Attack attack = new Attack(Info.Damage, Info.Element, Info.Verb, Info.Effect);
            return new ElementBoltAction(monster, target.Position, Info.Noun, attack);
        }

        public override float GetExperience(Race race)
        {
            // start with the damage
            float exp = Info.Damage.Average;

            // modify by the element
            exp *= Info.Element.AttackExperience();

            return exp;
        }
    }
}
