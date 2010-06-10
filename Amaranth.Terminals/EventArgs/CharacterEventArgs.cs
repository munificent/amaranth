using System;
using System.Collections.Generic;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class CharacterEventArgs : EventArgs
    {
        public Vec Position { get { return mPos; } }
        public Character Character { get { return mCharacter; } }
        public int X { get { return mPos.X; } }
        public int Y { get { return mPos.Y; } }

        public CharacterEventArgs(Character character, Vec pos)
        {
            mCharacter = character;
            mPos = pos;
        }

        private Character mCharacter;
        private Vec mPos;
    }
}
