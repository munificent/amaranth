using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// Extension methods on <c>int</c>.
    /// </summary>
    public static class Int32Extensions
    {
        /// <summary>
        /// Clamps the value to be within the given range, inclusive.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns><c>value</c> clamped to the range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <c>min</c> is greater than <c>max</c>.</exception>
        public static int Clamp(this int value, int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("The minimum must be less than or equal to the maximum.");

            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Normalizes the value to range from 0.0 to 1.0 from the given range.
        /// </summary>
        /// <param name="value">The value to map to the given range.</param>
        /// <param name="min">If <c>value</c> is <c>min</c> then 0.0 is returned.</param>
        /// <param name="max">If <c>value</c> is <c>max</c> then 1.0 is returned.</param>
        /// <returns><c>value</c> mapped to the range (min = 0.0, max = 1.0). Will return
        /// values less than 0.0 or greater than 1.0 if <c>value</c> is not within
        /// <c>min</c> and <c>max</c>.</returns>
        public static float Normalize(this int value, int min, int max)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return ((float)value).Normalize(min, max);
        }

        /// <summary>
        /// Remaps the value from within its original range to the corresponding
        /// location in the given second range.
        /// </summary>
        /// <param name="value">The value in the range (<c>min</c>, <c>max</c>).</param>
        /// <param name="min">The minimum of the starting range.</param>
        /// <param name="max">The maximum of the starting range.</param>
        /// <param name="outMin">The minimum out the resulting range.</param>
        /// <param name="outMax">The maximum out the resulting range.</param>
        /// <returns><c>value</c> mapped from the range (<c>min</c>, <c>max</c>) to
        /// (<c>outMin</c>, <c>outMax</c>). Will return values less than <c>outMin</c>
        /// or greater than <c>outMax</c> if <c>value</c> is not within <c>min</c> and
        /// <c>max</c>.</returns>
        public static int Remap(this int value, int min, int max, int outMin, int outMax)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (int)(((float)value).Normalize(min, max) * (outMax - outMin)) + outMin;
        }

        /// <summary>
        /// Remaps the value from within its original range to the corresponding
        /// location in the given second range.
        /// </summary>
        /// <param name="value">The value in the range (<c>min</c>, <c>max</c>).</param>
        /// <param name="min">The minimum of the starting range.</param>
        /// <param name="max">The maximum of the starting range.</param>
        /// <param name="outMin">The minimum out the resulting range.</param>
        /// <param name="outMax">The maximum out the resulting range.</param>
        /// <returns><c>value</c> mapped from the range (<c>min</c>, <c>max</c>) to
        /// (<c>outMin</c>, <c>outMax</c>). Will return values less than <c>outMin</c>
        /// or greater than <c>outMax</c> if <c>value</c> is not within <c>min</c> and
        /// <c>max</c>.</returns>
        public static float Remap(this int value, int min, int max, float outMin, float outMax)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (((float)value).Normalize(min, max) * (outMax - outMin)) + outMin;
        }
    }
}
