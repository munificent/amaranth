using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class TakeStairsAction : Action
    {
        public TakeStairsAction(Hero hero)
            : base(hero)
        {
        }

        protected override ActionResult OnProcess()
        {
            // see what kind of stair we're on
            TileType tileType = Dungeon.Tiles[Entity.Position].Type;

            switch (tileType)
            {
                case TileType.StairsUp:
                    Log(LogType.Message, "{subject} ascend[s] closer to the light.");
                    Game.ChangeFloor(-1);
                    return ActionResult.Done;

                case TileType.StairsDown:
                    Log(LogType.Message, "{subject} descend[s] deeper into the dungeon.");
                    Game.ChangeFloor(1);
                    return ActionResult.Done;

                case TileType.TownPortal:
                    Log(LogType.Message, "{subject} step through as the portal closes behind {pronoun}...");
                    if (Game.Depth > 0)
                    {
                        // portal back to town
                        Game.SetFloor(0);
                    }
                    else
                    {
                        // portal back to the dungeon
                        Game.SetFloor(Game.Hero.DeepestDepth);
                    }
                    return ActionResult.Done;

                default:
                    // not on stairs
                    return ActionResult.Fail;
            }
        }
    }
}
