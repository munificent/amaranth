using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Engine;

namespace Amaranth.Engine.Tests
{
    [TestFixture]
    public class FluidStatFixture : StatBaseFixture
    {
        [SetUp]
        public void SetUp()
        {
            mChangedExpectedCount = 0;
            mChangedReceivedCount = 0;
        }

        [Test]
        public void TestConstructor()
        {
            FluidStat stat = new FluidStat(10);

            Assert.AreEqual(0, stat.Min);
            Assert.AreEqual(10, stat.Base);
            Assert.AreEqual(10, stat.Max);
            Assert.AreEqual(10, stat.Current);
        }

        [Test]
        public void TestSetBase()
        {
            FluidStat stat = new FluidStat(10);

            Assert.AreEqual(10, stat.Base);
            Assert.AreEqual(10, stat.Max);
            Assert.AreEqual(10, stat.Current);

            // lower
            stat.Base -= 2;
            Assert.AreEqual(8, stat.Base);
            Assert.AreEqual(8, stat.Max);
            Assert.AreEqual(8, stat.Current);

            // raise
            stat.Base = 20;
            Assert.AreEqual(20, stat.Base);
            Assert.AreEqual(20, stat.Max);
            Assert.AreEqual(8, stat.Current);

            // clamp to min
            stat.Base = stat.Min - 10;
            Assert.AreEqual(stat.Min, stat.Base);
            Assert.AreEqual(stat.Min, stat.Max);
            Assert.AreEqual(stat.Min, stat.Current);
        }

        [Test]
        public void TestSetCurrent()
        {
            FluidStat stat = new FluidStat(10);

            Assert.AreEqual(10, stat.Current);

            // lower
            stat.Current -= 2;
            Assert.AreEqual(8, stat.Current);

            // clamp to max
            stat.Current = stat.Base + 20;
            Assert.AreEqual(stat.Base, stat.Current);

            // clamp to min
            stat.Current = stat.Min - 10;
            Assert.AreEqual(stat.Min, stat.Current);
        }

        [Test]
        public void TestIsMax()
        {
            FluidStat stat = new FluidStat(10);

            Assert.IsTrue(stat.IsMax);

            // lower
            stat.Current -= 2;
            Assert.IsFalse(stat.IsMax);

            // restore
            stat.Current = stat.Max;
            Assert.IsTrue(stat.IsMax);

            // add a bonus
            stat.AddBonus(BonusType.Haste, 5);
            Assert.IsFalse(stat.IsMax);

            // restore
            stat.Current = stat.Max;
            Assert.IsTrue(stat.IsMax);
        }

        [Test]
        public void TestSetBonus()
        {
            FluidStat stat = new FluidStat(15);

            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(15, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.SetBonus(BonusType.Equipment, 2);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(17, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.SetBonus(BonusType.Equipment, 0);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(15, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.SetBonus(BonusType.Equipment, -4);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(11, stat.Max);
            Assert.AreEqual(11, stat.Current);

            stat.SetBonus(BonusType.Haste, 2);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(13, stat.Max);
            Assert.AreEqual(11, stat.Current);
        }

        [Test]
        public void TestAddBonus()
        {
            FluidStat stat = new FluidStat(15);

            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(15, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.AddBonus(BonusType.Drain, 2);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(17, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.AddBonus(BonusType.Drain, 0);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(17, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.AddBonus(BonusType.Drain, -4);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(13, stat.Max);
            Assert.AreEqual(13, stat.Current);
        }

        [Test]
        public void TestRestore()
        {
            FluidStat stat = new FluidStat(15);

            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(15, stat.Max);
            Assert.AreEqual(15, stat.Current);

            stat.AddBonus(BonusType.Drain, -2);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(13, stat.Max);
            Assert.AreEqual(13, stat.Current);

            bool restored = stat.Restore();
            Assert.IsTrue(restored);
            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(15, stat.Max);
            Assert.AreEqual(13, stat.Current);

            restored = stat.Restore();
            Assert.IsFalse(restored);
        }

        [Test]
        public void TestChangedEvent()
        {
            FluidStat stat = new FluidStat(15);

            stat.Changed += Stat_Changed;

            // set to same base
            stat.Base = 15;
            AssertNoChangedReceived();

            // set to different base
            stat.Base = 13;
            AssertChangedReceived();

            // set to same current
            stat.Current = 13;
            AssertNoChangedReceived();

            // set to same current (clamped)
            stat.Current = 25;
            AssertNoChangedReceived();

            // set to different current
            stat.Current = 8;
            AssertChangedReceived();

            // set no bonus
            stat.SetBonus(BonusType.Haste, 0);
            AssertNoChangedReceived();

            // set a bonus
            stat.SetBonus(BonusType.Haste, 3);
            AssertChangedReceived();

            // set no bonus
            stat.SetBonus(BonusType.Haste, 3);
            AssertNoChangedReceived();

            // add no bonus
            stat.AddBonus(BonusType.Haste, 0);
            AssertNoChangedReceived();

            // add a bonus
            stat.AddBonus(BonusType.Haste, 3);
            AssertChangedReceived();

            // drain
            stat.AddBonus(BonusType.Drain, -3);
            AssertChangedReceived();

            // restore
            stat.Restore();
            AssertChangedReceived();

            // restore when not needed
            stat.Restore();
            AssertNoChangedReceived();

            stat.Changed -= Stat_Changed;

            // set to different value after unregistering
            stat.Base = 17;
            AssertNoChangedReceived();
        }

        protected override StatBase CreateStat(int baseValue)
        {
            return new FluidStat(baseValue);
        }

        private void Stat_Changed(object sender, EventArgs e)
        {
            mChangedReceivedCount++;
        }

        private void AssertChangedReceived()
        {
            mChangedExpectedCount++;

            Assert.AreEqual(mChangedExpectedCount, mChangedReceivedCount);
        }

        private void AssertNoChangedReceived()
        {
            Assert.AreEqual(mChangedExpectedCount, mChangedReceivedCount);
        }

        private int mChangedReceivedCount;
        private int mChangedExpectedCount;
    }
}
