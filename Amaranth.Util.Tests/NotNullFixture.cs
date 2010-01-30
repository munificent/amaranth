using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class NotNullFixture
    {
        [Test]
        public void TestConstructor()
        {
            Foo foo = new Foo();
            NotNull<Foo> notNull = new NotNull<Foo>(foo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorThrowsOnNull()
        {
            Foo foo = null;
            NotNull<Foo> notNull = new NotNull<Foo>(foo);
        }

        [Test]
        public void TestGetValue()
        {
            Foo foo = new Foo();
            NotNull<Foo> notNull = new NotNull<Foo>(foo);

            Assert.AreEqual(foo, notNull.Value);
        }

        [Test]
        public void TestImplicitWrap()
        {
            Foo foo = new Foo();
            NotNull<Foo> notNull = foo;

            Assert.AreEqual(foo, notNull.Value);
        }

        [Test]
        public void TestImplicitUnwrap()
        {
            Foo foo = new Foo();
            NotNull<Foo> notNull = new NotNull<Foo>(foo);

            Foo unwrap = notNull;

            Assert.AreEqual(foo, unwrap);
        }

        public class Foo { }
    }
}
