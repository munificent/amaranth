using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class UseAction : ItemAction
    {
        public UseAction(Hero hero, Item item, Vec? target)
            : base(hero, item)
        {
            mTarget = target;
        }

        protected override ActionResult OnProcess()
        {
            // choose an appropriate action based on usage
            switch (Item.Type.ChargeType)
            {
                case ChargeType.None: throw new InvalidOperationException("Cannot use an item with a usage type of ItemUsage.None.");
                case ChargeType.Single: return new UseSingleAction(Entity, Item, mTarget);
                case ChargeType.Light: return new UseLightAction(Entity, Item);
                case ChargeType.Multi: return new UseMultiAction(Entity, Item, mTarget);
                default: throw new UnexpectedEnumValueException(Item.Type.ChargeType);
            }
        }

        private Vec? mTarget;
    }
}
