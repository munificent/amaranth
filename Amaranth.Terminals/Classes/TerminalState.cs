using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class TerminalState : ITerminalState
    {
        public Vec Cursor
        {
            get { return mCursor; }
            set { mCursor = value; }
        }

        public Color ForeColor
        {
            get { return mForeColor; }
            set { mForeColor = value; }
        }

        public Color BackColor
        {
            get { return mBackColor; }
            set { mBackColor = value; }
        }

        public TerminalState(ITerminalState cloneFrom)
        {
            mCursor = cloneFrom.Cursor;
            mForeColor = cloneFrom.ForeColor;
            mBackColor = cloneFrom.BackColor;
        }

        public TerminalState(Vec cursor, Color foreColor, Color backColor)
        {
            mCursor = cursor;
            mForeColor = foreColor;
            mBackColor = backColor;
        }

        public TerminalState(Color foreColor, Color backColor)
            : this(new Vec(0, 0), foreColor, backColor)
        {
        }

        public TerminalState()
            : this(new Vec(0, 0), TerminalColors.White, TerminalColors.Black)
        {
        }

        private Vec mCursor;
        private Color mForeColor;
        private Color mBackColor;
    }
}
