using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    //### bob: CheckForCancel stuff here feels hackish
    /// <summary>
    /// Basic Action for walking (or crawling or whatever) a single step in one direction. Also used to wait a turn,
    /// by specifying no direction.
    /// </summary>
    public class WalkAction : Action
    {
        public WalkAction(Entity entity, Direction direction, bool checkForCancel)
            : base(entity)
        {
            mDirection = direction;
            mCheckForCancel = checkForCancel;
        }

        public WalkAction(Entity entity, Direction direction)
            : this(entity, direction, false)
        {
        }

        protected override ActionResult OnProcess()
        {
            Vec newPos = Entity.Position + mDirection;
            TileType tileType = Dungeon.Tiles[newPos].Type;

            // if walking into a closed door, open it
            if (tileType == TileType.DoorClosed) return new OpenDoorAction(Entity, newPos);

            // if there is something in the tile we are moving to, attack it
            Entity occupier = Dungeon.Entities.GetAt(newPos);
            if ((occupier != null) && (occupier != Entity))
            {
                return new AttackAction(Entity, occupier);
            }

            // fail if the tile is not occupiable
            if (!Entity.CanMove(mDirection))
            {
                // not occupyable tile
                if (Entity is Hero)
                {
                    // you know it's a wall now
                    Dungeon.SetTileExplored(newPos);
                }

                return Fail("{subject} can't walk there.");
            }

            // move the entity
            Entity.Position = newPos;

            // enter a store
            switch (tileType)
            {
                case TileType.DoorStore1:
                case TileType.DoorStore2:
                case TileType.DoorStore3:
                case TileType.DoorStore4:
                case TileType.DoorStore5:
                case TileType.DoorStore6:
                    Hero hero = Entity as Hero;
                    if (hero != null)
                    {
                        AddAction(new EnterStoreAction(hero, tileType));
                    }
                    break;
            }

            if (mCheckForCancel)
            {
                return ActionResult.CheckForCancel;
            }
            else
            {
                return ActionResult.Done;
            }
        }

        private Direction mDirection;
        private bool mCheckForCancel;
    }
}