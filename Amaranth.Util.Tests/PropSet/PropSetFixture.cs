using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class PropSetFixture
    {
        #region Constructor

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValueBase_ThrowsOnNullName()
        {
            PropSet dummy = new PropSet(null, "bar", new PropSet[] { new PropSet("base") });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValueBase_ThrowsOnNullValue()
        {
            PropSet dummy = new PropSet("foo", null, new PropSet[] { new PropSet("base") });
        }

        [Test]
        public void Constructor_NameValueBase_DoesNotThrowOnNullBase()
        {
            PropSet dummy = new PropSet("foo", "bar", null);
        }

        [Test]
        public void Constructor_NameValueBase()
        {
            PropSet prop = new PropSet("foo", "bar", new PropSet[] { new PropSet("base") });

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual("bar", prop.Value);
            Assert.AreEqual(0, prop.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValue_ThrowsOnNullName()
        {
            PropSet dummy = new PropSet(null, "bar");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameValue_ThrowsOnNullValue()
        {
            PropSet dummy = new PropSet("foo", (string)null);
        }

        [Test]
        public void Constructor_NameValue()
        {
            PropSet prop = new PropSet("foo", "bar");

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual("bar", prop.Value);
            Assert.AreEqual(0, prop.Count);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NameBase_ThrowsOnNullName()
        {
            PropSet dummy = new PropSet(null, new PropSet[] { new PropSet("base") });
        }

        [Test]
        public void Constructor_NameBase_DoesNotThrowOnNullBase()
        {
            PropSet dummy = new PropSet("foo", (PropSet[])null);
        }

        [Test]
        public void Constructor_NameBase_DoesNotThrowOnEmptyBase()
        {
            PropSet dummy = new PropSet("foo", new PropSet[0]);
        }

        [Test]
        public void Constructor_NameBase()
        {
            PropSet prop = new PropSet("foo", new PropSet[] { new PropSet("base") });

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual(String.Empty, prop.Value);
            Assert.AreEqual(0, prop.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Name_ThrowsOnNull()
        {
            PropSet dummy = new PropSet(null);
        }

        [Test]
        public void Constructor_Name()
        {
            PropSet prop = new PropSet("foo");

            Assert.AreEqual("foo", prop.Name);
            Assert.AreEqual(String.Empty, prop.Value);
            Assert.AreEqual(0, prop.Count);
        }

        #endregion

        [Test]
        public void Add()
        {
            PropSet prop = new PropSet("foo", "bar");

            Assert.AreEqual(0, prop.Count);

            prop.Add(new PropSet("item", "value"));

            Assert.AreEqual(1, prop.Count);
            Assert.AreEqual("item", prop["item"].Name);
            Assert.AreEqual("value", prop["item"].Value);
        }

        [Test]
        public void Contains()
        {
            PropSet prop = new PropSet("foo", "bar");

            prop.Add(new PropSet("name", "value"));

            Assert.IsTrue(prop.Contains("name"));
            Assert.IsFalse(prop.Contains("not name"));
        }

        [Test]
        public void GetOrDefault_String()
        {
            PropSet prop = new PropSet("foo", "bar");

            prop.Add(new PropSet("name", "value"));

            Assert.AreEqual("value", prop.GetOrDefault("name", "default"));
            Assert.AreEqual("default", prop.GetOrDefault("not name", "default"));
        }

        [Test]
        public void GetOrDefault_Int()
        {
            PropSet prop = new PropSet("foo", "bar");

            prop.Add(new PropSet("name", "123"));

            Assert.AreEqual(123, prop.GetOrDefault("name", 666));
            Assert.AreEqual(666, prop.GetOrDefault("not name", 666));
        }

        [Test]
        public void GetOrDefault_Converter()
        {
            PropSet prop = new PropSet("foo", "bar");

            prop.Add(new PropSet("name", "value"));

            Assert.AreEqual("value (added)", prop.GetOrDefault("name", value => value + " (added)", "default"));
            Assert.AreEqual("default", prop.GetOrDefault("not name", "default"));
        }

        [Test]
        public void InheritValuesFromBases()
        {
            PropSet base1Prop = new PropSet("base1");
            base1Prop.Add(new PropSet("from base 1", "value 1"));

            PropSet base2Prop = new PropSet("base2");
            base2Prop.Add(new PropSet("from base 2", "value 2"));

            PropSet derivedProp = new PropSet("derived", new PropSet[] { base1Prop, base2Prop });
            derivedProp.Add(new PropSet("from derived", "value"));

            Assert.AreEqual(3, derivedProp.Count);
            Assert.AreEqual("value 1", derivedProp["from base 1"].Value);
            Assert.AreEqual("value 2", derivedProp["from base 2"].Value);
            Assert.AreEqual("value", derivedProp["from derived"].Value);
        }

        [Test]
        public void OverrideAcrossBases()
        {
            PropSet base1Prop = new PropSet("base1");
            base1Prop.Add(new PropSet("from base", "value 1"));

            PropSet base2Prop = new PropSet("base2");
            base2Prop.Add(new PropSet("from base", "value 2"));

            PropSet derivedProp = new PropSet("derived", new PropSet[] { base1Prop, base2Prop });

            Assert.AreEqual(1, derivedProp.Count);
            Assert.AreEqual("value 2", derivedProp["from base"].Value);
        }

        [Test]
        public void OverrideValueFromBase()
        {
            PropSet baseProp = new PropSet("base");
            baseProp.Add(new PropSet("from base", "value"));
            baseProp.Add(new PropSet("override", "base value"));

            PropSet derivedProp = new PropSet("derived", new PropSet[] { baseProp });
            derivedProp.Add(new PropSet("from derived", "value"));
            derivedProp.Add(new PropSet("override", "derived value"));

            Assert.AreEqual(3, derivedProp.Count);
            Assert.AreEqual("value", derivedProp["from base"].Value);
            Assert.AreEqual("derived value", derivedProp["override"].Value);
        }
    }
}