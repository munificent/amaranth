using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class Int32ExtensionsFixture
    {
        [Test]
        public void TestClamp()
        {
            Assert.AreEqual(3, (3).Clamp(-1, 15));
            Assert.AreEqual(5, (12).Clamp(3, 5));
            Assert.AreEqual(2, (1).Clamp(2, 4));
            Assert.AreEqual(3, (4).Clamp(3, 3));
            Assert.AreEqual(-2, (2).Clamp(-3, -2));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestClampThrowsIfMinGreaterThanMax()
        {
            int dummy = (0).Clamp(3, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemapIntThrowsIfNoRange()
        {
            int dummy = (5).Remap(4, 4, 4, 7);
        }

        [Test]
        public void TestRemapInt()
        {
            Assert.AreEqual(5, (3).Remap(3, 17, 5, 8));
            Assert.AreEqual(8, (17).Remap(3, 17, 5, 8));
            Assert.AreEqual(2, (0).Remap(2, 4, 3, 4));
            Assert.AreEqual(5, (6).Remap(2, 4, 3, 4));
            Assert.AreEqual(12, (4).Remap(2, 10, 10, 18));
        }
    }
}
