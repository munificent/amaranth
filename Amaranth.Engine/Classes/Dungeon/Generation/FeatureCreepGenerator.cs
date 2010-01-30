using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class FeatureCreepGenerator : IDungeonGenerator, IFeatureWriter
    {
        #region IDungeonGenerator Members

        public void Create(Dungeon dungeon, bool isDescending, int depth, object optionsObj)
        {
            mOptions = (FeatureCreepGeneratorOptions)optionsObj;

            mDungeon = dungeon;

            // sometimes the generator makes dud dungeons with just one or two rooms, keep
            // trying from scratch until we get one with at least a certain amount of
            // carved open area.
            do
            {
                mTry++;

                dungeon.Entities.Clear();
                dungeon.Items.Clear();

                MakeDungeon(dungeon.Bounds.Size, depth);
            }
            while ((100 * mOpenCount / dungeon.Bounds.Area < mOptions.MinimumOpenPercent)
                  || !mMadeDownStair || !mMadeUpStair);
        }

        #endregion

        private void MakeDungeon(Vec size, int depth)
        {
            // clear the grid
            mOpenCount = 0;
            mMadeUpStair = false;
            mMadeDownStair = false;
            mDungeon.Tiles.SetAll(pos => new Tile(TileType.Wall));

            // create a starting room
            mUnusedConnectors.Clear();
            mStartPos = FeatureFactory.MakeStartingRoom(this, depth).Center;

            for (int tries = 0; (tries < mOptions.MaxTries) && (mUnusedConnectors.Count > 0); tries++)
            {
                // pull off the first unused connector
                Connector connector = mUnusedConnectors[0];

                bool success = false;

                switch (connector.From)
                {
                    case ConnectFrom.Room:
                        // try to add a hall
                        success = FeatureFactory.MakeHall(this, connector, depth);
                        break;

                    case ConnectFrom.Hall:
                        var create = mDungeon.Game.Content.Features.CreateOne(depth);
                        success = create(this, connector, depth);
                        break;
                }

                // the connector has been tried
                mUnusedConnectors.Remove(connector);

                // if we failed to connect something, move the connector to the end of the list
                // since it pulls connectors from the beginning, this should encourage
                // the dungeon to spread out first instead of just hammering the same crowded
                // connectors over and over again.
                if (!success)
                {
                    mUnusedConnectors.Add(connector);
                }
            }
        }

        #region IFeatureWriter Members

        Rect IFeatureWriter.Bounds { get { return mDungeon.Bounds; } }

        Content IFeatureWriter.Content { get { return mDungeon.Game.Content; } }

        FeatureCreepGeneratorOptions IFeatureWriter.Options { get { return mOptions; } }

        /// <summary>
        /// Gets whether the given rectangle is empty (i.e. solid walls) and can have a feature placed in it.
        /// </summary>
        /// <param name="rect">The rectangle to test.</param>
        /// <param name="exception">An optional exception position. If this position is not a wall, it's still
        /// possible to use the rect. Used for the connector to a new feature.</param>
        /// <remarks>It tests this by simply seeing if the rect contains the dungeon starting position, or if
        /// the outer edge of the rect touches a non-wall square. As long as the dungeon is always connected
        /// to the starting position, this should be enough to tell if any square inside the rect is in use.</remarks>
        bool IFeatureWriter.IsOpen(Rect rect, Vec? exception)
        {
            // must be totally in bounds
            if (!mDungeon.Bounds.Contains(rect)) return false;

            // and not cover the starting grid
            if (rect.Contains(mStartPos)) return false;

            // or something connected to it
            foreach (Vec edge in rect.Trace())
            {
                // allow the exception
                if (exception.HasValue && (exception.Value == edge)) continue;

                if (mDungeon.Tiles[edge].Type != TileType.Wall) return false;
            }

            return true;
        }

        TileType IFeatureWriter.GetTile(Vec pos)
        {
            return mDungeon.Tiles[pos].Type;
        }

        void IFeatureWriter.SetTile(Vec pos, TileType type)
        {
            mDungeon.Tiles[pos].Type = type;

            // keep track of how much dungeon we've carved
            if (mDungeon.Tiles[pos].IsPassable) mOpenCount++;

            //### bob: hackish. assumes will never get overwritten after
            if (type == TileType.StairsUp) mMadeUpStair = true;
            if (type == TileType.StairsDown) mMadeDownStair = true;
        }

        void IFeatureWriter.LightRect(Rect bounds, int depth)
        {
            // light the room
            if ((depth <= Rng.Int(1, 80)))
            {
                foreach (Vec pos in bounds.Inflate(1))
                {
                    mDungeon.SetTilePermanentLit(pos, true);
                }
            }
        }

        void IFeatureWriter.AddRoomConnector(Vec pos, Direction dir)
        {
            mUnusedConnectors.Insert(Rng.Int(mUnusedConnectors.Count), new Connector(ConnectFrom.Room, dir, pos));
        }

        void IFeatureWriter.AddHallConnector(Vec pos, Direction dir)
        {
            mUnusedConnectors.Insert(Rng.Int(mUnusedConnectors.Count), new Connector(ConnectFrom.Hall, dir, pos));
        }

        void IFeatureWriter.Populate(Vec pos, int monsterDensity, int itemDensity, int depth)
        {
            // test every open tile
            if (mDungeon.Tiles[pos].IsPassable)
            {
                // place a monster
                if ((mDungeon.Entities.GetAt(pos) == null) && (Rng.Int(1000) < monsterDensity + (depth / 4)))
                {
                    Monster.AddRandom(mDungeon, depth, pos);
                }

                // place an item
                if (Rng.Int(1000) < itemDensity + (depth / 4))
                {
                    Race race = Race.Random(mDungeon, depth, false);
                    race.PlaceDrop(mDungeon, pos);
                }
            }
        }

        void IFeatureWriter.AddEntity(Entity entity)
        {
            mDungeon.Entities.Add(entity);
        }

        #endregion

        private FeatureCreepGeneratorOptions mOptions;
        private readonly List<Connector> mUnusedConnectors = new List<Connector>();
        private int mTry = 0;
        private int mOpenCount;
        private Dungeon mDungeon;
        private Vec mStartPos;
        bool mMadeUpStair;
        bool mMadeDownStair;
    }

    public enum ConnectFrom
    {
        Room,
        Hall
    }

    public class Connector
    {
        public Direction Direction;
        public Vec Position;
        public ConnectFrom From;

        public Connector(ConnectFrom from, Direction direction, Vec position)
        {
            From = from;
            Direction = direction;
            Position = position;
        }
    }
}
