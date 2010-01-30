using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Basic <see cref="Behavior"/> that returns one set <see cref="Action"/> once and then waits for user input.
    /// </summary>
    [Serializable]
    public class OneShotBehavior : Behavior
    {
        public override bool NeedsUserInput { get { return mAction == null; } }

        public OneShotBehavior()
        {
        }

        public OneShotBehavior(Action action)
        {
            mAction = action;
        }

        public override Action NextAction()
        {
            // clear the action so it is only used once
            Action action = mAction;
            mAction = null;

            return action;
        }

        [NonSerialized]
        private Action mAction;
    }
}
