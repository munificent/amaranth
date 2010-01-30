using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Bolt action that applies <see cref="Element"/> damage to every tile hit.
    /// </summary>
    public class ElementBoltAction : BoltAction
    {
        /// <summary>
        /// Creates a new ElementBoltAction.
        /// </summary>
        public ElementBoltAction(NotNull<Entity> entity, Vec target, INoun noun, Attack attack)
            : base(entity, target)
        {
            mNoun = noun;
            mAttack = attack;
        }

        protected override bool OnEffect(Vec pos, Direction direction)
        {
            AddEffect(new Effect(pos, direction, mAttack.EffectType, mAttack.Element));

            return Dungeon.HitAt(pos, this, new Hit(mNoun, mAttack, direction));
        }

        private INoun mNoun;
        private Attack mAttack;
    }
}
