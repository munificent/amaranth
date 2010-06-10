using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Action class for using an <see cref="Item"/> with an <see cref="ChargeType"/> of <see cref="ChargeType.Single"/>.
    /// </summary>
    public class UseSingleAction : ItemAction
    {
        public UseSingleAction(Entity entity, Item item, Vec? target)
            : base(entity, item)
        {
            mTarget = target;
        }

        protected override ActionResult OnProcess()
        {
            // consume the item
            Item used = Item.SplitStack(1);

            // used up the stack
            if (Item.Quantity == 0)
            {
                Hero hero = (Hero)Entity;
                if (hero.Inventory.Contains(Item))
                {
                    // it's in the inventory
                    hero.Inventory.Remove(Item);
                }
                else if (hero.Equipment.Contains(Item))
                {
                    // it's in the equipment
                    hero.Equipment.Remove(Item);
                }
                else
                {
                    // it's in the dungeon
                    Dungeon.Items.Remove(Item);
                }
            }

            Log(LogType.Message, "{subject} use[s] {object}.", used);

            // use the item if it actually has a use
            if (used.Type.Use != null)
            {
                used.Type.Use.Invoke(Entity, used, this, mTarget);
            }

            return ActionResult.Done;
        }

        private Vec? mTarget;
    }
}
