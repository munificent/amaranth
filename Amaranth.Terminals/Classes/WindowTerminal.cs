using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class WindowTerminal : TerminalBase
    {
        public WindowTerminal(TerminalBase parent, Color foreColor, Color backColor, Rect bounds)
            : base(foreColor, backColor)
        {
            mParent = parent;
            mBounds = bounds;
        }

        public override Vec Size { get { return mBounds.Size; } }

        protected override Character GetValue(Vec pos)
        {
            return mParent.Get(pos + mBounds.Position);
        }

        protected override bool SetValue(Vec pos, Character value)
        {
            if (!mBounds.Size.Contains(pos)) return false;

            return mParent.SetInternal(pos + mBounds.Position, value);
        }

        private TerminalBase mParent;
        private Rect mBounds;
    }
}
