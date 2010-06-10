using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Detects all of the doors and stairs in the level.
    /// </summary>
    public class DetectFeaturesAction : Action
    {
        /// <summary>
        /// Initializes a new DetectFeaturesAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> doing the detecting.</param>
        public DetectFeaturesAction(Entity entity)
            : base(entity)
        {
        }

        protected override ActionResult OnProcess()
        {
            foreach (Vec pos in Dungeon.Bounds)
            {
                switch (Dungeon.Tiles[pos].Type)
                {
                    case TileType.DoorClosed:
                    case TileType.DoorOpen:
                    case TileType.StairsDown:
                    case TileType.StairsUp:
                    case TileType.TownPortal:
                        Dungeon.SetTileExplored(pos);
                        break;
                }
            }

            Log(LogType.Message, "{subject} sense[s] the details of the dungeon.");

            return ActionResult.Done;
        }
    }
}