using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class SingleExtensionsFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestNormalizeThrowsIfNoRange()
        {
            float dummy = (5.0f).Normalize(4.0f, 4.0f);
        }

        [Test]
        public void TestNormalize()
        {
            Assert.AreEqual(0.0f, (3.0f).Normalize(3, 17));
            Assert.AreEqual(1.0f, (17.0f).Normalize(3, 17));
            Assert.AreEqual(-1.0f, (0.0f).Normalize(2, 4));
            Assert.AreEqual(2.0f, (6.0f).Normalize(2, 4));
            Assert.AreEqual(0.25f, (4.0f).Normalize(2, 10));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemapFloatThrowsIfNoRange()
        {
            float dummy = (5).Remap(4, 4, 4, 7);
        }

        [Test]
        public void TestRemapFloat()
        {
            Assert.AreEqual(5.0f, (3.0f).Remap(3.0f, 17.0f, 5.0f, 8.0f));
            Assert.AreEqual(8.0f, (17.0f).Remap(3.0f, 17.0f, 5.0f, 8.0f));
            Assert.AreEqual(2.0f, (0.0f).Remap(2.0f, 4.0f, 3.0f, 4.0f));
            Assert.AreEqual(5.0f, (6.0f).Remap(2.0f, 4.0f, 3.0f, 4.0f));
            Assert.AreEqual(12.0f, (4.0f).Remap(2.0f, 10.0f, 10.0f, 18.0f));
        }
    }
}
