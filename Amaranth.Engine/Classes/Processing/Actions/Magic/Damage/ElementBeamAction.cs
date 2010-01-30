using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Beam action that applies <see cref="Element"/> damage to every tile hit.
    /// A beam is similar to a bolt, except that it will travel through entities.
    /// </summary>
    public class ElementBeamAction : BoltAction
    {
        /// <summary>
        /// Creates a new ElementBeamAction.
        /// </summary>
        public ElementBeamAction(NotNull<Entity> entity, Vec target, INoun noun, Attack attack)
            : base(entity, target)
        {
            mNoun = noun;
            mAttack = attack;
        }

        protected override bool OnEffect(Vec pos, Direction direction)
        {
            AddEffect(new Effect(pos, direction, mAttack.EffectType, mAttack.Element));

            Dungeon.HitAt(pos, this, new Hit(mNoun, mAttack, direction));

            // stop if we hit a wall
            return !Dungeon.Tiles[pos].IsPassable;
        }

        private INoun mNoun;
        private Attack mAttack;
    }
}
