using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// One square of a Dungeon.
    /// </summary>
    [Serializable]
    public class Tile : IDungeonTile
    {
        /// <summary>
        /// Gets the main feature of this Tile.
        /// </summary>
        public TileType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        /// <summary>
        /// Gets whether this Tile is currently visible to the <see cref="Hero"/>. Only
        /// implies an open line-of-sight, not that the Tile is lit.
        /// </summary>
        public bool IsVisible { get { return mIsVisible; } }

        /// <summary>
        /// Gets whether this Tile has been explored.
        /// </summary>
        public bool IsExplored { get { return mIsExplored; } }

        /// <summary>
        /// Gets whether this Tile is currently lit due to a nearby <see cref="Thing"/>'s
        /// light source.
        /// </summary>
        public bool IsLitByThing { get { return mIsLitByThing; } }

        /// <summary>
        /// Gets whether this Tile is itself permanently illuminated.
        /// </summary>
        public bool IsSelfLit { get { return mIsSelfLit; } }

        /// <summary>
        /// Gets whether this Tile is currently lit by anything.
        /// </summary>
        public bool IsLit { get { return IsLitByThing || IsSelfLit; } }

        public bool IsPassable
        {
            get { return mType.IsPassable(); }
        }

        public bool IsTransparent
        {
            get { return mType.IsTransparent(); }
        }

        public Tile(TileType type, bool isSelfLit)
        {
            mType = type;
            mIsSelfLit = IsSelfLit;
        }

        public Tile(TileType type)
            : this(type, false)
        {
        }

        /// <summary>
        /// Marks this Tile as permanently self-lit.
        /// </summary>
        public void Light()
        {
            mIsSelfLit = true;
        }

        /// <summary>
        /// Marks this Tile as permanently self-lit and explored
        /// (i.e. for the town).
        /// </summary>
        public void LightKnown()
        {
            mIsSelfLit = true;
            mIsExplored = true;
        }

        #region IDungeonTile Members

        bool IDungeonTile.SetTileType(TileType type)
        {
            if (mType != type)
            {
                mType = type;

                // type changed
                return true;
            }
            else
            {
                // no change
                return false;
            }
        }

        bool IDungeonTile.SetIsVisible(bool isVisible)
        {
            if (mIsVisible != isVisible)
            {
                mIsVisible = isVisible;

                // visibility changed
                return true;
            }
            else
            {
                // no change
                return false;
            }
        }

        bool IDungeonTile.SetExplored()
        {
            if (!mIsExplored)
            {
                mIsExplored = true;

                // changed
                return true;
            }
            else
            {
                // no change
                return false;
            }
        }

        bool IDungeonTile.SetTileThingLit(bool isLit)
        {
            if (mIsLitByThing != isLit)
            {
                mIsLitByThing = isLit;

                // lighting changed
                return true;
            }
            else
            {
                // no change
                return false;
            }
        }

        bool IDungeonTile.SetTilePermanentLit(bool isLit)
        {
            if (mIsSelfLit != isLit)
            {
                mIsSelfLit = isLit;

                // lighting changed
                return true;
            }
            else
            {
                // no change
                return false;
            }
        }

        #endregion

        private TileType mType;

        private bool mIsVisible;
        private bool mIsLitByThing;
        private bool mIsSelfLit;
        private bool mIsExplored;
    }

    public enum TileType
    {
        Floor,
        Wall,

        LowWall,

        StairsDown,
        StairsUp,

        DoorOpen,
        DoorClosed,

        TownPortal,

        // used in the town
        RoofLight,
        RoofDark,
        DoorStore1,
        DoorStore2,
        DoorStore3,
        DoorStore4,
        DoorStore5,
        DoorStore6
    }

    public static class TileTypeExtensions
    {
        public static bool IsPassable(this TileType type)
        {
            return (type != TileType.Wall) &&
                   (type != TileType.DoorClosed) &&
                   (type != TileType.RoofDark) &&
                   (type != TileType.RoofLight) &&
                   (type != TileType.LowWall);
        }

        public static bool IsTransparent(this TileType type)
        {
            return (type != TileType.Wall) &&
                   (type != TileType.DoorClosed) &&
                   (type != TileType.RoofDark) &&
                   (type != TileType.RoofLight);
        }
    }

    /// <summary>
    /// Interface to communicate with a Tile only used by Dungeon.
    /// </summary>
    public interface IDungeonTile
    {
        bool SetTileType(TileType type);
        bool SetIsVisible(bool isVisible);
        bool SetExplored();
        bool SetTileThingLit(bool isLit);
        bool SetTilePermanentLit(bool isLit);
    }

    public class TileEventArgs : EventArgs
    {
        public Tile Tile;
        public Vec Position;

        public TileEventArgs(Tile tile, Vec position)
        {
            Tile = tile;
            Position = position;
        }
    }
}
