using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Action class for using an <see cref="Item"/> with an <see cref="ChargeType"/> of <see cref="ChargeType.Multi"/>.
    /// </summary>
    public class UseMultiAction : ItemAction
    {
        public UseMultiAction(Entity entity, Item item, Vec? target)
            : base(entity, item)
        {
            mTarget = target;
        }

        protected override ActionResult OnProcess()
        {
            // make sure it has charges
            if (Item.Charges > 0)
            {
                Log(LogType.Message, "{subject} use[s] {object}.", Item);

                // use the item if it actually has a use
                if (Item.Type.Use != null)
                {
                    Item.Type.Use.Invoke(Entity, Item, this, mTarget);
                }

                // and use a charge
                Item.Charges--;

                return ActionResult.Done;
            }
            else
            {
                return Fail(Item, "{subject} has no charges left.");
            }
        }

        private Vec? mTarget;
    }
}
