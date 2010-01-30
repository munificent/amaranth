using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class Math2Fixture
    {
        [Test]
        public void TestClamp()
        {
            Assert.AreEqual(3, Math2.Clamp(-1, 3, 15));
            Assert.AreEqual(5, Math2.Clamp(3, 12, 5));
            Assert.AreEqual(2, Math2.Clamp(2, 1, 4));
            Assert.AreEqual(3, Math2.Clamp(3, 4, 3));
            Assert.AreEqual(-2, Math2.Clamp(-3, 2, -2));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestClampThrowsIfMinGreaterThanMax()
        {
            int dummy = Math2.Clamp(3, 0, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestNormalThrowsIfNoRange()
        {
            float dummy = Math2.Normal(4.0f, 5.0f, 4.0f);
        }

        [Test]
        public void TestNormal()
        {
            Assert.AreEqual(0.0f, Math2.Normal(3, 3, 17));
            Assert.AreEqual(1.0f, Math2.Normal(3, 17, 17));
            Assert.AreEqual(-1.0f, Math2.Normal(2, 0, 4));
            Assert.AreEqual(2.0f, Math2.Normal(2, 6, 4));
            Assert.AreEqual(0.25f, Math2.Normal(2, 4, 10));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemapIntThrowsIfNoRange()
        {
            int dummy = Math2.Remap(4, 5, 4, 4, 7);
        }

        [Test]
        public void TestRemapInt()
        {
            Assert.AreEqual( 5, Math2.Remap(3,  3, 17,  5,  8));
            Assert.AreEqual( 8, Math2.Remap(3, 17, 17,  5,  8));
            Assert.AreEqual( 2, Math2.Remap(2,  0,  4,  3,  4));
            Assert.AreEqual( 5, Math2.Remap(2,  6,  4,  3,  4));
            Assert.AreEqual(12, Math2.Remap(2,  4, 10, 10, 18));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemapFloatThrowsIfNoRange()
        {
            float dummy = Math2.Remap(4, 5, 4, 4, 7);
        }

        [Test]
        public void TestRemapFloat()
        {
            Assert.AreEqual( 5.0f, Math2.Remap(3.0f,  3.0f, 17.0f,  5.0f,  8.0f));
            Assert.AreEqual( 8.0f, Math2.Remap(3.0f, 17.0f, 17.0f,  5.0f,  8.0f));
            Assert.AreEqual( 2.0f, Math2.Remap(2.0f,  0.0f,  4.0f,  3.0f,  4.0f));
            Assert.AreEqual( 5.0f, Math2.Remap(2.0f,  6.0f,  4.0f,  3.0f,  4.0f));
            Assert.AreEqual(12.0f, Math2.Remap(2.0f,  4.0f, 10.0f, 10.0f, 18.0f));
        }

        [Test]
        public void TestSwapValue()
        {
            int left = 3;
            int right = 5;

            Math2.Swap(ref left, ref right);

            Assert.AreEqual(5, left);
            Assert.AreEqual(3, right);
        }

        [Test]
        public void TestSwapRef()
        {
            Foo originalLeft = new Foo();
            Foo originalRight = new Foo();
            Foo left = originalLeft;
            Foo right = originalRight;

            Math2.Swap(ref left, ref right);

            Assert.AreEqual(originalLeft, right);
            Assert.AreEqual(originalRight, left);
        }

        [Test]
        public void TestMin()
        {
            Assert.AreEqual(3.0f, Math2.Min(14.3f, 43.2f, 4.0f, 3.0f, 15.0f));
            Assert.AreEqual(-1.0f, Math2.Min(14.3f, -1.0f, 3.0f, 15.0f));
            Assert.AreEqual(12.12f, Math2.Min(12.12f));
        }

        [Test]
        public void TestMax()
        {
            Assert.AreEqual(43.2f, Math2.Max(14.3f, 43.2f, 4.0f, 3.0f, 15.0f));
            Assert.AreEqual(-1.0f, Math2.Max(-14.3f, -1.0f, -3.0f, -15.0f));
            Assert.AreEqual(12.12f, Math2.Max(12.12f));
        }

        public class Foo { }
    }
}
