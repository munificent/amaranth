using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Detects all of the Items in the level.
    /// </summary>
    public class DetectItemsAction : Action
    {
        /// <summary>
        /// Initializes a new DetectItemsAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> doing the detecting.</param>
        public DetectItemsAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            foreach (Item item in Dungeon.Items)
            {
                Dungeon.SetTileExplored(item.Position);
            }

            Log(LogType.Message, "{subject} sense[s] the treasures of the dungeon.");
            return ActionResult.Done;
        }
    }
}