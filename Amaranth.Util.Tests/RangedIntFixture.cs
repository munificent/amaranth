using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class RangedIntFixture
    {
        [Test]
        public void TestImplicitCast()
        {
            RangedInt ranged = new RangedInt(5, 0, 10);

            int cast = ranged;

            Assert.AreEqual(5, cast);
        }

        #region Events

        [Test]
        public void TestChangedEvent()
        {
            RangedInt ranged = new RangedInt(5, 0, 10);

            // count the number of times the event is received
            int received = 0;
            EventHandler handler = (sender, args) => received++;

            // listen to it
            ranged.Changed += handler;

            // changing any property should raise the event
            ranged.Current++;
            Assert.AreEqual(1, received);

            ranged.Max++;
            Assert.AreEqual(2, received);

            ranged.Min++;
            Assert.AreEqual(3, received);

            ranged.Changed -= handler;

            // should not be raised after it's unregistered
            ranged.Current++;
            Assert.AreEqual(3, received);
        }

        [Test]
        public void TestCurrentChangedEvent()
        {
            RangedInt ranged = new RangedInt(5, 0, 10);

            // count the number of times the event is received
            int expectedOld = 0;
            int expectedNew = 0;
            int received = 0;

            EventHandler<ValueChangeEventArgs<int>> handler =
                (sender, args) =>
                {
                    Assert.AreEqual(expectedOld, args.Old);
                    Assert.AreEqual(expectedNew, args.New);
                    received++;
                };

            // listen to it
            ranged.CurrentChanged += handler;

            expectedOld = 5;
            expectedNew = 7;
            ranged.Current = 7;
            Assert.AreEqual(1, received);

            // test the clamping
            expectedOld = 7;
            expectedNew = 10;
            ranged.Current = 25;

            Assert.AreEqual(2, received);

            // moving the max should push the current
            expectedOld = 10;
            expectedNew = 7;
            ranged.Max = 7;

            Assert.AreEqual(3, received);

            // moving the min should push the current
            expectedOld = 7;
            expectedNew = 15;
            ranged.Min = 15;

            Assert.AreEqual(4, received);

            ranged.CurrentChanged -= handler;

            // should not be raised after it's unregistered
            ranged.Current++;
            Assert.AreEqual(4, received);
        }

        [Test]
        public void TestMinChangedEvent()
        {
            RangedInt ranged = new RangedInt(5, 0, 10);

            // count the number of times the event is received
            int expectedOld = 0;
            int expectedNew = 0;
            int received = 0;

            EventHandler<ValueChangeEventArgs<int>> handler =
                (sender, args) =>
                {
                    Assert.AreEqual(expectedOld, args.Old);
                    Assert.AreEqual(expectedNew, args.New);
                    received++;
                };

            // listen to it
            ranged.MinChanged += handler;

            expectedOld = 0;
            expectedNew = 2;
            ranged.Min = 2;
            Assert.AreEqual(1, received);

            ranged.MinChanged -= handler;

            // should not be raised after it's unregistered
            ranged.Min++;
            Assert.AreEqual(1, received);
        }

        [Test]
        public void TestMaxChangedEvent()
        {
            RangedInt ranged = new RangedInt(5, 0, 10);

            // count the number of times the event is received
            int expectedOld = 0;
            int expectedNew = 0;
            int received = 0;

            EventHandler<ValueChangeEventArgs<int>> handler =
                (sender, args) =>
                {
                    Assert.AreEqual(expectedOld, args.Old);
                    Assert.AreEqual(expectedNew, args.New);
                    received++;
                };

            // listen to it
            ranged.MaxChanged += handler;

            expectedOld = 10;
            expectedNew = 7;
            ranged.Max = 7;
            Assert.AreEqual(1, received);

            ranged.MaxChanged -= handler;

            // should not be raised after it's unregistered
            ranged.Max++;
            Assert.AreEqual(1, received);
        }

        #endregion

        #region Properties

        [Test]
        public void TestCurrent()
        {
            RangedInt ranged = new RangedInt(5, 0, 10);

            Assert.AreEqual(5, ranged.Current);

            ranged.Current = 7;
            Assert.AreEqual(7, ranged.Current);

            // clamp to min
            ranged.Current = -10;
            Assert.AreEqual(0, ranged.Current);

            // clamp to max
            ranged.Current = 30;
            Assert.AreEqual(10, ranged.Current);

            // clamp if max changes
            ranged.Max = 30;
            Assert.AreEqual(10, ranged.Current);

            ranged.Max = 8;
            Assert.AreEqual(8, ranged.Current);

            // clamp if min changes
            ranged.Min = 20;
            Assert.AreEqual(20, ranged.Current);
        }

        #endregion
    }
}
