using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class IndentationTreeFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_ThrowsOnNull()
        {
            IndentationTree dummy = IndentationTree.Parse(null);
        }

        [Test]
        public void Parse_Empty()
        {
            IndentationTree tree = IndentationTree.Parse(new string[0]);

            Assert.AreEqual(-1, tree.Indent);
            Assert.AreEqual(String.Empty, tree.Text);
            Assert.AreEqual(0, tree.Children.Count);
        }

        [Test]
        public void Parse_SingleLine()
        {
            List<string> lines = new List<string>{ "single line" };
            IndentationTree tree = IndentationTree.Parse(lines);

            Assert.AreEqual(-1, tree.Indent);
            Assert.AreEqual(String.Empty, tree.Text);

            Assert.AreEqual(1, tree.Children.Count);
            Assert.AreEqual(0, tree.Children[0].Indent);
            Assert.AreEqual("single line", tree.Children[0].Text);
        }

        [Test]
        public void Parse_ThreeLines()
        {
            List<string> lines = new List<string> { "one", "two", "three" };
            IndentationTree tree = IndentationTree.Parse(lines);

            Assert.AreEqual(-1, tree.Indent);
            Assert.AreEqual(String.Empty, tree.Text);

            Assert.AreEqual(3, tree.Children.Count);
            Assert.AreEqual(0, tree.Children[0].Indent);
            Assert.AreEqual("one", tree.Children[0].Text);
            Assert.AreEqual(0, tree.Children[1].Indent);
            Assert.AreEqual("two", tree.Children[1].Text);
            Assert.AreEqual(0, tree.Children[2].Indent);
            Assert.AreEqual("three", tree.Children[2].Text);
        }

        [Test]
        public void Parse_IndentedLines()
        {
            List<string> lines = new List<string>
            {   "one",
                "   two",
                "     three",
                "  four",
                "five"
            };

            IndentationTree tree = IndentationTree.Parse(lines);

            // root
            Assert.AreEqual(-1, tree.Indent);
            Assert.AreEqual(String.Empty, tree.Text);
            Assert.AreEqual(2, tree.Children.Count);

            // one
            Assert.AreEqual(0, tree.Children[0].Indent);
            Assert.AreEqual("one", tree.Children[0].Text);
            Assert.AreEqual(2, tree.Children[0].Children.Count);

            // two
            Assert.AreEqual(1, tree.Children[0].Children[0].Indent);
            Assert.AreEqual("two", tree.Children[0].Children[0].Text);
            Assert.AreEqual(1, tree.Children[0].Children[0].Children.Count);

            // three
            Assert.AreEqual(2, tree.Children[0].Children[0].Children[0].Indent);
            Assert.AreEqual("three", tree.Children[0].Children[0].Children[0].Text);
            Assert.AreEqual(0, tree.Children[0].Children[0].Children[0].Children.Count);

            // four
            Assert.AreEqual(1, tree.Children[0].Children[1].Indent);
            Assert.AreEqual("four", tree.Children[0].Children[1].Text);
            Assert.AreEqual(0, tree.Children[0].Children[1].Children.Count);

            // five
            Assert.AreEqual(0, tree.Children[1].Indent);
            Assert.AreEqual("five", tree.Children[1].Text);
            Assert.AreEqual(0, tree.Children[1].Children.Count);
        }

        [Test]
        public void Test_ToString()
        {
            IndentationTree tree = IndentationTree.Parse(
                new string[]
                {   "one",
                    "   two",
                    "     three",
                    "  four",
                    "five"
                });

            Assert.AreEqual("one\r\n  two\r\n    three\r\n  four\r\nfive\r\n", tree.ToString());
        }
    }
}