using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Amaranth.Terminals
{
    /// <summary>
    /// A mutable string of <see cref="Character">Characters</see>.
    /// </summary>
    public class CharacterString : List<Character>
    {
        public CharacterString()
        {
        }

        public CharacterString(IEnumerable<Character> characters)
            : base(characters)
        {
        }

        /// <summary>
        /// Initializes a new CharacterString using the given string of ASCII characters.
        /// Color escape codes will not be parsed out of the string.
        /// </summary>
        /// <param name="text"></param>
        public CharacterString(string text)
        {
            foreach (char c in text)
            {
                Add(new Character(c));
            }
        }

        /// <summary>
        /// Initializes a new CharacterString using the given string of ASCII characters.
        /// Color escape codes will also be parsed out of the string.
        /// </summary>
        /// <param name="text"></param>
        public CharacterString(string text, Color foreColor, Color backColor)
        {
            Color originalForeColor = foreColor;

            bool waitingForColor = false;

            foreach (char c in text)
            {
                // see if this character should be a color code
                if (waitingForColor)
                {
                    if (c == '-')
                    {
                        // - means "return to original color"
                        foreColor = originalForeColor;
                    }
                    else
                    {
                        foreColor = TerminalColors.FromEscapeChar(c);
                    }
                    waitingForColor = false;
                }
                else
                {
                    // handle color escape keys
                    if (c == '^')
                    {
                        waitingForColor = true;
                    }
                    else
                    {
                        Add(new Character(c, foreColor, backColor));
                    }
                }
            }
        }

        //### bob: copied from Util.Text
        public CharacterString[] WordWrap(int lineWidth)
        {
            List<CharacterString> lines = new List<CharacterString>();
            CharacterString line;

            int lastWrapPoint = 0;
            int thisLineStart = 0;
            for (int i = 0; i < Count; i++)
            {
                Character c = this[i];

                if (c.IsWhitespace)
                {
                    lastWrapPoint = i;
                }

                // wrap if we got too long
                if (i - thisLineStart >= lineWidth)
                {
                    if (lastWrapPoint != 0)
                    {
                        // have a recent point to wrap at, so word wrap
                        line = Substring(thisLineStart, lastWrapPoint - thisLineStart);
                        thisLineStart = lastWrapPoint;
                    }
                    else
                    {
                        // no convenient point to word wrap, so character wrap
                        line = Substring(thisLineStart, i - thisLineStart);
                        thisLineStart = i;
                    }

                    line = line.Trim();
                    lines.Add(line);
                }
            }

            // add the last bit
            line = Substring(thisLineStart);
            line = line.Trim();
            lines.Add(line);

            return lines.ToArray();
        }

        /// <summary>
        /// Creates a new CharacterString containing a substring of this one.
        /// </summary>
        /// <param name="startIndex">The zero-based starting character position of the
        /// substring in this instance</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <returns>A new CharacterString containing the substring.</returns>
        public CharacterString Substring(int startIndex, int length)
        {
            CharacterString substring = new CharacterString();
            substring.AddRange(this.Where((c, index) => (index >= startIndex) && (index < startIndex + length)));

            return substring;
        }

        /// <summary>
        /// Creates a new CharacterString containing a substring of this one.
        /// </summary>
        /// <param name="startIndex">The zero-based starting character position of the
        /// substring in this instance</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <returns>A new CharacterString containing the substring.</returns>
        public CharacterString Substring(int startIndex)
        {
            return Substring(startIndex, Count - startIndex);
        }

        /// <summary>
        /// Removes all leading and trailing whitespace from this CharacterString. This
        /// CharacterString is not affected.
        /// </summary>
        /// <returns>The trimmed CharacterString.</returns>
        public CharacterString Trim()
        {
            CharacterString trimmed = new CharacterString(this);

            // trim from the front
            while ((trimmed.Count > 0) && (trimmed[0].IsWhitespace))
            {
                trimmed.RemoveAt(0);
            }

            // trim from the end
            while ((trimmed.Count > 0) && (trimmed[trimmed.Count - 1].IsWhitespace))
            {
                trimmed.RemoveAt(trimmed.Count - 1);
            }

            return trimmed;
        }
    }
}
