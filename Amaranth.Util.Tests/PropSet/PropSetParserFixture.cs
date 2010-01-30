using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class PropSetParserFixture
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            // make a temp dir
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }

            Directory.CreateDirectory(TempDir);

            // dump some files there
            File.WriteAllLines(Path.Combine(TempDir, "include.txt"),
                new string[] { "included text", "#include \"" + TempDir + "\\nested.txt\"" });

            // dump a file there
            File.WriteAllLines(Path.Combine(TempDir, "nested.txt"), new string[] { "nested line" });

            // dump some files in a subdir
            string subDir = Path.Combine(TempDir, "sub");
            Directory.CreateDirectory(subDir);

            File.WriteAllLines(Path.Combine(subDir, "a.txt"), new string[] { "a" });
            File.WriteAllLines(Path.Combine(subDir, "b.txt"), new string[] { "b" });
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            // clean up
            Directory.Delete(TempDir, true);
        }

        #region StripEmptyLines

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StripEmptyLines_ThrowsOnNull()
        {
            IEnumerable<string> dummy = PropSetParser.StripEmptyLines(null);
        }

        [Test]
        public void StripEmptyLines_NoLines()
        {
            List<string> result = new List<string>(PropSetParser.StripEmptyLines(new string[0]));

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void StripEmptyLines()
        {
            List<string> lines = new List<string>
            {   "",
                "a",
                "  ",
                "b",
                "  c",
                "      ",
                "d",
                "",
                "e",
                " "
            };

            List<string> results = new List<string>(PropSetParser.StripEmptyLines(lines));

            Assert.AreEqual(5, results.Count);

            Assert.AreEqual("a", results[0]);
            Assert.AreEqual("b", results[1]);
            Assert.AreEqual("  c", results[2]);
            Assert.AreEqual("d", results[3]);
            Assert.AreEqual("e", results[4]);
        }

        #endregion

        #region StripComments

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StripComments_ThrowsOnNull()
        {
            IEnumerable<string> dummy = PropSetParser.StripComments(null);
        }

        [Test]
        public void StripComments_NoLines()
        {
            List<string> result = new List<string>(PropSetParser.StripComments(new string[0]));

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void StripComments()
        {
            List<string> lines = new List<string>
            {   "no comments",
                "", // empty line
                "  ", // whitespace only
                "// comment at start of line",
                "  // comment after whitespace",
                " // comment with // other comment",
                "stuff before // comment",
                "stuff before // comment // with comment",
                "single / slash",
                "single / slash and // comment"
            };

            List<string> results = new List<string>(PropSetParser.StripComments(lines));

            Assert.AreEqual(lines.Count, results.Count);

            Assert.AreEqual("no comments",          results[0]);
            Assert.AreEqual("",                     results[1]);
            Assert.AreEqual("  ",                   results[2]);
            Assert.AreEqual("",                     results[3]);
            Assert.AreEqual("  ",                   results[4]);
            Assert.AreEqual(" ",                    results[5]);
            Assert.AreEqual("stuff before ",        results[6]);
            Assert.AreEqual("stuff before ",        results[7]);
            Assert.AreEqual("single / slash",       results[8]);
            Assert.AreEqual("single / slash and ",  results[9]);
        }

        #endregion

        #region ParseIncludes

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseIncludes_ThrowsOnNull()
        {
            IEnumerable<string> dummy = PropSetParser.ParseIncludes(null);

            // needed to make sure dummy is evaluated
            int count = dummy.Count();
        }

        [Test]
        public void ParseIncludes()
        {

            IEnumerable<string> results = PropSetParser.ParseIncludes(
                new string[]
                {
                    "before",
                    "#include \"" + TempDir + "\\include.txt\"",
                    "middle",
                    "#include \"" + TempDir + "\\sub\"",
                    "#include \"" + TempDir + "\\some missing file.txt\"",
                    "after"
                });

            List<string> lines = new List<string>(results);

            Assert.AreEqual(7, lines.Count);
            Assert.AreEqual("before", lines[0]);
            Assert.AreEqual("included text", lines[1]);
            Assert.AreEqual("nested line", lines[2]);
            Assert.AreEqual("middle", lines[3]);
            Assert.AreEqual("a", lines[4]);
            Assert.AreEqual("b", lines[5]);
            Assert.AreEqual("after", lines[6]);
        }

        #endregion

        #region Parse

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_ThrowsOnNull()
        {
            PropSet prop = PropSetParser.Parse(null);
        }

        [Test]
        public void Parse_Simple()
        {
            IndentationTree tree = IndentationTree.Parse(new string[] { "simple name = simple value" });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("simple name", "simple value"));
        }

        [Test]
        public void Parse_OddCharacters()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[] { "!@#$%^&*-_+[]\\|{};:'\",<.>/? = !@#$%^&*-=_+[]\\|{};:'\",<.>/?" });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("!@#$%^&*-_+[]\\|{};:'\",<.>/?", "!@#$%^&*-=_+[]\\|{};:'\",<.>/?"));
        }

        [Test]
        public void Parse_Multiline()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[]
                {
                    "multi-line =",
                    "  first",
                    "  second",
                    "  third",
                    "nest",
                    "   multi-line =",
                    "       first",
                    "       second",
                    "       third",
                });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("multi-line", "first second third"),
                AssertProp("nest", String.Empty,
                    AssertProp("multi-line", "first second third")));
        }

        [Test]
        public void Parse_Nested()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[]
                {
                    "parent",
                    "  child1 = 1",
                    "  child2",
                    "    grandchild1 = blah",
                    "    grandchild2 = blah",
                    "  child3"
                });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("parent", String.Empty,
                    AssertProp("child1", "1"),
                    AssertProp("child2", String.Empty,
                        AssertProp("grandchild1", "blah"),
                        AssertProp("grandchild2", "blah")),
                    AssertProp("child3", String.Empty)));
        }

        [Test]
        public void Parse_TrimmedWhitespace()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[] { "whitespace after    =    and before  ", });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("whitespace after", "and before"));
        }

        [Test]
        public void Parse_Inherits()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[]
                {
                    "base",
                    "derive with same name",
                    ":: abstract base",
                    "derived :: base",
                    "  child base :: base",
                    "  other child :: child base",
                    "  derive with same name ::",
                    "other derived :: abstract base"
                });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("base", String.Empty),
                AssertProp("derive with same name", String.Empty),
                AssertProp("derived", String.Empty, "base",
                    AssertProp("child base", String.Empty, "base"),
                    AssertProp("other child", String.Empty, "child base"),
                    AssertProp("derive with same name", String.Empty, "derive with same name")),
                AssertProp("other derived", String.Empty, "abstract base"));
        }

        [Test]
        public void Parse_InheritWithSameNameDoesNotReuseSelf()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[]
                {
                    "foo",
                    "  bar = in base",
                    "a",
                    "  foo ::",
                    "    bar = overridden",
                    "b",
                    "  foo ::"
                });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("foo", String.Empty,
                    AssertProp("bar", "in base")),
                AssertProp("a", String.Empty,
                    AssertProp("foo", String.Empty,
                        AssertProp("bar", "overridden"))),
                AssertProp("b", String.Empty,
                    AssertProp("foo", String.Empty,
                        AssertProp("bar", "in base"))));
        }

        [Test]
        public void Parse_MultipleInheritance()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[]
                {
                    ":: a",
                    "  foo = from a",
                    "  bar = from a",
                    ":: b",
                    "  foo = from b",
                    "  baz = from b",
                    "c :: a :: b",
                    "  spang = from c"
                });

            PropSet prop = PropSetParser.Parse(tree);

            AssertProp(prop, String.Empty, String.Empty,
                AssertProp("c", String.Empty, "a,b",
                    AssertProp("foo", "from b"),
                    AssertProp("bar", "from a"),
                    AssertProp("baz", "from b"),
                    AssertProp("spang", "from c")));
        }

        #endregion

        private string TempDir
        {
            get
            {
                string tempDir = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                return Path.Combine(tempDir, "Amaranth.Util.Tests");
            }
        }

        private void AssertProp(PropSet prop, string name, string value, string baseNames, params PropAsserter[] children)
        {
            PropAsserter assert = AssertProp(name, value, baseNames, children);
            assert.Test(prop);
        }

        private void AssertProp(PropSet prop, string name, string value, params PropAsserter[] children)
        {
            PropAsserter assert = AssertProp(name, value, String.Empty, children);
            assert.Test(prop);
        }

        private PropAsserter AssertProp(string name, string value, string baseNames, params PropAsserter[] children)
        {
            return new PropAsserter(name, value, baseNames, children);
        }

        private PropAsserter AssertProp(string name, string value, params PropAsserter[] children)
        {
            return new PropAsserter(name, value, String.Empty, children);
        }

        private class PropAsserter
        {
            public PropAsserter(string name, string value, string baseNames, params PropAsserter[] children)
            {
                mName = name;
                mValue = value;
                mBaseNames = baseNames;

                mChildren.AddRange(children);
            }

            public void Test(PropSet prop)
            {
                Assert.AreEqual(mName, prop.Name);
                Assert.AreEqual(mValue, prop.Value);

                if (!String.IsNullOrEmpty(mBaseNames))
                {
                    string[] bases = mBaseNames.Split(',');

                    Assert.AreEqual(bases.Length, prop.Bases.Count, "Property " + prop.Name + " does not inherit the expected bases " + mBaseNames + ".");
                    for (int i = 0; i < bases.Length; i++)
                    {
                        Assert.AreEqual(bases[i], prop.Bases[i].Name);
                    }
                }

                Assert.AreEqual(mChildren.Count, prop.Count);
                foreach (PropAsserter child in mChildren)
                {
                    child.Test(prop[child.mName]);
                }
            }

            private string mName;
            private string mValue;
            private string mBaseNames;
            private List<PropAsserter> mChildren = new List<PropAsserter>();
        }
    }
}