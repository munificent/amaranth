using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class Stat : FixedStat
    {
        public static implicit operator int(Stat stat)
        {
            return stat.Current;
        }

        public const int TotalMin = 1;
        public const int TotalMax = 60;

        public const int BaseMin = 1;
        public const int BaseMax = 40;

        /// <summary>
        /// Gets the name of this Stat.
        /// </summary>
        public string Name { get { return GetType().Name; } }

        /// <summary>
        /// Initializes a new instance of Stat.
        /// </summary>
        /// <param name="baseValue">The Base Stat value.</param>
        public Stat(int baseValue)
            : base(baseValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of Stat with a random Base.
        /// </summary>
        public Stat() : this(10)
        {
        }

        /// <summary>
        /// Overridden from <see cref="StatBase"/>.
        /// </summary>
        protected override int GetBaseMin()
        {
            return BaseMin;
        }

        /// <summary>
        /// Overridden from <see cref="StatBase"/>.
        /// </summary>
        protected override int GetBaseMax()
        {
            return BaseMax;
        }

        /// <summary>
        /// Overridden from <see cref="FixedStat"/>.
        /// </summary>
        protected override int GetTotalMin()
        {
            return TotalMin;
        }

        /// <summary>
        /// Overridden from <see cref="FixedStat"/>.
        /// </summary>
        protected override int GetTotalMax()
        {
            return TotalMax;
        }
    }
}
