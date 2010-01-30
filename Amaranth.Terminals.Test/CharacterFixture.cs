using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Terminals;

namespace Amaranth.Terminals.Tests
{
    [TestFixture]
    public class CharacterFixture
    {
        #region Static properties

        [Test]
        public void TestDefaultForeColor()
        {
            Assert.AreEqual(Color.White, Character.DefaultForeColor);
        }

        [Test]
        public void TestDefaultBackColor()
        {
            Assert.AreEqual(Color.Black, Character.DefaultBackColor);
        }

        #endregion

        #region Static methods

        [Test]
        public void TestToGlyph()
        {
            Assert.AreEqual(Glyph.ALower,       Character.ToGlyph('a'));
            Assert.AreEqual(Glyph.DUpper,       Character.ToGlyph('D'));
            Assert.AreEqual(Glyph.Asterisk,     Character.ToGlyph('*'));
            Assert.AreEqual(Glyph.Space,        Character.ToGlyph(' '));
            Assert.AreEqual(Glyph.Backslash,    Character.ToGlyph('\\'));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseThrowsIfNull()
        {
            Character dummy = Character.Parse(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseThrowsIfEmpty()
        {
            Character dummy = Character.Parse("");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseThrowsIfTooManyParts()
        {
            Character dummy = Character.Parse("  Gold   Yellow  toomany ~ ");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseThrowsIfBadGlyphName()
        {
            Character dummy = Character.Parse(" NotARealGlyph ");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseThrowsIfBadForeColor()
        {
            Character dummy = Character.Parse("  Fuschia ~ ");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseThrowsIfBadBackColor()
        {
            Character dummy = Character.Parse("  Gold   BurntSienna ~ ");
        }

        [Test]
        public void TestParse()
        {
            TestParse(Glyph.ALower, Character.DefaultForeColor, Character.DefaultBackColor, "a");
            TestParse(Glyph.HUpper, Character.DefaultForeColor, Character.DefaultBackColor, " H  ");
            TestParse(Glyph.Slash, Character.DefaultForeColor, Character.DefaultBackColor, " /  ");

            TestParse(Glyph.Dash, TerminalColors.Red, Character.DefaultBackColor, "   Red - ");
            TestParse(Glyph.Pipe, TerminalColors.DarkBlue, Character.DefaultBackColor, "DarkBlue |");

            TestParse(Glyph.MUpper, TerminalColors.Green, TerminalColors.Pink, "Green Pink M");
            TestParse(Glyph.Percent, TerminalColors.Black, TerminalColors.Black, " Black   Black  % ");
        }

        #endregion

        #region Properties

        [Test]
        public void TestGlyph()
        {
            Character c = new Character(Glyph.SolidFill);
            Assert.AreEqual(Glyph.SolidFill, c.Glyph);
        }

        [Test]
        public void TestForeColor()
        {
            Character c = new Character(Glyph.SolidFill, Color.Red);
            Assert.AreEqual(Color.Red, c.ForeColor);
        }

        [Test]
        public void TestBackColor()
        {
            Character c = new Character(Glyph.SolidFill, Color.Red, Color.Blue);
            Assert.AreEqual(Color.Blue, c.BackColor);
        }

        [Test]
        public void TestIsWhitespace()
        {
            Character c1 = new Character(Glyph.Space);
            Assert.IsTrue(c1.IsWhitespace);

            Character c2 = new Character(Glyph.Percent);
            Assert.IsFalse(c2.IsWhitespace);
        }

        #endregion

        #region Constructors

        [Test]
        public void TestConstructors()
        {
            Character c1 = new Character();
            Test(Glyph.Space, Character.DefaultForeColor, Character.DefaultBackColor, c1);

            Character c2 = new Character(Glyph.SolidFill);
            Test(Glyph.SolidFill, Character.DefaultForeColor, Character.DefaultBackColor, c2);

            Character c3 = new Character(Glyph.Door, Color.Red);
            Test(Glyph.Door, Color.Red, Character.DefaultBackColor, c3);

            Character c4 = new Character(Glyph.Face, Color.Red, Color.Blue);
            Test(Glyph.Face, Color.Red, Color.Blue, c4);

            Character c5 = new Character('%');
            Test(Glyph.Percent, Character.DefaultForeColor, Character.DefaultBackColor, c5);

            Character c6 = new Character(';', Color.Red);
            Test(Glyph.Semicolon, Color.Red, Character.DefaultBackColor, c6);

            Character c7 = new Character('=', Color.Red, Color.Blue);
            Test(Glyph.Equals, Color.Red, Color.Blue, c7);
        }

        #endregion

        #region Methods

        [Test]
        public void TestEquals()
        {
            Character c1 = new Character(Glyph.Space, Color.Red, Color.Blue);
            Character c2 = new Character(Glyph.Space, Color.Red, Color.Blue);
            Character c3 = new Character(Glyph.Face, Color.Red, Color.Blue);
            Character c4 = new Character(Glyph.Space, Color.Pink, Color.Blue);
            Character c5 = new Character(Glyph.Space, Color.Red, Color.Gray);

            Assert.IsTrue(c1.Equals(c2));
            Assert.IsFalse(c1.Equals(c3));
            Assert.IsFalse(c1.Equals(c4));
            Assert.IsFalse(c1.Equals(c5));
        }

        [Test]
        public void TestEqualsObject()
        {
            object c1 = new Character(Glyph.Space, Color.Red, Color.Blue);
            object c2 = new Character(Glyph.Space, Color.Red, Color.Blue);
            object c3 = new Character(Glyph.Face, Color.Red, Color.Blue);
            object c4 = new Character(Glyph.Space, Color.Pink, Color.Blue);
            object c5 = new Character(Glyph.Space, Color.Red, Color.Gray);

            Assert.IsTrue(c1.Equals(c2));
            Assert.IsFalse(c1.Equals(c3));
            Assert.IsFalse(c1.Equals(c4));
            Assert.IsFalse(c1.Equals(c5));
            Assert.IsFalse(c1.Equals(null));
        }

        #endregion

        #region Helper methods

        private void TestParse(Glyph glyph, Color foreColor, Color backColor, string text)
        {
            Character c = Character.Parse(text);
            Test(glyph, foreColor, backColor, c);
        }

        private void Test(Glyph glyph, Color foreColor, Color backColor, Character c)
        {
            Assert.AreEqual(glyph, c.Glyph);
            Assert.AreEqual(foreColor, c.ForeColor);
            Assert.AreEqual(backColor, c.BackColor);
        }

        #endregion
    }
}
