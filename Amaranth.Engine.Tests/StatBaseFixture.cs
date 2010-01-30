using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Engine;

namespace Amaranth.Engine.Tests
{
    public abstract class StatBaseFixture
    {
        [SetUp]
        public void SetUpStatBase()
        {
            mBonusChangedExpectedCount = 0;
            mBonusChangedReceivedCount = 0;
        }

        [Test]
        public void TestGetBonus()
        {
            StatBase stat = CreateStat(15);

            Assert.AreEqual(0, stat.GetBonus(BonusType.Drain));

            stat.SetBonus(BonusType.Drain, 12);
            Assert.AreEqual(12, stat.GetBonus(BonusType.Drain));

            stat.AddBonus(BonusType.Drain, 2);
            Assert.AreEqual(14, stat.GetBonus(BonusType.Drain));
        }

        [Test]
        public void TestHasBonus()
        {
            StatBase stat = CreateStat(15);

            Assert.IsFalse(stat.HasBonus(BonusType.Drain));

            stat.SetBonus(BonusType.Drain, 12);
            Assert.IsTrue(stat.HasBonus(BonusType.Drain));

            stat.AddBonus(BonusType.Drain, -12);
            Assert.IsFalse(stat.HasBonus(BonusType.Drain));
        }

        [Test]
        public void TestBonusChangedEvent()
        {
            StatBase stat = CreateStat(15);

            stat.BonusChanged += Stat_BonusChanged;

            // set base
            stat.Base = 13;
            AssertNoBonusChangedReceived();

            // set no bonus
            stat.SetBonus(BonusType.Haste, 0);
            AssertNoBonusChangedReceived();

            // set a bonus
            stat.SetBonus(BonusType.Haste, 3);
            AssertBonusChangedReceived();

            // set no bonus
            stat.SetBonus(BonusType.Haste, 3);
            AssertNoBonusChangedReceived();

            // add no bonus
            stat.AddBonus(BonusType.Haste, 0);
            AssertNoBonusChangedReceived();

            // add a bonus
            stat.AddBonus(BonusType.Haste, 3);
            AssertBonusChangedReceived();

            // drain
            stat.AddBonus(BonusType.Drain, -3);
            AssertBonusChangedReceived();

            // restore
            stat.Restore();
            AssertBonusChangedReceived();

            // restore when not needed
            stat.Restore();
            AssertNoBonusChangedReceived();

            stat.BonusChanged -= Stat_BonusChanged;

            // set to different value after unregistering
            stat.AddBonus(BonusType.Drain, -3);
            AssertNoBonusChangedReceived();
        }

        protected abstract StatBase CreateStat(int baseValue);

        private void Stat_BonusChanged(object sender, EventArgs e)
        {
            mBonusChangedReceivedCount++;
        }

        private void AssertBonusChangedReceived()
        {
            mBonusChangedExpectedCount++;

            Assert.AreEqual(mBonusChangedExpectedCount, mBonusChangedReceivedCount);
        }

        private void AssertNoBonusChangedReceived()
        {
            Assert.AreEqual(mBonusChangedExpectedCount, mBonusChangedReceivedCount);
        }

        private class MyStat : Stat { }

        private int mBonusChangedReceivedCount;
        private int mBonusChangedExpectedCount;
    }
}
