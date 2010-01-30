using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Simple Action that just executes a given function.
    /// </summary>
    public class DelegateAction : Action
    {
        public DelegateAction(Entity entity)
            : base(entity)
        {
        }

        protected void SetCallback(Func<ActionResult> callback) { mCallback = callback; }

        protected override ActionResult OnProcess()
        {
            return mCallback();
        }

        private Func<ActionResult> mCallback;
    }
}
