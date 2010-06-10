using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class CreatePortalAction : Action
    {
        /// <summary>
        /// Initializes a new CreatePortalAction.
        /// </summary>
        public CreatePortalAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            Vec pos = Entity.Position;

            if (!Dungeon.TryFindOpenTileWithin(Entity.Position, 4, 12, out pos))
            {
                return Fail("{subject} fails to create the portal!");
            }

            Dungeon.SetTileType(pos, TileType.TownPortal);

            if (Game.Depth > 0)
            {
                Log(LogType.Message, "{subject} create[s] a portal back to the town.");
            }
            else
            {
                Log(LogType.Message, "{subject} create[s] a portal back to the deepest dungeon.");
            }

            return ActionResult.Done;
        }
    }
}
