using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public abstract class TerminalBase : TerminalWriterBase, ITerminal
    {
        public event EventHandler<CharacterEventArgs> CharacterChanged;

        public abstract Vec Size { get; }

        public int Width { get { return Size.X; } }

        public int Height { get { return Size.Y; } }

        public TerminalBase()
        {
        }

        public Character Get(Vec pos)
        {
            pos = FlipNegativePosition(pos);

            return GetValue(pos);
        }

        public Character Get(int x, int y)
        {
            return Get(new Vec(x, y));
        }

        /*
        public void Set(Vec pos, Character value)
        {
            pos = FlipNegativePosition(pos);

            if (!GetValue(pos).Equals(value))
            {
                SetValue(pos, value);

                if (CharacterChanged != null) CharacterChanged(this, new CharacterEventArgs(value, pos));
            }
        }
        */

        public void Set(Vec pos, Character value)
        {
            pos = FlipNegativePosition(pos);

            SetInternal(pos, value);
        }

        internal bool SetInternal(Vec pos, Character value)
        {
            if (SetValue(pos, value))
            {
                if (CharacterChanged != null) CharacterChanged(this, new CharacterEventArgs(value, pos));
                return true;
            }

            return false;
        }

        public void Set(int x, int y, Character value)
        {
            Set(new Vec(x, y), value);
        }

        public ITerminal CreateWindow(Rect bounds)
        {
            return new WindowTerminal(this, bounds);
        }

        public void Write(Vec pos, Character character)
        {
            Set(pos, character);
        }

        public void Write(CharacterString text, ITerminalState state)
        {
            Vec pos = state.Cursor;

            CheckBounds(pos.X, pos.Y);

            foreach (Character c in text)
            {
                Write(pos, c);
                pos += new Vec(1, 0);

                // don't run past edge
                if (pos.X >= Width) break;
            }
        }

        public void Scroll(Vec pos, Vec size, Vec offset, Func<Vec, Character> scrollOnCallback)
        {
            CheckBounds(pos, size);

            int xStart = pos.X;
            int xEnd = pos.X + size.X;
            int xStep = 1;

            int yStart = pos.Y;
            int yEnd = pos.Y + size.Y;
            int yStep = 1;

            if (offset.X > 0)
            {
                xStep = -1;

                Math2.Swap(ref xStart, ref xEnd);

                // shift the bounds back. the loops below
                // are half-inclusive, so when we flip the
                // range, we need to make it inclusive on the
                // other end of the range.
                // in other words, if the bounds are 10-50,
                // when we flip, we need to iterate from 9-49
                xStart--;
                xEnd--;
            }

            if (offset.Y > 0)
            {
                yStep = -1;

                Math2.Swap(ref yStart, ref yEnd);

                // shift the bounds back. the loops below
                // are half-inclusive, so when we flip the
                // range, we need to make it inclusive on the
                // other end of the range.
                // in other words, if the bounds are 10-50,
                // when we flip, we need to iterate from 9-49
                yStart--;
                yEnd--;
            }

            Rect bounds = new Rect(pos, size);

            for (int y = yStart; y != yEnd; y += yStep)
            {
                for (int x = xStart; x != xEnd; x += xStep)
                {
                    Vec to = new Vec(x, y);
                    Vec from = to - offset;

                    if (bounds.Contains(from))
                    {
                        // can be scrolled from
                        Set(to, Get(from));
                    }
                    else
                    {
                        // nothing to scroll onto this char, so clear it
                        Set(to, scrollOnCallback(to)); /* new Character(Glyph.Space)); */
                    }
                }
            }
        }

        public void Fill(Vec pos, Vec size, Glyph glyph, Color foreColor, Color backColor)
        {
            CheckBounds(pos, size);

            Character character = new Character(glyph, foreColor, backColor);
            foreach (Vec iter in new Rect(pos, size))
            {
                Write(iter, character);
            }
        }

        public void DrawBox(Vec pos, Vec size, Color foreColor, Color backColor, bool isDouble, bool isContinue)
        {
            CheckBounds(pos, size);

            if (size.X == 1)
            {
                DrawVerticalLine(pos, size.Y, foreColor, backColor, isDouble, isContinue);
            }
            else if (size.Y == 1)
            {
                DrawHorizontalLine(pos, size.X, foreColor, backColor, isDouble, isContinue);
            }
            else
            {
                // figure out which glyphs to use
                Glyph topLeft;
                Glyph topRight;
                Glyph bottomLeft;
                Glyph bottomRight;
                Glyph horizontal;
                Glyph vertical;

                if (isDouble)
                {
                    topLeft = Glyph.BarDoubleDownRight;
                    topRight = Glyph.BarDoubleDownLeft;
                    bottomLeft = Glyph.BarDoubleUpRight;
                    bottomRight = Glyph.BarDoubleUpLeft;
                    horizontal = Glyph.BarDoubleLeftRight;
                    vertical = Glyph.BarDoubleUpDown;
                }
                else
                {
                    topLeft = Glyph.BarDownRight;
                    topRight = Glyph.BarDownLeft;
                    bottomLeft = Glyph.BarUpRight;
                    bottomRight = Glyph.BarUpLeft;
                    horizontal = Glyph.BarLeftRight;
                    vertical = Glyph.BarUpDown;
                }

                // top left corner
                WriteLineChar(pos, topLeft, foreColor, backColor);

                // top right corner
                WriteLineChar(pos.OffsetX(size.X - 1), topRight, foreColor, backColor);

                // bottom left corner
                WriteLineChar(pos.OffsetY(size.Y - 1), bottomLeft, foreColor, backColor);

                // bottom right corner
                WriteLineChar(pos + size - 1, bottomRight, foreColor, backColor);

                // top and bottom edges
                foreach (Vec iter in Rect.Row(pos.X + 1, pos.Y, size.X - 2))
                {
                    WriteLineChar(iter, horizontal, foreColor, backColor);
                    WriteLineChar(iter.OffsetY(size.Y - 1), horizontal, foreColor, backColor);
                }

                // left and right edges
                foreach (Vec iter in Rect.Column(pos.X, pos.Y + 1, size.Y - 2))
                {
                    WriteLineChar(iter, vertical, foreColor, backColor);
                    WriteLineChar(iter.OffsetX(size.X - 1), vertical, foreColor, backColor);
                }
            }
        }

        #region ITerminal Members

        public abstract ITerminalState State { get; }

        public abstract void PushState(ITerminalState state);

        public abstract void PushState();

        public abstract void PopState();

        #endregion

        protected abstract Character GetValue(Vec pos);
        protected abstract bool SetValue(Vec pos, Character value);

        protected override TerminalBase GetTerminal() { return this; }
        protected override Vec GetSize() { return Size; }

        private Vec FlipNegativePosition(Vec pos)
        {
            // negative coordinates mean from the right/bottom edge
            if (pos.X < 0) pos.X = Width + pos.X;
            if (pos.Y < 0) pos.Y = Height + pos.Y;

            return pos;
        }

        private void DrawHorizontalLine(Vec pos, int length, Color foreColor, Color backColor, bool isDouble, bool isContinue)
        {
            // figure out which glyphs to use
            Glyph left = Glyph.BarRight;
            Glyph middle = Glyph.BarLeftRight;
            Glyph right = Glyph.BarLeft;

            if (isDouble)
            {
                middle = Glyph.BarDoubleLeftRight;

                if (isContinue)
                {
                    left = Glyph.BarDoubleLeftRight;
                    right = Glyph.BarDoubleLeftRight;
                }
                else
                {
                    left = Glyph.BarDoubleRight;
                    right = Glyph.BarDoubleLeft;
                }
            }
            else
            {
                if (isContinue)
                {
                    left = Glyph.BarLeftRight;
                    right = Glyph.BarLeftRight;
                }
            }

            // left edge
            WriteLineChar(pos, left, foreColor, backColor);

            // right edge
            WriteLineChar(pos.OffsetX(length - 1), right, foreColor, backColor);

            // middle
            foreach (Vec iter in Rect.Row(pos.X + 1, pos.Y, length - 2))
            {
                WriteLineChar(iter, middle, foreColor, backColor);
            }
        }

        private void DrawVerticalLine(Vec pos, int length, Color foreColor, Color backColor, bool isDouble, bool isContinue)
        {
            // figure out which glyphs to use
            Glyph top = Glyph.BarDown;
            Glyph middle = Glyph.BarUpDown;
            Glyph bottom = Glyph.BarUp;

            if (isDouble)
            {
                middle = Glyph.BarDoubleUpDown;

                if (isContinue)
                {
                    top = Glyph.BarDoubleUpDown;
                    bottom = Glyph.BarDoubleUpDown;
                }
                else
                {
                    top = Glyph.BarDoubleDown;
                    bottom = Glyph.BarDoubleUp;
                }
            }
            else
            {
                if (isContinue)
                {
                    top = Glyph.BarUpDown;
                    bottom = Glyph.BarUpDown;
                }
            }

            // top edge
            WriteLineChar(pos, top, foreColor, backColor);

            // bottom edge
            WriteLineChar(pos.OffsetY(length - 1), bottom, foreColor, backColor);

            // middle
            foreach (Vec iter in Rect.Column(pos.X, pos.Y + 1, length - 2))
            {
                WriteLineChar(iter, middle, foreColor, backColor);
            }
        }

        private void WriteLineChar(Vec pos, Glyph glyph, Color foreColor, Color backColor)
        {
            this[pos][foreColor, backColor].Write(glyph);
        }

        private void CheckBounds(int x, int y)
        {
            if (x >= Width) throw new ArgumentOutOfRangeException("x");
            if (y >= Height) throw new ArgumentOutOfRangeException("y");

            // negative values are valid and mean "from the right or bottom", so apply and check range
            if ((x < 0) && (Width + x >= Width)) throw new ArgumentOutOfRangeException("x");
            if ((y < 0) && (Height + y >= Height)) throw new ArgumentOutOfRangeException("y");
        }

        private void CheckBounds(int x, int y, int width, int height)
        {
            //### bob: need to handle negative coords
            if (x < 0) throw new ArgumentOutOfRangeException("x");
            if (x >= Width) throw new ArgumentOutOfRangeException("x");
            if (y < 0) throw new ArgumentOutOfRangeException("y");
            if (y >= Height) throw new ArgumentOutOfRangeException("y");
            if (width <= 0) throw new ArgumentException("width");
            if (x + width > Width) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentException("height");
            if (y + height > Height) throw new ArgumentOutOfRangeException("height");
        }

        private void CheckBounds(Vec pos, Vec size)
        {
            CheckBounds(pos.X, pos.Y, size.X, size.Y);
        }
    }
}
