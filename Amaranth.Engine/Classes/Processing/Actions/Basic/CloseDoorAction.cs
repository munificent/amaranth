using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class CloseDoorAction : Action
    {
        public CloseDoorAction(Entity entity, Vec doorPos)
            : base(entity)
        {
            mDoorPos = doorPos;
        }

        protected override ActionResult OnProcess()
        {
            switch (Dungeon.Tiles[mDoorPos].Type)
            {
                case TileType.DoorOpen:
                    // close it
                    Dungeon.SetTileType(mDoorPos, TileType.DoorClosed);
                    Dungeon.DirtyLighting();
                    Dungeon.DirtyVisibility();

                    Log(LogType.Message, "{subject} close[s] the door.");
                    return ActionResult.Done;

                case TileType.DoorClosed:
                    return Fail("The door is already closed.");

                case TileType.DoorStore1:
                case TileType.DoorStore2:
                case TileType.DoorStore3:
                case TileType.DoorStore4:
                case TileType.DoorStore5:
                case TileType.DoorStore6:
                    return Fail("The door cannot be closed.");

                default:
                    return Fail("There is no door there.");
            }
        }

        private Vec mDoorPos;
    }
}
