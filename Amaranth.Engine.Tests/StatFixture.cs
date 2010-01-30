using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Engine;

namespace Amaranth.Engine.Tests
{
    [TestFixture]
    public class StatFixture : StatBaseFixture
    {
        [SetUp]
        public void SetUp()
        {
            mChangedExpectedCount = 0;
            mChangedReceivedCount = 0;
        }

        [Test]
        public void TestCastToInt()
        {
            Stat stat = new Stat(10);

            int cast = stat;

            Assert.AreEqual(10, cast);
        }

        [Test]
        public void TestConstructor()
        {
            Stat stat = new Stat(10);

            Assert.AreEqual(10, stat.Base);
            Assert.AreEqual(10, stat.Current);
        }

        [Test]
        public void TestDefaultConstructor()
        {
            // chooses random stats between 10 and 20 (inclusive)
            for (int i = 0; i < 1000; i++)
            {
                Stat stat = new Stat();

                Assert.LessOrEqual(10, stat.Base);
                Assert.GreaterOrEqual(20, stat.Current);
            }
        }

        [Test]
        public void TestConstructorClampsToBaseMin()
        {
            Stat stat = new Stat(Stat.BaseMin - 2);

            Assert.AreEqual(Stat.BaseMin, stat.Base);
            Assert.AreEqual(Stat.BaseMin, stat.Current);
        }

        [Test]
        public void TestConstructorClampsToBaseMax()
        {
            Stat stat = new Stat(Stat.BaseMax + 2);

            Assert.AreEqual(Stat.BaseMax, stat.Base);
            Assert.AreEqual(Stat.BaseMax, stat.Current);
        }

        [Test]
        public void TestSetBase()
        {
            Stat stat = new Stat(15);

            Assert.AreEqual(15, stat.Base);
            Assert.AreEqual(15, stat.Current);

            // set
            stat.Base = 7;
            Assert.AreEqual(7, stat.Base);
            Assert.AreEqual(7, stat.Current);

            // clamp to base min
            stat.Base = Stat.BaseMin - 4;
            Assert.AreEqual(Stat.BaseMin, stat.Base);
            Assert.AreEqual(Stat.BaseMin, stat.Current);

            // clamp to base max
            stat.Base = Stat.BaseMax + 4;
            Assert.AreEqual(Stat.BaseMax, stat.Base);
            Assert.AreEqual(Stat.BaseMax, stat.Current);
        }

        [Test]
        public void TestName()
        {
            MyStat stat = new MyStat();

            Assert.AreEqual("MyStat", stat.Name);
        }

        [Test]
        public void TestIsLowered()
        {
            Stat stat = new Stat(15);
            Assert.IsFalse(stat.IsLowered);

            stat.SetBonus(BonusType.Haste, 5);
            Assert.IsFalse(stat.IsLowered);

            stat.SetBonus(BonusType.Drain, -5);
            Assert.IsTrue(stat.IsLowered);

            stat.SetBonus(BonusType.Haste, 0);
            Assert.IsTrue(stat.IsLowered);

            stat.SetBonus(BonusType.Drain, 0);
            Assert.IsFalse(stat.IsLowered);
        }

        [Test]
        public void TestIsRaised()
        {
            Stat stat = new Stat(15);
            Assert.IsFalse(stat.IsRaised);

            stat.SetBonus(BonusType.Haste, -5);
            Assert.IsFalse(stat.IsRaised);

            stat.SetBonus(BonusType.Drain, 5);
            Assert.IsTrue(stat.IsRaised);

            stat.SetBonus(BonusType.Haste, 0);
            Assert.IsTrue(stat.IsRaised);

            stat.SetBonus(BonusType.Drain, 0);
            Assert.IsFalse(stat.IsRaised);
        }

        [Test]
        public void TestSetBonus()
        {
            Stat stat = new Stat(15);

            Assert.AreEqual(15, stat.Current);

            stat.SetBonus(BonusType.Equipment, 2);
            Assert.AreEqual(17, stat.Current);

            stat.SetBonus(BonusType.Equipment, 0);
            Assert.AreEqual(15, stat.Current);

            stat.SetBonus(BonusType.Equipment, -4);
            Assert.AreEqual(11, stat.Current);

            stat.SetBonus(BonusType.Haste, 2);
            Assert.AreEqual(13, stat.Current);

            stat.SetBonus(BonusType.Equipment, -1000);
            Assert.AreEqual(Stat.TotalMin, stat.Current);

            stat.SetBonus(BonusType.Equipment, 1000);
            Assert.AreEqual(Stat.TotalMax, stat.Current);
        }

        [Test]
        public void TestAddBonus()
        {
            Stat stat = new Stat(15);

            Assert.AreEqual(15, stat.Current);

            stat.AddBonus(BonusType.Drain, 2);
            Assert.AreEqual(17, stat.Current);

            stat.AddBonus(BonusType.Drain, 0);
            Assert.AreEqual(17, stat.Current);

            stat.AddBonus(BonusType.Drain, -4);
            Assert.AreEqual(13, stat.Current);

            stat.AddBonus(BonusType.Drain, -1000);
            Assert.AreEqual(Stat.TotalMin, stat.Current);

            stat.AddBonus(BonusType.Equipment, 4000);
            Assert.AreEqual(Stat.TotalMax, stat.Current);
        }

        [Test]
        public void TestRestore()
        {
            Stat stat = new Stat(15);

            Assert.AreEqual(15, stat.Current);

            stat.AddBonus(BonusType.Drain, -2);
            Assert.AreEqual(13, stat.Current);

            bool restored = stat.Restore();
            Assert.IsTrue(restored);
            Assert.AreEqual(15, stat.Current);

            restored = stat.Restore();
            Assert.IsFalse(restored);
        }

        [Test]
        public void TestChangedEvent()
        {
            Stat stat = new Stat(15);

            stat.Changed += Stat_Changed;

            // set to same value
            stat.Base = 15;
            AssertNoChangedReceived();

            // set to different value
            stat.Base = 13;
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
            return new Stat(baseValue);
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

        private class MyStat : Stat { }

        private int mChangedReceivedCount;
        private int mChangedExpectedCount;
    }
}
