using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for a <see cref="StatBase"/> whose value is relatively unchanging.
    /// This means <see cref="Current"/> includes the bonuses.
    /// </summary>
    [Serializable]
    public abstract class FixedStat : StatBase
    {
        /// <summary>
        /// Gets and sets the current Stat value. Cannot be lowered
        /// below <see cref="BaseMin"/>. If raised above <see cref="Max"/>,
        /// Max will be raised too, but not beyond <see cref="BaseMax"/>.
        /// </summary>
        public int Current
        {
            get
            {
                int current = Base + BonusTotal;

                current = current.Clamp(GetTotalMin(), GetTotalMax());

                return current;
            }
        }

        /// <summary>
        /// Initializes a new instance of FixedStat.
        /// </summary>
        /// <param name="baseValue">The Base FixedStat value.</param>
        public FixedStat(int baseValue)
            : base(baseValue)
        {
        }

        /// <summary>
        /// Override this to return the minimum value the <see cref="Current"/> can have.
        /// </summary>
        protected abstract int GetTotalMin();

        /// <summary>
        /// Override this to return the maximum value the <see cref="Current"/> can have.
        /// </summary>
        protected abstract int GetTotalMax();
    }
}
