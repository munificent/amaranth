using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class VecFixture
    {
        #region Public static properties

        [Test]
        public void TestZero()
        {
            Vec v = Vec.Zero;

            Assert.AreEqual(0, v.X);
            Assert.AreEqual(0, v.Y);
        }

        [Test]
        public void TestOne()
        {
            Vec v = Vec.One;

            Assert.AreEqual(1, v.X);
            Assert.AreEqual(1, v.Y);
        }

        #endregion

        #region Operators

        [Test]
        public void TestOperatorEquals()
        {
            Assert.AreEqual(true, new Vec(1, 2) == new Vec(1, 2));
            Assert.AreEqual(true, new Vec(1, 1) == Vec.One);
            Assert.AreEqual(false, new Vec(1, 1) == Vec.Zero);
            Assert.AreEqual(false, new Vec(3, 4) == new Vec(4, 5));

            Assert.AreEqual(false, new Vec(3, 4) == null);
            Assert.AreEqual(false, null == new Vec(3, 4));
        }

        [Test]
        public void TestOperatorNotEquals()
        {
            Assert.AreEqual(false, new Vec(1, 2) != new Vec(1, 2));
            Assert.AreEqual(false, new Vec(1, 1) != Vec.One);
            Assert.AreEqual(true, new Vec(1, 1) != Vec.Zero);
            Assert.AreEqual(true, new Vec(3, 4) != new Vec(4, 5));

            Assert.AreEqual(true, new Vec(3, 4) != null);
            Assert.AreEqual(true, null != new Vec(3, 4));
        }

        [Test]
        public void TestOperatorVecPlusVec()
        {
            Assert.AreEqual(new Vec(3, 4), new Vec(1, 2) + new Vec(2, 2));
            Assert.AreEqual(new Vec(0, 0), new Vec(1, 2) + new Vec(-1, -2));
            Assert.AreEqual(new Vec(4, 5), new Vec(4, 0) + new Vec(0, 5));
        }

        [Test]
        public void TestOperatorVecPlusInt()
        {
            Assert.AreEqual(new Vec(3, 4), new Vec(1, 2) + 2);
            Assert.AreEqual(new Vec(0, 1), new Vec(1, 2) + -1);
            Assert.AreEqual(new Vec(4, 5), new Vec(4, 5) + 0);
        }

        [Test]
        public void TestOperatorIntPlusVec()
        {
            Assert.AreEqual(new Vec(3, 4), 2 + new Vec(1, 2));
            Assert.AreEqual(new Vec(0, 1), -1 + new Vec(1, 2));
            Assert.AreEqual(new Vec(4, 5), 0 + new Vec(4, 5));
        }

        [Test]
        public void TestOperatorVecMinusVec()
        {
            Assert.AreEqual(new Vec(-1, 0), new Vec(1, 2) - new Vec(2, 2));
            Assert.AreEqual(new Vec(2, 4), new Vec(1, 2) - new Vec(-1, -2));
            Assert.AreEqual(new Vec(4, -5), new Vec(4, 0) - new Vec(0, 5));
        }

        [Test]
        public void TestOperatorVecMinusInt()
        {
            Assert.AreEqual(new Vec(-1, 0), new Vec(1, 2) - 2);
            Assert.AreEqual(new Vec(2, 3), new Vec(1, 2) - -1);
            Assert.AreEqual(new Vec(4, 5), new Vec(4, 5) - 0);
        }

        [Test]
        public void TestOperatorIntMinusVec()
        {
            Assert.AreEqual(new Vec(1, 0), 2 - new Vec(1, 2));
            Assert.AreEqual(new Vec(-2, -3), -1 - new Vec(1, 2));
            Assert.AreEqual(new Vec(-4, -5), 0 - new Vec(4, 5));
        }

        [Test]
        public void TestOperatorVecTimesInt()
        {
            Assert.AreEqual(new Vec(2, 4), new Vec(1, 2) * 2);
            Assert.AreEqual(new Vec(-1, -2), new Vec(1, 2) * -1);
            Assert.AreEqual(new Vec(0, 0), new Vec(4, 5) * 0);
        }

        [Test]
        public void TestOperatorIntTimesVec()
        {
            Assert.AreEqual(new Vec(2, 4), 2 * new Vec(1, 2));
            Assert.AreEqual(new Vec(-1, -2), -1 * new Vec(1, 2));
            Assert.AreEqual(new Vec(0, 0), 0 * new Vec(4, 5));
        }

        [Test]
        public void TestOperatorVecDividedByInt()
        {
            Assert.AreEqual(new Vec(0, 1), new Vec(1, 2) / 2);
            Assert.AreEqual(new Vec(-2, 3), new Vec(6, -9) / -3);
            Assert.AreEqual(new Vec(4, -5), new Vec(4, -5) / 1);
        }

        [Test]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestOperatorVecDividedByZeroThrows()
        {
            Vec dummy = new Vec(1, 3) / 0;
        }

        #endregion

        #region Static methods

        [Test]
        public void TestIsDistanceWithin()
        {
            Assert.AreEqual(true, Vec.IsDistanceWithin(new Vec(0, 0), new Vec(0, 0), 0));
            Assert.AreEqual(true, Vec.IsDistanceWithin(new Vec(2, 2), new Vec(2, 2), 0));
            Assert.AreEqual(true, Vec.IsDistanceWithin(new Vec(1, 7), new Vec(1, 7), 1));
            Assert.AreEqual(true, Vec.IsDistanceWithin(new Vec(2, 3), new Vec(1, 3), 1));
            Assert.AreEqual(true, Vec.IsDistanceWithin(new Vec(0, 0), new Vec(3, 4), 5));
            Assert.AreEqual(true, Vec.IsDistanceWithin(new Vec(-2, 0), new Vec(3, 0), 5));

            Assert.AreEqual(false, Vec.IsDistanceWithin(new Vec(2, 3), new Vec(0, 3), 1));
            Assert.AreEqual(false, Vec.IsDistanceWithin(new Vec(0, 0), new Vec(3, 4), 4));
            Assert.AreEqual(false, Vec.IsDistanceWithin(new Vec(-2, 0), new Vec(3, 0), 4));
        }

        #endregion

        #region Properties

        [Test]
        public void TestX()
        {
            Assert.AreEqual(3, new Vec(3, 2).X);
            Assert.AreEqual(-4, new Vec(-4, 0).X);
        }

        [Test]
        public void TestY()
        {
            Assert.AreEqual(3, new Vec(2, 3).Y);
            Assert.AreEqual(-4, new Vec(0, -4).Y);
        }

        [Test]
        public void TestArea()
        {
            Assert.AreEqual(0, new Vec(0, 0).Area);
            Assert.AreEqual(0, new Vec(1, 0).Area);
            Assert.AreEqual(-1, new Vec(1, -1).Area);
            Assert.AreEqual(4, new Vec(-2, -2).Area);
            Assert.AreEqual(6, new Vec(2, 3).Area);
        }

        [Test]
        public void TestLengthSquared()
        {
            Assert.AreEqual(0, new Vec(0, 0).LengthSquared);
            Assert.AreEqual(1, new Vec(1, 0).LengthSquared);
            Assert.AreEqual(1, new Vec(0, -1).LengthSquared);
            Assert.AreEqual(2, new Vec(1, 1).LengthSquared);
            Assert.AreEqual(5 * 5, new Vec(3, 4).LengthSquared);
        }

        [Test]
        public void TestLength()
        {
            Assert.AreEqual(0, new Vec(0, 0).Length);
            Assert.AreEqual(1, new Vec(1, 0).Length);
            Assert.AreEqual(1, new Vec(0, -1).Length);
            Assert.AreEqual((float)Math.Sqrt(2), new Vec(1, 1).Length);
            Assert.AreEqual(5, new Vec(3, 4).Length);
        }

        [Test]
        public void TestRookLength()
        {
            Assert.AreEqual(0, new Vec(0, 0).RookLength);
            Assert.AreEqual(1, new Vec(1, 0).RookLength);
            Assert.AreEqual(1, new Vec(0, -1).RookLength);
            Assert.AreEqual(2, new Vec(1, 1).RookLength);
            Assert.AreEqual(5, new Vec(2, -3).RookLength);
        }

        [Test]
        public void TestKingLength()
        {
            Assert.AreEqual(0, new Vec(0, 0).KingLength);
            Assert.AreEqual(1, new Vec(1, 0).KingLength);
            Assert.AreEqual(1, new Vec(0, -1).KingLength);
            Assert.AreEqual(1, new Vec(1, 1).KingLength);
            Assert.AreEqual(3, new Vec(2, -3).KingLength);
        }

        #endregion

        #region Methods

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("0, 1", new Vec(0, 1).ToString());
            Assert.AreEqual("17, 5", new Vec(17, 5).ToString());
            Assert.AreEqual("38274, -4273", new Vec(38274,-4273).ToString());
        }

        [Test]
        public void TestEquals()
        {
            Vec v1 = new Vec(3, 5);
            Vec v2 = new Vec(3, 5);
            Vec v3 = new Vec(4, 5);
            Vec v4 = new Vec(0, 0);
            Vec v5 = Vec.Zero;

            object obj2 = v2;

            // typed Equals
            Assert.IsTrue(v1.Equals(v1));
            Assert.IsTrue(v1.Equals(v2));
            Assert.IsFalse(v1.Equals(v3));
            Assert.IsFalse(v3.Equals(v4));
            Assert.IsTrue(v4.Equals(v5));

            // object Equals
            Assert.IsTrue(v1.Equals(obj2));
            Assert.IsTrue(obj2.Equals(v1));
            Assert.IsFalse(v3.Equals(obj2));

            // null
            Assert.IsFalse(v1.Equals(null));
        }

        [Test]
        public void TestGetHashCode()
        {
            // make sure the hash code is consistent
            Assert.AreEqual(new Vec(0, 0).GetHashCode(), new Vec(0, 0).GetHashCode());
            Assert.AreEqual(new Vec(1, 3).GetHashCode(), new Vec(1, 3).GetHashCode());
            Assert.AreEqual(new Vec(15, -123).GetHashCode(), new Vec(15, -123).GetHashCode());

            // just check for a few obvious collisions
            Assert.AreNotEqual(new Vec(0, 0).GetHashCode(), new Vec(0, 1).GetHashCode());
            Assert.AreNotEqual(new Vec(0, 1).GetHashCode(), new Vec(1, 0).GetHashCode());
            Assert.AreNotEqual(new Vec(100, -1).GetHashCode(), new Vec(100, 1).GetHashCode());
        }

        [Test]
        public void TestContains()
        {
            Assert.IsTrue(new Vec(2, 5).Contains(new Vec(0, 0)));
            Assert.IsTrue(new Vec(2, 5).Contains(new Vec(1, 0)));
            Assert.IsTrue(new Vec(2, 5).Contains(new Vec(1, 4)));

            Assert.IsFalse(new Vec(2, 5).Contains(new Vec(2, 3)));
            Assert.IsFalse(new Vec(2, 5).Contains(new Vec(1, 5)));
            Assert.IsFalse(new Vec(2, 5).Contains(new Vec(2, 5)));
            Assert.IsFalse(new Vec(2, 5).Contains(new Vec(-1, 0)));

            Assert.IsFalse(new Vec(0, 0).Contains(new Vec(0, 0)));
        }

        [Test]
        public void TestIsAdjacentTo()
        {
            Assert.IsTrue(new Vec(2, 5).IsAdjacentTo(new Vec(2, 6)));
            Assert.IsTrue(new Vec(2, 5).IsAdjacentTo(new Vec(1, 4)));
            Assert.IsTrue(new Vec(2, 5).IsAdjacentTo(new Vec(1, 5)));

            Assert.IsFalse(new Vec(2, 5).IsAdjacentTo(new Vec(2, 3)));
            Assert.IsFalse(new Vec(2, 5).IsAdjacentTo(new Vec(1, 3)));
            Assert.IsFalse(new Vec(2, 5).IsAdjacentTo(new Vec(2, 5)));
            Assert.IsFalse(new Vec(2, 5).IsAdjacentTo(new Vec(-1, 0)));
        }

        [Test]
        public void TestOffset()
        {
            Assert.AreEqual(new Vec(2, 5), new Vec(1, 3).Offset(1, 2));
            Assert.AreEqual(new Vec(0, 0), new Vec(1, 3).Offset(-1, -3));
            Assert.AreEqual(new Vec(3, 4), new Vec(3, 4).Offset(0, 0));

            // make sure the original is not changed
            Vec v = new Vec(2, 3);
            Vec offset = v.Offset(1, 5);

            Assert.AreEqual(2, v.X);
            Assert.AreEqual(3, v.Y);
        }

        [Test]
        public void TestOffsetX()
        {
            Assert.AreEqual(new Vec(2, 3), new Vec(1, 3).OffsetX(1));
            Assert.AreEqual(new Vec(0, 3), new Vec(1, 3).OffsetX(-1));
            Assert.AreEqual(new Vec(3, 4), new Vec(3, 4).OffsetX(0));

            // make sure the original is not changed
            Vec v = new Vec(2, 3);
            Vec offset = v.OffsetX(1);

            Assert.AreEqual(2, v.X);
            Assert.AreEqual(3, v.Y);
        }

        [Test]
        public void TestOffsetY()
        {
            Assert.AreEqual(new Vec(1, 4), new Vec(1, 3).OffsetY(1));
            Assert.AreEqual(new Vec(1, 2), new Vec(1, 3).OffsetY(-1));
            Assert.AreEqual(new Vec(3, 4), new Vec(3, 4).OffsetY(0));

            // make sure the original is not changed
            Vec v = new Vec(2, 3);
            Vec offset = v.OffsetY(1);

            Assert.AreEqual(2, v.X);
            Assert.AreEqual(3, v.Y);
        }

        [Test]
        public void TestEach()
        {
            Vec start = new Vec(2, 3);
            Vec result = start.Each((coord) => coord + 2);

            Assert.AreEqual(new Vec(2, 3), start);
            Assert.AreEqual(new Vec(4, 5), result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEachFuncIsNotNull()
        {
            Vec start = new Vec(2, 3);
            Vec result = start.Each(null);
        }

        #endregion
    }
}
