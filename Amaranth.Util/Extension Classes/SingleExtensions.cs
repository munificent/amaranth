using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// Extension methods on <c>float</c>.
    /// </summary>
    public static class SingleExtensions
    {
        /// <summary>
        /// Normalizes the value to range from 0.0 to 1.0 from the given range.
        /// </summary>
        /// <param name="value">The value to map to the given range.</param>
        /// <param name="min">If <c>value</c> is <c>min</c> then 0.0 is returned.</param>
        /// <param name="max">If <c>value</c> is <c>max</c> then 1.0 is returned.</param>
        /// <returns><c>value</c> mapped to the range (min = 0.0, max = 1.0). Will return
        /// values less than 0.0 or greater than 1.0 if <c>value</c> is not within
        /// <c>min</c> and <c>max</c>.</returns>
        public static float Normalize(this float value, float min, float max)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (value - min) / (max - min);
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
        public static float Remap(this float value, float min, float max, float outMin, float outMax)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (value.Normalize(min, max) * (outMax - outMin)) + outMin;
        }

        /// <summary>
        /// Swaps the two values in place.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        public static void Swap<T>(ref T left, ref T right)
        {
            T temp = left;
            left = right;
            right = temp;
        }
    }
}
