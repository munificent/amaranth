using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    class UseLightAction : ItemAction
    {
        public UseLightAction(Entity entity, Item item)
            : base(entity, item)
        {
        }

        protected override ActionResult OnProcess()
        {
            if (Item.Charges == 0)
            {
                return Fail(Item, "{subject} is used up.");
            }
            else if (Item.Charges < 0)
            {
                // not lit, light it
                Log(LogType.Message, "{subject} light[s] {object}.", Item);
                Dungeon.DirtyLighting();

                Item.Charges = -Item.Charges;
            }
            else
            {
                // lit, unlight it
                Log(LogType.Message, "{subject} extinguish[es] {object}.", Item);
                Dungeon.DirtyLighting();

                Item.Charges = -Item.Charges;
            }

            return ActionResult.Done;
        }
    }
}
