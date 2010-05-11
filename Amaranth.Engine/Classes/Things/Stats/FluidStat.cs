using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A <see cref="Hero"/> statistic that can be freely spent and restored,
    /// like Health or Mana.
    /// </summary>
    [Serializable]
    public class FluidStat : StatBase
    {
        /// <summary>
        /// Gets the minimum value the FluidStat can have.
        /// </summary>
        public int Min { get { return 0; } }

        /// <summary>
        /// Gets the maximum value the FluidStat can have, starting with the
        /// <see cref="Base"/> and including any bonuses.
        /// </summary>
        public int Max
        {
            get
            {
                return Base + BonusTotal;
            }
        }

        /// <summary>
        /// Gets the FluidStat's current value, between <see cref="Min"/>
        /// and <see cref="Max"/>.
        /// </summary>
        public int Current
        {
            get { return mCurrent; }
            set
            {
                // keep it in bounds
                value = value.Clamp(Min, Max);

                if (mCurrent != value)
                {
                    mCurrent = value;

                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets whether or not the <see cref="Current"/> value is maxed out.
        /// </summary>
        public bool IsMax { get { return Current == Max; } }

        /// <summary>
        /// Initializes a new instance of FluidStat.
        /// </summary>
        /// <param name="baseValue">The base maximum stat value.</param>
        public FluidStat(int baseValue)
            : base(baseValue)
        {
            mCurrent = Base;
        }

        /// <summary>
        /// Overridden from <see cref="StatBase"/>.
        /// </summary>
        protected override int GetBaseMin()
        {
            return Min;
        }

        /// <summary>
        /// Overridden from <see cref="StatBase"/>.
        /// </summary>
        protected override int GetBaseMax()
        {
            // no real maximum for max fluid stats
            return Int32.MaxValue;
        }

        /// <summary>
        /// Overridden from <see cref="StatBase"/>.
        /// </summary>
        protected override void OnBaseChanged()
        {
            KeepCurrentInBounds();
        }

        /// <summary>
        /// Overridden from <see cref="StatBase"/>.
        /// </summary>
        protected override void OnBonusChanged()
        {
            base.OnBonusChanged();

            KeepCurrentInBounds();
        }

        private void KeepCurrentInBounds()
        {
            mCurrent = mCurrent.Clamp(Min, Max);
        }

        private int mCurrent;
    }
}
