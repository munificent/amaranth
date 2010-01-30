using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Bundles the data required for one <see cref="Thing"/> to hit another.
    /// </summary>
    public class Hit
    {
        public Attack Attack;
        public INoun Attacker;
        public bool CanDodge;
        public Direction Direction;

        /// <summary>
        /// Gets the damage this Hit applied to the defender. Only available after the Hit has been applied.
        /// </summary>
        public int Damage
        {
            get
            {
                if (!mDamage.HasValue) throw new InvalidOperationException("Cannot access the Hit's Damage before it has been set by the defender.");

                return mDamage.Value;
            }
        }

        /// <summary>
        /// Initializes a new Hit.
        /// </summary>
        public Hit(INoun attacker, Attack attack, bool canDodge, Direction direction)
        {
            Attacker = attacker;
            Attack = attack;
            CanDodge = canDodge;
            Direction = direction;
        }

        /// <summary>
        /// Initializes a new Hit that cannot be dodged.
        /// </summary>
        /// <param name="damage">Damage the hit does.</param>
        public Hit(INoun attacker, Attack attack, Direction direction)
            : this(attacker, attack, false, direction)
        {
        }

        /// <summary>
        /// Creates an <see cref="Effect"/> appropriate for this Hit at the given position.
        /// </summary>
        /// <param name="position">Position to create the Effect.</param>
        /// <returns></returns>
        public Effect CreateEffect(Vec position)
        {
            return new Effect(position, Direction, Attack.EffectType, Attack.Element);
        }

        public void SetDamage(int damage)
        {
            mDamage = damage;
        }

        private int? mDamage;
    }
}
