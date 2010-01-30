using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public abstract class TerminalWriterBase : IWriterPosColor
    {
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
            get { return new TerminalWriter(GetTerminal(), size, new TerminalState(pos, GetState().ForeColor, GetState().BackColor)); }
        }

        public IWriterColor this[int x, int y, int width, int height]
        {
            get { return this[new Vec(x, y), new Vec(width, height)]; }
        }

        #endregion

        #region IWriterColor Members

        public IWriter this[Color foreColor, Color backColor]
        {
            get { return new TerminalWriter(GetTerminal(), GetSize(), new TerminalState(GetState().Cursor, foreColor, backColor)); }
        }

        public IWriter this[ColorPair color]
        {
            get { return new TerminalWriter(GetTerminal(), GetSize(), new TerminalState(GetState().Cursor, color.Fore, color.Back)); }
        }

        public IWriter this[Color foreColor]
        {
            get { return this[foreColor, GetState().BackColor]; }
        }

        #endregion

        #region IWriter Members

        public void Write(char ascii)
        {
            ITerminalState state = GetState();
            GetTerminal().Write(state.Cursor, new Character(ascii, state.ForeColor, state.BackColor));
        }

        public void Write(Glyph glyph)
        {
            ITerminalState state = GetState();
            GetTerminal().Write(state.Cursor, new Character(glyph, state.ForeColor, state.BackColor));
        }

        public void Write(Character character)
        {
            GetTerminal().Write(GetState().Cursor, character);
        }

        public void Write(string text)
        {
            ITerminalState state = GetState();
            GetTerminal().Write(new CharacterString(text, state.ForeColor, state.BackColor), state);
        }

        public void Write(CharacterString text)
        {
            GetTerminal().Write(text, GetState());
        }

        public void Scroll(Vec offset, Func<Vec, Character> scrollOnCallback)
        {
            GetTerminal().Scroll(GetState().Cursor, GetSize(), offset, scrollOnCallback);
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
            GetTerminal().Fill(GetState().Cursor, GetSize(), glyph, GetState().ForeColor, GetState().BackColor);
        }

        public void DrawBox(bool isDouble, bool isContinue)
        {
            GetTerminal().DrawBox(GetState().Cursor, GetSize(), GetState().ForeColor, GetState().BackColor, isDouble, isContinue);
        }

        #endregion

        protected abstract TerminalBase GetTerminal();
        protected abstract Vec GetSize();
        protected abstract ITerminalState GetState();
    }
}
