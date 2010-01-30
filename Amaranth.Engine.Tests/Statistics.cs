using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Amaranth.Engine.Tests
{
    public static class Statistics
    {

        /// <summary>
        /// Tests that after repeated runs, the given predicate returns true
        /// with the expected normal frequency (i.e. true 50% of the time 
        /// means an expected of 0.5).
        /// </summary>
        /// <param name="expected">Expected frequency of true results, out of 1.0.</param>
        /// <param name="predicate">Predicate function to test.</param>
        public static void TestFrequency(float expected, Func<bool> predicate)
        {
            TestFrequencies(new float[] { 1.0f - expected, expected },
                () => predicate() ? 1 : 0);
        }

        /// <summary>
        /// Tests that after repeated runs, the given function returns values
        /// with the expected range of frequencies for each value.
        /// </summary>
        /// <param name="expected">Expected frequencies of results, out of 1.0. The index
        /// in the array corresponds to the value returned from <c>func</c>.</param>
        /// <param name="func">Function to test.</param>
        public static void TestFrequencies(float[] expected, Func<int> func)
        {
            int runs = 100000 * expected.Length;

            int[] counts = new int[expected.Length];

            // accumulate the results
            for (int i = 0; i < runs; i++)
            {
                int result = func();

                if ((result >= 0) && (result < counts.Length))
                {
                    counts[result]++;
                }
            }

            // show the results
            for (int i = 0; i < counts.Length; i++)
            {
                float normal = (float)counts[i] / (float)runs;
                float distance = Math.Abs(expected[i] - normal);
                Console.WriteLine("  " + i.ToString() + " : " + counts[i] + " / " + runs + " = " + normal + " (expected " + expected[i] + ")");
            }

            float epsilon = 0.01f;

            // test the results
            for (int i = 0; i < counts.Length; i++)
            {
                float normal = (float)counts[i] / (float)runs;
                float distance = Math.Abs(expected[i] - normal);
                Assert.Less(distance, epsilon);
            }
        }
    }
}
