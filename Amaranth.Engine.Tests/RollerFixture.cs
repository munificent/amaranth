using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Engine;

namespace Amaranth.Engine.Tests
{
    [TestFixture]
    public class RollerFixture
    {
        #region Parse

        [Test]
        public void TestParseFixed()
        {
            for (int outOf = 2; outOf < 10; outOf++)
            {
                for (int chance = 1; chance < outOf; chance++)
                {
                    float ave = Roller.Taper(chance, outOf).Average;
                    Console.WriteLine("taper " + chance + "/" + outOf + " = " + ave);
                }
            }

            TestParse("3", "3", 3, new float[] { 0, 0, 0, 1.0f, 0 });
            TestParse(" 2", "2", 2, new float[] { 0, 0, 1.0f, 0 });
            TestParse(" 1  ", "1", 1, new float[] { 0, 1.0f, 0 });
        }

        [Test]
        public void TestParseRange()
        {
            TestParse("2-6", "2-6", 4, new float[] { 0, 0, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0 });
            TestParse(" 1-2", "1-2", 1.5f, new float[] { 0, 0.5f, 0.5f, 0 });
            TestParse(" 3-3  ", "3-3", 3, new float[] { 0, 0, 0, 1.0f, 0 });
        }

        [Test]
        public void TestParseDice()
        {
            TestParse(" 1d5", "1d5", 3, new float[] { 0, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0 });

            TestParse(" 2d3  ", "2d3", 4, new float[] {
                0,
                0,
                1 / 9.0f,
                2 / 9.0f,
                3 / 9.0f,
                2 / 9.0f,
                1 / 9.0f,
                0 });
        }

        [Test]
        public void TestParseTriangle()
        {
            TestParse(" 2t1  ", "2t1", 2, new float[] { 0, 0.25f, 0.5f, 0.25f, 0 });

            TestParse("2t4", "2t4", 2, new float[] { 
                    3 / 25.0f,
                    4 / 25.0f,
                    5 / 25.0f,
                    4 / 25.0f,
                    3 / 25.0f,
                    2 / 25.0f,
                    1 / 25.0f });

            TestParse(" 3t0", "3t0", 3, new float[] { 0, 0, 0, 1.0f, 0, 0 });
        }

        [Test]
        public void TestParseFixedTaper()
        {
            // geometric series sums
            float taperAverageFour = 1.0f / 3.0f; // 1 in 4
            float taperAverageTwo = 1.0f; // 1 in 2

            TestParse("3(1:4)", "3(1:4)", 3 + taperAverageFour);
            TestParse(" 2(1:2)", "2(1:2)", 2 + taperAverageTwo);
            TestParse("1(3:4)", "1(3:4)", 1 + (3.0f / (4.0f - 3.0f)));
        }

        [Test]
        public void TestParseRangeTaper()
        {
            // geometric series sums
            float taperAverageFour = 1.0f / 3.0f; // 1 in 4
            float taperAverageTwo = 1.0f; // 1 in 2

            TestParse("2-6(1:4)", "2-6(1:4)", 4 + taperAverageFour);
            TestParse(" 1-2(1:2)", "1-2(1:2)", 1.5f + taperAverageTwo);
        }

        [Test]
        public void TestParseDiceTaper()
        {
            // geometric series sums
            float taperAverageFour = 1.0f / 3.0f; // 1 in 4
            float taperAverageTwo = 1.0f; // 1 in 2

            TestParse("1d5(1:4)", "1d5(1:4)", 3 + taperAverageFour);
            TestParse(" 2d3(1:2)", "2d3(1:2)", 4 + taperAverageTwo);
        }

        [Test]
        public void TestParseTriangleTaper()
        {
            // geometric series sums
            float taperAverageFour = 1.0f / 3.0f; // 1 in 4
            float taperAverageTwo = 1.0f; // 1 in 2

            TestParse("2t1(1:4)", "2t1(1:4)", 2 + taperAverageFour);
            TestParse(" 2t4(1:2)", "2t4(1:2)", 2 + taperAverageTwo);
        }

        #endregion

        #region Helper methods

        private void TestParse(string text, string expected, float average, float[] frequencies)
        {
            Roller roller = Roller.Parse(text);

            Assert.AreEqual(expected, roller.ToString());
            Assert.AreEqual(average, roller.Average);

            if (frequencies != null)
            {
                Statistics.TestFrequencies(frequencies, () => roller.Roll());
            }
        }

        private void TestParse(string text, string expected, float average)
        {
            TestParse(text, expected, average, null);
        }

        #endregion
    }
}
