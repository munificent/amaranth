using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Represents a magical power that can be added to an <see cref="Item"/>.
    /// </summary>
    [Serializable]
    public class Power
    {
        public string Name { get { return Type.Name; } }

        public object Appearance { get { return Type.Appearance; } }

        /// <summary>
        /// Gets the strike bonus provided by this Power.
        /// </summary>
        public int StrikeBonus { get { return mStrikeBonus; } }

        /// <summary>
        /// Gets the damage bonus provided by this Power.
        /// </summary>
        public float DamageBonus { get { return mDamageBonus; } }

        /// <summary>
        /// Gets the armor bonus provided by this Power.
        /// </summary>
        public int ArmorBonus { get { return mArmorBonus; } }

        /// <summary>
        /// Gets the <see cref="Stat"/> bonus provided by this Power.
        /// </summary>
        public int StatBonus { get { return mStatBonus; } }

        /// <summary>
        /// Gets the <see cref="Speed"/> bonus provided by this Power.
        /// </summary>
        public int SpeedBonus { get { return mSpeedBonus; } }

        public PowerType Type { get { return mType.Value; } }

        public FlagCollection Flags { get { return Type.Flags; } }

        public Power(PowerType type)
        {
            mType = type;

            // roll the power
            mStrikeBonus = type.StrikeBonus.Roll();
            mDamageBonus = type.DamageBonus.Roll() / 10.0f;
            mArmorBonus = type.ArmorBonus.Roll();
            mStatBonus = type.StatBonus.Roll();
            mSpeedBonus = type.SpeedBonus.Roll();
        }

        private PowerTypeRef mType;

        private int mStrikeBonus;
        private float mDamageBonus;
        private int mArmorBonus;
        private int mStatBonus;
        private int mSpeedBonus;
    }
}
