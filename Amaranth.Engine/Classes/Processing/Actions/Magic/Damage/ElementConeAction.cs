using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Action"/> for radiating cone of elemental damage, like a dragon's fire breath.
    /// </summary>
    public class ElementConeAction : ConeAction
    {
        public ElementConeAction(Entity owner, Vec target, int radius, INoun noun, Attack attack)
            : base(owner, target, radius)
        {
            mNoun = noun;
            mAttack = attack;
        }

        protected override void OnEffect(Vec pos, Direction direction, bool leadingEdge)
        {
            if (leadingEdge)
            {
                AddEffect(new Effect(pos, direction, EffectType.Cone, mAttack.Element));

                Dungeon.HitAt(pos, this, new Hit(mNoun, mAttack, direction));
            }
            else
            {
                AddEffect(new Effect(pos, direction, EffectType.ConeTrail, mAttack.Element));
            }
        }

        private INoun mNoun;
        private Attack mAttack;
    }
}
