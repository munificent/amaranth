using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class Terminal : TerminalBase
    {
        public override Vec Size { get { return mCharacters.Size; } }

        public Terminal(int width, int height)
            : base()
        {
            mCharacters = new Array2D<Character>(width, height);

            // fill with empty characters since default Character constructor doesn't initialize colors
            mCharacters.Fill((pos) => new Character(' '));
        }

        protected override Character GetValue(Vec pos)
        {
            return mCharacters[pos];
        }

        protected override bool SetValue(Vec pos, Character value)
        {
            // don't do anything if the value doesn't change
            if (mCharacters[pos].Equals(value)) return false;

            mCharacters[pos] = value;
            return true;
        }

        private readonly Array2D<Character> mCharacters;
    }
}
