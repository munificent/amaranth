using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public class Terminal : TerminalBase, ITerminal
    {
        public override Vec Size { get { return mCharacters.Size; } }

        public Terminal(int width, int height)
        {
            mCharacters = new Array2D<Character>(width, height);

            // fill with empty characters since default Character constructor doesn't initialize colors
            mCharacters.SetAll((pos) => new Character(' '));

            mStates = new Stack<ITerminalState>();

            // push a default state on
            mStates.Push(new TerminalState());
        }

        #region TerminalState methods

        public override ITerminalState State { get { return mStates.Peek(); } }

        public override void PushState(ITerminalState state)
        {
            mStates.Push(state);
        }

        public override void PushState()
        {
            PushState(new TerminalState());
        }

        public override void PopState()
        {
            if (mStates.Count <= 1) throw new InvalidOperationException("Cannot pop more states than were pushed.");

            mStates.Pop();
        }

        #endregion

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

        protected override ITerminalState GetState() { return State; }

        private readonly Array2D<Character> mCharacters;
        private readonly Stack<ITerminalState> mStates;
    }
}
