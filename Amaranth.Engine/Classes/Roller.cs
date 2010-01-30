using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Amaranth.Engine
{
    /// <summary>
    /// A roller rolls random numbers with certain parameters. It encapsulates a call
    /// to one of the Rng functions, and the parameters passed in.
    /// </summary>
    [Serializable]
    public class Roller
    {
        /// <summary>
        /// Tries to create a new Roller by parsing the given string representation of it.
        /// Valid strings include "3", "2-5", "2d6", "6t3", and "2-5(1:4)".
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <returns>The parsed Roller or <c>null</c> if unsuccessful.</returns>
        public static Roller Parse(string text)
        {
            // ignore whitespace
            text = text.Trim();

            // compile the regex if needed
            if (sParser == null)
            {
                string pattern = @"^((?<die>(?<dice>\d+)d(?<sides>\d+))|(?<tri>(?<center>\d+)t(?<range>\d+))|(?<range>(?<min>\d+)-(?<max>\d+))|(?<fixed>(?<value>-?\d+)))(?<taper>\((?<chance>\d+)\:(?<outof>\d+)\))?$";

                sParser = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            }

            // parse it
            Match match = sParser.Match(text);

            if (!match.Success) return null;

            Roller roller;

            if (match.Groups["die"].Success)
            {
                int dice = Int32.Parse(match.Groups["dice"].Value);
                int sides = Int32.Parse(match.Groups["sides"].Value);
                roller = Roller.Dice(dice, sides);
            }
            else if (match.Groups["tri"].Success)
            {
                int center = Int32.Parse(match.Groups["center"].Value);
                int range = Int32.Parse(match.Groups["range"].Value);
                roller = Roller.Triangle(center, range);
            }
            else if (match.Groups["range"].Success)
            {
                int min = Int32.Parse(match.Groups["min"].Value);
                int max = Int32.Parse(match.Groups["max"].Value);
                roller = Roller.Range(min, max);
            }
            else // fixed
            {
                int value = Int32.Parse(match.Groups["value"].Value);
                roller = Roller.Fixed(value);
            }

            // add the taper
            if (match.Groups["taper"].Success)
            {
                int chance = Int32.Parse(match.Groups["chance"].Value);
                int outOf = Int32.Parse(match.Groups["outof"].Value);

                roller.mNextRoller = Taper(chance, outOf);
            }

            return roller;
        }

        public static Roller Fixed(int value)
        {
            return new Roller(
                () => value,
                value, value.ToString(), true);
        }

        public static Roller Range(int min, int max)
        {
            return new Roller(
                () => Rng.IntInclusive(min, max),
                (min + max) / 2.0f, min.ToString() + "-" + max.ToString());
        }

        public static Roller Dice(int dice, int sides)
        {
            return new Roller(
                () => Rng.Roll(dice, sides),
                dice * ((1 + sides) / 2.0f), dice.ToString() + "d" + sides.ToString());
        }

        public static Roller Triangle(int center, int range)
        {
            return new Roller(
                () => Rng.TriangleInt(center, range),
                center, center.ToString() + "t" + range.ToString());
        }

        public static Roller Taper(int chance, int outOf)
        {
            return new Roller(
                () => Rng.Taper(0, 1, chance, outOf),
                (float)chance / (outOf - (float)chance), // sum of geometric series
                "(" + chance + ":" + outOf + ")");
        }

        /// <summary>
        /// Gets whether this Roller always returns a fixed value.
        /// </summary>
        public bool IsFixed
        {
            get
            {
                bool isFixed = mIsFixed;
                if (mNextRoller != null) isFixed &= mNextRoller.IsFixed;

                return isFixed;
            }
        }

        /// <summary>
        /// Gets the average of the values rolled by this Roller.
        /// </summary>
        public float Average
        {
            get
            {
                float average = mAverage;
                if (mNextRoller != null) average += mNextRoller.Average;

                return average;
            }
        }

        public override string ToString()
        {
            string text = mText;
            if (mNextRoller != null) text += mNextRoller.ToString();

            return text;
        }

        public int Roll()
        {
            int result = mRollFunction();
            if (mNextRoller != null) result += mNextRoller.Roll();

            return result;
        }

        private Roller(Func<int> rollFunction, float average, string text, bool isFixed)
        {
            mRollFunction = rollFunction;
            mAverage = average;
            mText = text;
            mIsFixed = isFixed;
        }

        private Roller(Func<int> rollFunction, float average, string text)
            : this(rollFunction, average, text, false)
        {
        }

        private static Regex sParser;

        private Func<int> mRollFunction;
        private float mAverage;
        private string mText;
        private bool mIsFixed;

        // allows rollers to be chained: 2d6 + 3d4 + 1t4...
        private Roller mNextRoller;
    }
}
