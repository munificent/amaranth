using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class WindowTerminal : TerminalBase, ITerminal
    {
        public WindowTerminal(TerminalBase parent, Rect bounds)
        {
            mParent = parent;
            mBounds = bounds;
        }

        public override Vec Size { get { return mBounds.Size; } }

        protected override Character GetValue(Vec pos)
        {
            return mParent.Get(pos + mBounds.Position);
        }

        /*
        protected override bool SetValue(Vec pos, Character value)
        {
            return mParent.Set(pos + mBounds.Position, value);
        }
        */

        protected override bool SetValue(Vec pos, Character value)
        {
            return mParent.SetInternal(pos + mBounds.Position, value);
        }

        protected override ITerminalState GetState()
        {
            return mParent.State;
        }

        #region TerminalState methods

        public override ITerminalState State { get { return mParent.State; } }

        public override void PushState(ITerminalState state)
        {
            mParent.PushState(state);
        }

        public override void PushState()
        {
            mParent.PushState();
        }

        public override void PopState()
        {
            mParent.PopState();
        }

        #endregion

        private TerminalBase mParent;
        private Rect mBounds;
    }
}
