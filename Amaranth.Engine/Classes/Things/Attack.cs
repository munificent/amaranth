using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// At attack performed on some Thing.
    /// </summary>
    public class Attack
    {
        public string Verb;
        public EffectType EffectType;

        /// <summary>
        /// Gets the average damage inflicted by this attack, including the <see cref="DamageBonus"/>.
        /// </summary>
        public float Average { get { return (int)Math.Round(mDamage.Average * mDamageBonus); } }

        public int StrikeBonus
        {
            get { return mStrikeBonus; }
            set { mStrikeBonus = value; }
        }

        public float DamageBonus
        {
            get { return mDamageBonus; }
            set { mDamageBonus = value; }
        }

        public Roller Damage { get { return mDamage; } }

        public Element Element
        {
            get { return mElement; }
            set { mElement = value; }
        }

        /// <summary>
        /// Gets the flags that apply to this Attack.
        /// </summary>
        public IFlagCollection Flags { get { return mFlags; } }

        public Attack(Roller damage, int strikeBonus, float damageBonus,
            Element element, string verb, EffectType effectType,
            IFlagCollection flags)
        {
            mDamage = damage;
            mStrikeBonus = strikeBonus;
            mDamageBonus = damageBonus;
            mElement = element;

            Verb = verb;
            EffectType = effectType;
            mFlags = flags;
        }

        public Attack(Roller damage, Element element, string verb, EffectType effectType)
            : this(damage, 0, 1.0f, element, verb, effectType, new FlagCollection())
        {
        }

        public Attack(Roller damage, string verb, EffectType effectType)
            : this(damage, 0, 1.0f, Element.Anima, verb, effectType, new FlagCollection())
        {
        }

        public Attack(Roller damage, Element element, string verb)
            : this(damage, 0, 1.0f, Element.Anima, verb, EffectType.Hit, new FlagCollection())
        {
        }

        public Attack(Attack attack, Element element)
            : this(attack.Damage, attack.StrikeBonus, attack.DamageBonus,
                   element, attack.Verb, attack.EffectType, attack.Flags)
        {
        }

        public int Roll()
        {
            return (int)Math.Round(mDamage.Roll() * mDamageBonus);
        }

        private Roller mDamage;
        private int mStrikeBonus;
        private float mDamageBonus;
        private Element mElement;
        private readonly IFlagCollection mFlags;
    }
}
