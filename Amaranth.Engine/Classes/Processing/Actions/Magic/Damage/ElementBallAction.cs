using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="Action"/> for an exploding circle of elemental damage, like a fireball.
    /// </summary>
    public class ElementBallAction : BallAction
    {
        public ElementBallAction(Entity owner, Vec center, int radius, INoun noun, Attack attack)
            : base(owner, center, radius)
        {
            mNoun = noun;
            mAttack = attack;
        }

        protected override void OnEffect(Vec pos, Direction direction, bool leadingEdge)
        {
            if (leadingEdge)
            {
                AddEffect(new Effect(pos, EffectType.Ball, mAttack.Element));

                Dungeon.HitAt(pos, this, new Hit(mNoun, mAttack, direction));
            }
            else
            {
                AddEffect(new Effect(pos, EffectType.BallTrail, mAttack.Element));
            }
        }

        private INoun mNoun;
        private Attack mAttack;
    }
}
