using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for <see cref="Stat"/> and <see cref="FluidStat"/>.
    /// </summary>
    [Serializable]
    public abstract class StatBase
    {
        /// <summary>
        /// Raised when the <see cref="Base"/> value or any bonuses change.
        /// </summary>
        internal event EventHandler Changed;

        /// <summary>
        /// Raised when any of the bonuses change.
        /// </summary>
        internal event EventHandler BonusChanged;

        /// <summary>
        /// Gets and sets the base value the stat has before taking
        /// bonuses into account.
        /// </summary>
        public int Base
        {
            get { return mBase; }
            set
            {
                // keep it in bounds
                value = Math2.Clamp(GetBaseMin(), value, GetBaseMax());

                if (mBase != value)
                {
                    mBase = value;

                    OnBaseChanged();
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets the total value of all bonuses.
        /// </summary>
        public int BonusTotal
        {
            get { return mBonuses.Values.Sum(); }
        }

        /// <summary>
        /// Gets whether there are any negative bonuses currently applied.
        /// </summary>
        public bool IsLowered
        {
            get
            {
                return mBonuses.Values.Any((value) => value < 0);
            }
        }

        /// <summary>
        /// Gets whether there are any positive bonuses currently applied.
        /// </summary>
        public bool IsRaised
        {
            get
            {
                return mBonuses.Values.Any((value) => value > 0);
            }
        }

        /// <summary>
        /// Initializes a new instance of StatBase.
        /// </summary>
        /// <param name="baseValue">The Base Stat value.</param>
        public StatBase(int baseValue)
        {
            mBase = Math2.Clamp(GetBaseMin(), baseValue, GetBaseMax());
        }

        /// <summary>
        /// Gets the whether the current bonus type is zero or not.
        /// </summary>
        /// <param name="bonus">Which bonus to get.</param>
        /// <returns><c>true</c> if the bonus is not zero.</returns>
        public bool HasBonus(BonusType bonus)
        {
            if (mBonuses.ContainsKey(bonus))
            {
                return mBonuses[bonus] != 0;
            }

            // not set
            return false;
        }

        /// <summary>
        /// Gets the current value of the given bonus type.
        /// </summary>
        /// <param name="bonus">Which bonus to get.</param>
        /// <returns>The bonus's current value. Defaults to 0 if never set.</returns>
        public int GetBonus(BonusType bonus)
        {
            if (mBonuses.ContainsKey(bonus))
            {
                return mBonuses[bonus];
            }

            // not set
            return 0;
        }

        /// <summary>
        /// Sets the given bonus type to the given value. Replaces any previous
        /// bonus value of that type.
        /// </summary>
        /// <param name="bonus">Type of bonus to set.</param>
        /// <param name="value">New bonus value.</param>
        public void SetBonus(BonusType bonus, int value)
        {
            // create the value if not there
            if (!mBonuses.ContainsKey(bonus))
            {
                mBonuses[bonus] = 0;
            }

            // set the bonus if different
            if (mBonuses[bonus] != value)
            {
                mBonuses[bonus] = value;

                OnBonusChanged();
                OnChanged();
            }
        }

        /// <summary>
        /// Adds the given amount to the given bonus type.
        /// </summary>
        /// <param name="bonus">Type of bonus to set.</param>
        /// <param name="value">Amount to add to the current bonus value.</param>
        public void AddBonus(BonusType bonus, int value)
        {
            // bail if not changing
            if (value == 0) return;

            // create the value if not there
            if (!mBonuses.ContainsKey(bonus))
            {
                mBonuses[bonus] = 0;
            }

            // add the value
            mBonuses[bonus] += value;

            OnBonusChanged();
            OnChanged();
        }

        /// <summary>
        /// Eliminates any negative bonuses currently affecting the Stat.
        /// </summary>
        /// <returns><c>true</c> if a bonus was changed.</returns>
        public bool Restore()
        {
            List<BonusType> toRemove = new List<BonusType>();

            foreach (KeyValuePair<BonusType, int> pair in mBonuses)
            {
                if (pair.Value < 0)
                {
                    toRemove.Add(pair.Key);
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (BonusType bonus in toRemove)
                {
                    mBonuses.Remove(bonus);
                }

                OnBonusChanged();
                OnChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        protected void OnChanged()
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="BonusChanged"/> event. Also lets the derived class perform any validation needed.
        /// </summary>
        protected virtual void OnBonusChanged()
        {
            if (BonusChanged != null) BonusChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Override this to return the minimum value the <see cref="Base"/> can have.
        /// </summary>
        protected abstract int GetBaseMin();

        /// <summary>
        /// Override this to return the maximum value the <see cref="Base"/> can have.
        /// </summary>
        protected abstract int GetBaseMax();

        /// <summary>
        /// Called when <see cref="Base"/> has changed to let the derived class perform any validation needed.
        /// </summary>
        protected virtual void OnBaseChanged()
        {
            // do nothing
        }

        private int mBase;
        private readonly Dictionary<BonusType, int> mBonuses = new Dictionary<BonusType, int>();
    }
}
