using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// One building in the <see cref="Town"/>.
    /// </summary>
    [Serializable]
    public class Building
    {
        public Store Store { get { return mStore; } }

        public Rect Bounds { get { return mBounds; } }
        public Vec Door { get { return mDoor; } }

        public TileType DoorType { get { return mType; } }

        public Building(Store store, TileType type, Rect bounds, Vec door)
        {
            mStore = store;
            mType = type;
            mBounds = bounds;
            mDoor = door;
        }

        private Store mStore;
        private TileType mType;
        private Rect mBounds;
        private Vec mDoor;
    }
}
