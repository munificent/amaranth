using System;
using System.Collections.Generic;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// The Random Number God.
    /// </summary>
    public static class Rng
    {
        /// <summary>
        /// Resets the seed used to generate the random numbers to a time-dependent one.
        /// </summary>
        public static void Seed()
        {
            sRandom = new Random();
        }

        /// <summary>
        /// Sets the seed used to generate the random numbers.
        /// </summary>
        /// <param name="seed">New seed.</param>
        public static void Seed(int seed)
        {
            sRandom = new Random(seed);
        }

        /// <summary>
        /// Gets a random int between 0 and max (half-inclusive).
        /// </summary>
        public static int Int(int max)
        {
            return sRandom.Next(max);
        }

        /// <summary>
        /// Gets a random int between min and max (half-inclusive).
        /// </summary>
        public static int Int(int min, int max)
        {
            return Int(max - min) + min;
        }

        /// <summary>
        /// Gets a random int between 0 and max (inclusive).
        /// </summary>
        public static int IntInclusive(int max)
        {
            return sRandom.Next(max + 1);
        }

        /// <summary>
        /// Gets a random int between min and max (inclusive).
        /// </summary>
        public static int IntInclusive(int min, int max)
        {
            return IntInclusive(max - min) + min;
        }

        /// <summary>
        /// Gets a random float between 0 and max.
        /// </summary>
        public static float Float(float max)
        {
            if (max < 0.0f) throw new ArgumentOutOfRangeException("The max must be zero or greater.");

            return (float)sRandom.NextDouble() * max;
        }

        /// <summary>
        /// Gets a random float between min and max.
        /// </summary>
        public static float Float(float min, float max)
        {
            if (max < min) throw new ArgumentOutOfRangeException("The max must be min or greater.");

            return Float(max - min) + min;
        }

        /// <summary>
        /// Gets a random Vec within the given size (half-inclusive).
        /// </summary>
        public static Vec Vec(Vec size)
        {
            return new Vec(Int(size.X), Int(size.Y));
        }

        /// <summary>
        /// Gets a random Vec within the given Rect (half-inclusive).
        /// </summary>
        public static Vec Vec(Rect rect)
        {
            return new Vec(Int(rect.Left, rect.Right), Int(rect.Top, rect.Bottom));
        }

        /// <summary>
        /// Gets a random Vec within the given Rect (inclusive).
        /// </summary>
        public static Vec VecInclusive(Rect rect)
        {
            return new Vec(IntInclusive(rect.Left, rect.Right), IntInclusive(rect.Top, rect.Bottom));
        }

        /// <summary>
        /// Gets a random item from the given list.
        /// </summary>
        public static T Item<T>(IList<T> items)
        {
            return items[Int(items.Count)];
        }

        /// <summary>
        /// Returns true if a random int chosen between 1 and chance was 1.
        /// </summary>
        public static bool OneIn(int chance)
        {
            if (chance <= 0) throw new ArgumentOutOfRangeException("The chance must be one or greater.");

            return Int(chance) == 0;
        }

        /// <summary>
        /// Rolls the given number of dice with the given number of sides on each die,
        /// and returns the sum. The values on each side range from 1 to the number of
        /// sides.
        /// </summary>
        /// <param name="dice">Number of dice to roll.</param>
        /// <param name="sides">Number of sides on each dice.</param>
        /// <returns>The sum of the dice rolled.</returns>
        public static int Roll(int dice, int sides)
        {
            if (dice <= 0) throw new ArgumentOutOfRangeException("dice", "The argument \"dice\" must be greater than zero.");
            if (sides <= 0) throw new ArgumentOutOfRangeException("sides", "The argument \"sides\" must be greater than zero.");

            int total = 0;

            for (int i = 0; i < dice; i++)
            {
                total += Int(1, sides + 1);
            }

            return total;
        }

        /// <summary>
        /// Gets a random number centered around "center" with range "range" (inclusive)
        /// using a triangular distribution. For example getTriInt(8, 4) will return values
        /// between 4 and 12 (inclusive) with greater distribution towards 8.
        /// </summary>
        /// <remarks>
        /// This means output values will range from (center - range) to (center + range)
        /// inclusive, with most values near the center, but not with a normal distribution.
        /// Think of it as a poor man's bell curve.
        ///
        /// The algorithm works essentially by choosing a random point inside the triangle,
        /// and then calculating the x coordinate of that point. It works like this:
        ///
        /// Consider Center 4, Range 3:
        /// 
        ///         *
        ///       * | *
        ///     * | | | *
        ///   * | | | | | *
        /// --+-----+-----+--
        /// 0 1 2 3 4 5 6 7 8
        ///  -r     c     r
        /// 
        /// Now flip the left half of the triangle (from 1 to 3) vertically and move it
        /// over to the right so that we have a square.
        /// 
        ///     +-------+
        ///     |       V
        ///     |
        ///     |   R L L L
        ///     | . R R L L
        ///     . . R R R L
        ///   . . . R R R R
        /// --+-----+-----+--
        /// 0 1 2 3 4 5 6 7 8
        /// 
        /// Choose a point in that square. Figure out which half of the triangle the
        /// point is in, and then remap the point back out to the original triangle.
        /// The result is the x coordinate of the point in the original triangle.
        /// </remarks>
        public static int TriangleInt(int center, int range)
        {
            if (range < 0) throw new ArgumentOutOfRangeException("range", "The argument \"range\" must be zero or greater.");

            // pick a point in the square
            int x = IntInclusive(range);
            int y = IntInclusive(range);

            // figure out which triangle we are in
            if (x <= y)
            {
                // larger triangle
                return center + x;
            }
            else
            {
                // smaller triangle
                return center - range - 1 + x;
            }
        }

        /// <summary>
        /// Randomly walks the given starting value repeatedly up and/or down
        /// with the given probabilities. Will only walk in one direction.
        /// </summary>
        /// <param name="start">Value to start at.</param>
        public static int Walk(int start, int chanceOfDec, int chanceOfInc)
        {
            // make sure we won't get stuck in an infinite loop
            if (chanceOfDec == 1) throw new ArgumentOutOfRangeException("chanceOfDec must be zero or greater than one.");
            if (chanceOfInc == 1) throw new ArgumentOutOfRangeException("chanceOfInc must be zero greater than one.");

            // decide if walking up or down
            int direction = Int(chanceOfDec + chanceOfInc);
            if (direction < chanceOfDec)
            {
                // exponential chance of decrementing
                int sanity = 10000;
                while (Rng.OneIn(chanceOfDec) && (sanity-- > 0))
                {
                    start--;
                }
            }
            else if (direction < chanceOfDec + chanceOfInc)
            {
                // exponential chance of incrementing
                int sanity = 10000;
                while (Rng.OneIn(chanceOfInc) && (sanity-- > 0))
                {
                    start++;
                }
            }

            return start;
        }

        /// <summary>
        /// Randomly walks the given level using a... unique distribution. The
        /// goal is to return a value that approximates a bell curve centered
        /// on the start level whose wideness increases as the level increases.
        /// Thus, starting at a low start level will only walk a short distance,
        /// while starting at a higher level can wander a lot farther.
        /// </summary>
        /// <returns></returns>
        public static int WalkLevel(int level)
        {
            int result = level;

            // stack a few triangles to approximate a bell
            for (int i = 0; i < Math.Min(5, level); i++)
            {
                // the width of the triangle is based on the level
                result += Rng.TriangleInt(0, 1 + (level / 20));
            }

            // also have an exponentially descreasing change of going out of depth
            while (Rng.OneIn(10))
            {
                result += 1 + Rng.Int(2 + (level / 5));
            }

            return result;
        }

        /// <summary>
        /// Repeatedly adds an increment to a given starting value as long as random
        /// numbers continue to be chosen from within a given range. Yields numbers
        /// whose probability gradually tapers off from the starting value.
        /// </summary>
        /// <param name="start">Starting value.</param>
        /// <param name="increment">Amount to modify starting value every successful
        /// iteration.</param>
        /// <param name="chance">The odds of an iteration being successful.</param>
        /// <param name="outOf">The range to choose from to see if the iteration
        /// is successful.</param>
        /// <returns>The resulting value.</returns>
        public static int Taper(int start, int increment, int chance, int outOf)
        {
            if (increment == 0) throw new ArgumentOutOfRangeException("increment", "The increment cannot be zero.");
            if (chance <= 0) throw new ArgumentOutOfRangeException("chance", "The chance must be greater than zero.");
            if (chance >= outOf) throw new ArgumentOutOfRangeException("chance", "The chance must be less than the range.");
            if (outOf <= 0) throw new ArgumentOutOfRangeException("outOf", "The range must be positive.");

            int value = start;

            while (Rng.Int(outOf) < chance)
            {
                value += increment;
            }

            return value;
        }

        private static Random sRandom = new Random();
    }
}
