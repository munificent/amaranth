using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public static class Math2
    {
        /// <summary>
        /// Clamps the given value to be within the given range, inclusive.
        /// Conceptually equivalent to <code>Math.Max(min, Math.Min(value, max))</code>.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="value">The value to clamp.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns><c>value</c> clamped to the range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <c>min</c> is greater than <c>max</c>.</exception>
        public static int Clamp(int min, int value, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("The minimum must be less than or equal to the maximum.");

            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Normalizes the given value to range from 0.0 to 1.0 from the given range.
        /// </summary>
        /// <param name="min">If <c>value</c> is <c>min</c> then 0.0 is returned.</param>
        /// <param name="value">The value to map to the given range.</param>
        /// <param name="max">If <c>value</c> is <c>max</c> then 1.0 is returned.</param>
        /// <returns><c>value</c> mapped to the range (min = 0.0, max = 1.0). Will return
        /// values less than 0.0 or greater than 1.0 if <c>value</c> is not within
        /// <c>min</c> and <c>max</c>.</returns>
        public static float Normal(float min, float value, float max)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (value - min) / (max - min);
        }

        /// <summary>
        /// Remaps the given value from within its original range to the corresponding
        /// location in the given second range.
        /// </summary>
        /// <param name="min">The minimum of the starting range.</param>
        /// <param name="value">The value in the range (<c>min</c>, <c>max</c>).</param>
        /// <param name="max">The maximum of the starting range.</param>
        /// <param name="outMin">The minimum out the resulting range.</param>
        /// <param name="outMax">The maximum out the resulting range.</param>
        /// <returns><c>value</c> mapped from the range (<c>min</c>, <c>max</c>) to
        /// (<c>outMin</c>, <c>outMax</c>). Will return values less than <c>outMin</c>
        /// or greater than <c>outMax</c> if <c>value</c> is not within <c>min</c> and
        /// <c>max</c>.</returns>
        public static int Remap(int min, int value, int max, int outMin, int outMax)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (int)(Normal(min, value, max) * (outMax - outMin)) + outMin;
        }

        /// <summary>
        /// Remaps the given value from within its original range to the corresponding
        /// location in the given second range.
        /// </summary>
        /// <param name="min">The minimum of the starting range.</param>
        /// <param name="value">The value in the range (<c>min</c>, <c>max</c>).</param>
        /// <param name="max">The maximum of the starting range.</param>
        /// <param name="outMin">The minimum out the resulting range.</param>
        /// <param name="outMax">The maximum out the resulting range.</param>
        /// <returns><c>value</c> mapped from the range (<c>min</c>, <c>max</c>) to
        /// (<c>outMin</c>, <c>outMax</c>). Will return values less than <c>outMin</c>
        /// or greater than <c>outMax</c> if <c>value</c> is not within <c>min</c> and
        /// <c>max</c>.</returns>
        public static float Remap(float min, float value, float max, float outMin, float outMax)
        {
            if (min == max) throw new ArgumentOutOfRangeException("The min and max cannot be the same value.");

            return (Normal(min, value, max) * (outMax - outMin)) + outMin;
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

        /// <summary>
        /// Returns the minimum of a collection of values.
        /// </summary>
        /// <param name="values">The values to find the minimum of.</param>
        /// <returns>The lowest value in the collection of values.</returns>
        public static int Min(params int[] values)
        {
            return Accumulate((result, value) => Math.Min(result, value), values);
        }

        /// <summary>
        /// Returns the maximum of a collection of values.
        /// </summary>
        /// <param name="values">The values to find the maximum of.</param>
        /// <returns>The highest value in the collection of values.</returns>
        public static int Max(params int[] values)
        {
            return Accumulate((result, value) => Math.Max(result, value), values);
        }

        /// <summary>
        /// Returns the minimum of a collection of values.
        /// </summary>
        /// <param name="values">The values to find the minimum of.</param>
        /// <returns>The lowest value in the collection of values.</returns>
        public static float Min(params float[] values)
        {
            return Accumulate((result, value) => Math.Min(result, value), values);
        }

        /// <summary>
        /// Returns the maximum of a collection of values.
        /// </summary>
        /// <param name="values">The values to find the maximum of.</param>
        /// <returns>The highest value in the collection of values.</returns>
        public static float Max(params float[] values)
        {
            return Accumulate((result, value) => Math.Max(result, value), values);
        }

        private static int Accumulate(Func<int, int, int> accumulator, int[] values)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length == 0) throw new ArgumentException("Must pass at least one value.");

            int result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = accumulator(result, values[i]);
            }

            return result;
        }

        private static float Accumulate(Func<float, float, float> accumulator, float[] values)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length == 0) throw new ArgumentException("Must pass at least one value.");

            float result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = accumulator(result, values[i]);
            }

            return result;
        }
    }
}
