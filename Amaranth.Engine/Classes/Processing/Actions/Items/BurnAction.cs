using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Action class for a light source's own processing when lit.
    /// </summary>
    public class BurnAction : ItemAction
    {
        public BurnAction(Game game, Item item)
            : base(game, item)
        {
        }

        protected override ActionResult OnProcess()
        {
            Item.Charges--;

            if (Item.Charges == 200)
            {
                Log(LogType.Message, Item, "{subject} is growing faint.");
            }
            else if (Item.Charges == 100)
            {
                Log(LogType.Message, Item, "{subject} is growing guttering.");
            }
            else if (Item.Charges == 0)
            {
                Log(LogType.Message, Item, "{subject} has gone out.");
                Dungeon.DirtyLighting();
            }

            return ActionResult.Done;
        }
    }
}
