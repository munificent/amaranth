using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Amaranth.Terminals
{
    public struct Character : IEquatable<Character>
    {
        /// <summary>
        /// Gets the default foreground <see cref="Color"/> for a Character.
        /// </summary>
        public static Color DefaultForeColor { get { return Color.White; } }

        /// <summary>
        /// Gets the default background <see cref="Color"/> for a Character.
        /// </summary>
        public static Color DefaultBackColor { get { return Color.Black; } }

        /// <summary>
        /// Gets the <see cref="Glyph"/> represented by the given ASCII character.
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns></returns>
        public static Glyph ToGlyph(char ascii)
        {
            return (Glyph)(ascii - 32);
        }

        /// <summary>
        /// Parses a Character from the given string formatted like:
        /// <code>Glyph [ForeColor [BackColor]]</code>
        /// "Glyph" can either be the case-insensitive name of a value in the <see cref="Glyph"/>
        /// enum, or a single character representing the ASCII value of the glyph. ForeColor and
        /// BackColor, if present, are the case-insensitive names of values in
        /// <see cref="TerminalColors"/>.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>A Character defined by the given text.</returns>
        /// <exception cref="ArgumentNullException"><c>text</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>text</c> is empty or contains more than three words.</exception>
        public static Character Parse(string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            if (text.Length == 0) throw new ArgumentException("Argument 'text' cannot be empty.");

            text = text.Trim();

            // separate out the colors and glyph
            string[] parts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // only supports three parts (max)
            if (parts.Length > 3) throw new ArgumentException("Character.Parse() should be formatted \"Glyph\", \"ForeColor Glyph\", or \"ForeColor BackColor Glyph\".");

            Glyph glyph;
            Color foreColor = DefaultForeColor;
            Color backColor = DefaultBackColor;

            // parse the glyph
            glyph = ParseGlyph(parts[parts.Length - 1]);

            // parse the fore color
            if (parts.Length > 1)
            {
                foreColor = TerminalColors.FromName(parts[0]);
            }

            // parse the back color
            if (parts.Length > 2)
            {
                backColor = TerminalColors.FromName(parts[1]);
            }

            return new Character(glyph, foreColor, backColor);
        }

        public static Glyph ParseGlyph(string text)
        {
            if (text.Length == 1)
            {
                // a single character is assumed to be ascii
                return ToGlyph(text[0]);
            }
            else
            {
                // multiple characters are the glyph enum names
                return (Glyph)Enum.Parse(typeof(Glyph), text, true);
            }
        }

        /// <summary>
        /// Gets the <see cref="Glyph"/> used to draw this Character.
        /// </summary>
        public Glyph Glyph { get { return mGlyph; } }

        /// <summary>
        /// Gets the foreground <see cref="Color"/> of this Character.
        /// </summary>
        public Color ForeColor
        {
            get
            {
                // default if empty
                if (mForeColor == Color.Empty) mForeColor = DefaultForeColor;

                return mForeColor;
            }
        }

        /// <summary>
        /// Gets the background <see cref="Color"/> of this Character.
        /// </summary>
        public Color BackColor
        {
            get
            {
                // default if empty
                if (mBackColor == Color.Empty) mBackColor = DefaultBackColor;

                return mBackColor;
            }
        }

        /// <summary>
        /// Returns true if the <see cref="Glyph"/> for this Character is a non-visible
        /// whitespace Glyph.
        /// </summary>
        public bool IsWhitespace { get { return mGlyph == Glyph.Space; } }

        /// <summary>
        /// Initializes a new Character.
        /// </summary>
        /// <param name="glyph">Glyph used to draw the Character.</param>
        /// <param name="foreColor">Foreground <see cref="Color"/> of the Character.</param>
        /// <param name="backColor">Background <see cref="Color"/> of the Character.</param>
        public Character(Glyph glyph, Color foreColor, Color backColor)
        {
            mGlyph = glyph;
            mBackColor = backColor;
            mForeColor = foreColor;
        }

        /// <summary>
        /// Initializes a new Character using the default background <see cref="Color"/>.
        /// </summary>
        /// <param name="glyph">Glyph used to draw the Character.</param>
        /// <param name="foreColor">Foreground <see cref="Color"/> of the Character.</param>
        public Character(Glyph glyph, Color foreColor)
            : this(glyph, foreColor, DefaultBackColor)
        {
        }

        /// <summary>
        /// Initializes a new Character using the default background and foreground
        /// <see cref="Color"/>.
        /// </summary>
        /// <param name="glyph">Glyph used to draw the Character.</param>
        public Character(Glyph glyph)
            : this(glyph, DefaultForeColor)
        {
        }

        /// <summary>
        /// Initializes a new Character.
        /// </summary>
        /// <param name="ascii">ASCII representation of the <see cref="Glyph"/> used
        /// to draw the Character.</param>
        /// <param name="foreColor">Foreground <see cref="Color"/> of the Character.</param>
        /// <param name="backColor">Background <see cref="Color"/> of the Character.</param>
        public Character(char ascii, Color foreColor, Color backColor)
            : this(Character.ToGlyph(ascii), foreColor, backColor)
        {
        }

        /// <summary>
        /// Initializes a new Character using the default background <see cref="Color"/>.
        /// </summary>
        /// <param name="ascii">ASCII representation of the <see cref="Glyph"/> used
        /// to draw the Character.</param>
        /// <param name="foreColor">Foreground <see cref="Color"/> of the Character.</param>
        public Character(char ascii, Color foreColor)
            : this(Character.ToGlyph(ascii), foreColor, DefaultBackColor)
        {
        }

        /// <summary>
        /// Initializes a new Character using the default background and foreground
        /// <see cref="Color"/>.
        /// </summary>
        /// <param name="ascii">ASCII representation of the <see cref="Glyph"/> used
        /// to draw the Character.</param>
        public Character(char ascii)
            : this(Character.ToGlyph(ascii), DefaultForeColor)
        {
        }

        /// <summary>
        /// Gets a string representation of this Character.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mGlyph.ToString();
        }

        /// <summary>
        /// Determines whether the specified object equals this <see cref="Character"/>.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns><c>true</c> if <c>obj</c> is a Character equivalent to this Character; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Character) return Equals((Character)obj);

            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Character"/>.
        /// </summary>
        /// <returns>An integer value that specifies the hash code for this Character.</returns>
        public override int GetHashCode()
        {
            return mGlyph.GetHashCode() + mBackColor.GetHashCode() + mForeColor.GetHashCode();
        }

        #region IEquatable<Character> Members

        /// <summary>
        /// Determines whether the specified <see cref="Character"/> equals this one.
        /// </summary>
        /// <param name="other">The <see cref="Character"/> to test.</param>
        /// <returns><c>true</c> if <c>other</c> is equivalent to this Character; otherwise, <c>false</c>.</returns>
        public bool Equals(Character other)
        {
            return (mGlyph == other.mGlyph) && mBackColor.Equals(other.mBackColor) && mForeColor.Equals(other.mForeColor);
        }

        #endregion

        private Glyph mGlyph;
        private Color mForeColor;
        private Color mBackColor;
    }
}
