using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base Action class for an Action that processes in multiple steps and wants to implement that using
    /// an IEnumerable{ActionResult}.
    /// </summary>
    public abstract class EnumerableAction : Action
    {
        public EnumerableAction(NotNull<Entity> entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            if (mEnumerator == null)
            {
                mEnumerator = OnProcessEnumerable().GetEnumerator();
            }

            // done if the enumerator runs out
            if (!mEnumerator.MoveNext()) return ActionResult.Done;

            // return the next step
            return mEnumerator.Current;
        }

        protected abstract IEnumerable<ActionResult> OnProcessEnumerable();

        private IEnumerator<ActionResult> mEnumerator;
    }
}
