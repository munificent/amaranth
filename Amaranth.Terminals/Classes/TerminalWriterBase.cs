using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public abstract class TerminalWriterBase : IWriterPosColor
    {
        public TerminalWriterBase(ITerminalState state)
        {
            mState = state;
        }

        public ITerminalState State { get { return mState; } }

        #region IWriterPosColor Members

        public IWriterColor this[Vec pos]
        {
            get { return this[pos, Vec.One]; }
        }

        public IWriterColor this[int x, int y]
        {
            get { return this[new Vec(x, y)]; }
        }

        public IWriterColor this[Rect rect]
        {
            get { return this[rect.Position, rect.Size]; }
        }

        public IWriterColor this[Vec pos, Vec size]
        {
            get { return new TerminalWriter(GetTerminal(), size, new TerminalState(pos, mState.ForeColor, mState.BackColor)); }
        }

        public IWriterColor this[int x, int y, int width, int height]
        {
            get { return this[new Vec(x, y), new Vec(width, height)]; }
        }

        #endregion

        #region IWriterColor Members

        public IWriter this[Color foreColor, Color backColor]
        {
            get { return new TerminalWriter(GetTerminal(), GetSize(), new TerminalState(mState.Cursor, foreColor, backColor)); }
        }

        public IWriter this[ColorPair color]
        {
            get { return new TerminalWriter(GetTerminal(), GetSize(), new TerminalState(mState.Cursor, color.Fore, color.Back)); }
        }

        public IWriter this[Color foreColor]
        {
            get { return this[foreColor, mState.BackColor]; }
        }

        #endregion

        #region IWriter Members

        public void Write(char ascii)
        {
            GetTerminal().Write(mState.Cursor, new Character(ascii, mState.ForeColor, mState.BackColor));
        }

        public void Write(Glyph glyph)
        {
            GetTerminal().Write(mState.Cursor, new Character(glyph, mState.ForeColor, mState.BackColor));
        }

        public void Write(Character character)
        {
            GetTerminal().Write(mState.Cursor, character);
        }

        public void Write(string text)
        {
            GetTerminal().Write(new CharacterString(text, mState.ForeColor, mState.BackColor), mState);
        }

        public void Write(CharacterString text)
        {
            GetTerminal().Write(text, mState);
        }

        public void Scroll(Vec offset, Func<Vec, Character> scrollOnCallback)
        {
            GetTerminal().Scroll(mState.Cursor, GetSize(), offset, scrollOnCallback);
        }

        public void Scroll(int x, int y, Func<Vec, Character> scrollOnCallback)
        {
            Scroll(new Vec(x, y), scrollOnCallback);
        }

        public void Clear()
        {
            Fill(Glyph.Space);
        }

        public void Fill(Glyph glyph)
        {
            GetTerminal().Fill(mState.Cursor, GetSize(), glyph, mState.ForeColor, mState.BackColor);
        }

        public void DrawBox(bool isDouble, bool isContinue)
        {
            GetTerminal().DrawBox(mState.Cursor, GetSize(), mState.ForeColor, mState.BackColor, isDouble, isContinue);
        }

        #endregion

        protected abstract TerminalBase GetTerminal();
        protected abstract Vec GetSize();

        private ITerminalState mState;
    }
}
