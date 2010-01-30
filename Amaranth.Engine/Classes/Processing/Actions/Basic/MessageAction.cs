using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Basic Action that just logs a message.
    /// </summary>
    public class MessageAction : Action
    {
        public MessageAction(Entity entity, string message, Entity target)
            : base(entity)
        {
            mTarget = target;
            mMessage = message;
        }

        protected override ActionResult OnProcess()
        {
            Log(LogType.Message, Entity, mMessage, mTarget);

            return ActionResult.Done;
        }

        private Entity mTarget;
        private string mMessage;
    }
}
