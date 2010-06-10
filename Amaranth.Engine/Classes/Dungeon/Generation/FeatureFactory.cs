using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class FeatureFactory
    {
        public FeatureFactory(IFeatureWriter writer, int depth)
        {
            mWriter = writer;
            mDepth = depth;
        }

        public Rect MakeStartingRoom()
        {
            return CreateRoom(null);
        }

        public bool CreateFeature(string name, Connector connector)
        {
            switch (name)
            {
                case "stair": return MakeStair(connector);
                case "room": return MakeRoom(connector);
                case "maze": return MakeMaze(connector);
                case "pit": return MakePit(connector);
                case "junction": return MakeJunction(connector);
                default: throw new ArgumentException("Unknown feature \"" + name + "\"/");
            }
        }

        public bool MakeHall(Connector connector)
        {
            // create a random hall
            int length = Rng.Int(mWriter.Options.HallLengthMin, mWriter.Options.HallLengthMax);

            // check to see if we can place it
            Rect bounds = Rect.Empty;
            if (connector.Direction == Direction.N) bounds = new Rect(connector.Position.X - 1, connector.Position.Y - length, 3, length + 1);
            if (connector.Direction == Direction.S) bounds = new Rect(connector.Position.X - 1, connector.Position.Y, 3, length + 1);
            if (connector.Direction == Direction.E) bounds = new Rect(connector.Position.X, connector.Position.Y - 1, length + 1, 3);
            if (connector.Direction == Direction.W) bounds = new Rect(connector.Position.X - length, connector.Position.Y - 1, length + 1, 3);

            if (!mWriter.IsOpen(bounds, null)) return false;

            // make sure the end corners aren't open unless the position in front of the end is too
            // prevents cases like:
            Vec pos = connector.Position + (connector.Direction.Offset * (length + 1));

            if (!mWriter.Bounds.Contains(pos)) return false;
            if (!mWriter.Bounds.Contains(pos + connector.Direction.RotateLeft90)) return false;
            if (!mWriter.Bounds.Contains(pos + connector.Direction.RotateRight90)) return false;
            // ####..
            // ####..
            // ....## <- new hall ends at corner of room
            // ######
            if ((mWriter.GetTile(pos + connector.Direction.RotateLeft90) != TileType.Wall) &&
                (mWriter.GetTile(pos) == TileType.Wall)) return false;

            if ((mWriter.GetTile(pos + connector.Direction.RotateRight90) != TileType.Wall) &&
                (mWriter.GetTile(pos) == TileType.Wall)) return false;

            // place the hall
            pos = connector.Position;
            for (int i = 0; i <= length; i++)
            {
                mWriter.SetTile(pos, TileType.Floor);

                pos += connector.Direction;
            }

            PlaceDoor(connector.Position);
            PlaceDoor(connector.Position + (connector.Direction.Offset * length));

            // add the connectors
            mWriter.AddHallConnector(connector.Position + (connector.Direction.Offset * length),
                                    connector.Direction);

            Populate(bounds, 10, 10, mDepth);

            return true;
        }

        private bool MakeJunction(Connector connector)
        {
            // create a random junction
            Vec center = connector.Position + connector.Direction;

            bool left = false;
            bool right = false;
            bool straight = false;

            int choice = Rng.Int(100);
            if (choice < mWriter.Options.ChanceOfTurn)
            {
                if (Rng.OneIn(2)) left = true; else right = true;
            }
            else if (choice - mWriter.Options.ChanceOfTurn < mWriter.Options.ChanceOfFork)
            {
                if (Rng.OneIn(2)) left = true; else right = true;
                straight = true;
            }
            else if (choice - mWriter.Options.ChanceOfTurn
                            - mWriter.Options.ChanceOfFork < mWriter.Options.ChanceOfTee)
            {
                left = true;
                right = true;
            }
            else if (choice - mWriter.Options.ChanceOfTurn
                            - mWriter.Options.ChanceOfFork
                            - mWriter.Options.ChanceOfTee < mWriter.Options.ChanceOfFourWay)
            {
                left = true;
                right = true;
                straight = true;
            }
            else
            {
                straight = true;
            }

            // check to see if we can place it
            Rect rect = new Rect(center.Offset(-1, -1), 3, 3);
            if (!mWriter.IsOpen(rect, center + connector.Direction.Rotate180)) return false;

            // place the junction
            mWriter.SetTile(center, TileType.Floor);

            // add the connectors
            if (left) mWriter.AddRoomConnector(center + connector.Direction.RotateLeft90, connector.Direction.RotateLeft90);
            if (right) mWriter.AddRoomConnector(center + connector.Direction.RotateRight90, connector.Direction.RotateRight90);
            if (straight) mWriter.AddRoomConnector(center + connector.Direction, connector.Direction);

            return true;
        }

        private bool MakeRoom(Connector connector)
        {
            return CreateRoom(connector) != Rect.Empty;
        }

        private bool MakeStair(Connector connector)
        {
            // check to see if we can place it
            Rect rect = new Rect(connector.Position.Offset(-1, -1), 3, 3);
            if (!mWriter.IsOpen(rect, connector.Position + connector.Direction.Rotate180)) return false;

            TileType type = (Rng.Int(10) < 6) ? TileType.StairsDown : TileType.StairsUp;
            mWriter.SetTile(connector.Position, type);

            return true;
        }

        private bool MakeMaze(Connector connector)
        {
            // in maze units (i.e. thin walls), not tiles
            int width = Rng.Int(mWriter.Options.MazeSizeMin, mWriter.Options.MazeSizeMax);
            int height = Rng.Int(mWriter.Options.MazeSizeMin, mWriter.Options.MazeSizeMax);

            int tileWidth = width * 2 + 3;
            int tileHeight = height * 2 + 3;
            Rect bounds = CreateRectRoom(connector, tileWidth, tileHeight);

            // bail if we failed
            if (bounds == Rect.Empty) return false;

            // the hallway around the maze
            foreach (Vec pos in bounds.Trace())
            {
                mWriter.SetTile(pos, TileType.Floor);
            }

            // sometimes make the walls low
            if (Rng.OneIn(2))
            {
                foreach (Vec pos in bounds.Inflate(-1))
                {
                    mWriter.SetTile(pos, TileType.LowWall);
                }
            }

            // add an opening in one corner
            Vec doorway;
            switch (Rng.Int(8))
            {
                case 0: doorway = bounds.TopLeft.Offset(2, 1); break;
                case 1: doorway = bounds.TopLeft.Offset(1, 2); break;
                case 2: doorway = bounds.TopRight.Offset(-3, 1); break;
                case 3: doorway = bounds.TopRight.Offset(-2, 2); break;
                case 4: doorway = bounds.BottomRight.Offset(-3, -2); break;
                case 5: doorway = bounds.BottomRight.Offset(-2, -3); break;
                case 6: doorway = bounds.BottomLeft.Offset(2, -2); break;
                case 7: doorway = bounds.BottomLeft.Offset(1, -3); break;
                default: throw new Exception();
            }
            PlaceDoor(doorway);

            // carve the maze
            Maze maze = new Maze(width, height);
            maze.GrowTree();

            Vec offset = bounds.Position.Offset(1, 1);
            maze.Draw(pos => mWriter.SetTile(pos + offset, TileType.Floor));

            mWriter.LightRect(bounds, mDepth);

            // populate it
            int boostedDepth = mDepth + Rng.Int(mDepth / 5) + 2;
            Populate(bounds.Inflate(-2), 200, 300, boostedDepth);

            // place the connectors
            AddRoomConnectors(connector, bounds);

            return true;
        }

        private bool MakePit(Connector connector)
        {
            // pits use room size right now
            int width = Rng.Int(mWriter.Options.RoomSizeMin, mWriter.Options.RoomSizeMax);
            int height = Rng.Int(mWriter.Options.RoomSizeMin, mWriter.Options.RoomSizeMax);
            Rect bounds = CreateRectRoom(connector, width, height);

            // bail if we failed
            if (bounds == Rect.Empty) return false;

            // light it
            mWriter.LightRect(bounds, mDepth);

            // choose a group
            IList<Race> races = mWriter.Content.Races.AllInGroup(Rng.Item(mWriter.Content.Races.Groups));

            // make sure we've got some races that aren't too out of depth
            races = new List<Race>(races.Where(race => race.Depth <= mDepth + 10));
            if (races.Count == 0) return false;

            // place the room
            foreach (Vec pos in bounds)
            {
                mWriter.SetTile(pos, TileType.Floor);
            }

            RoomDecoration.DecorateInnerRoom(bounds, new RoomDecorator(this,
                pos => mWriter.AddEntity(new Monster(pos, Rng.Item(races)))));

            return true;
        }

        private Rect CreateRoom(Connector connector)
        {
            int width = Rng.Int(6, 13);
            int height = Rng.Int(6, 13);

            Rect bounds = CreateRectRoom(connector, width, height);

            // bail if we failed
            if (bounds == Rect.Empty) return bounds;

            // place the room
            foreach (Vec pos in bounds)
            {
                mWriter.SetTile(pos, TileType.Floor);
            }

            TileType decoration = ChooseInnerWall();

            RoomDecoration.Decorate(bounds, new FeatureFactory.RoomDecorator(this,
                pos => mWriter.Populate(pos, 60, 200, mDepth + Rng.Int(mDepth / 10))));

            mWriter.LightRect(bounds, mDepth);

            // place the connectors
            AddRoomConnectors(connector, bounds);

            Populate(bounds, 20, 20, mDepth);

            return bounds;
        }

        private Rect CreateRectRoom(Connector connector, int width, int height)
        {
            int x = 0;
            int y = 0;

            // position the room
            if (connector == null)
            {
                // initial room, so start near center
                x = Rng.TriangleInt((mWriter.Bounds.Width - width) / 2, (mWriter.Bounds.Width - width) / 2 - 4);
                y = Rng.TriangleInt((mWriter.Bounds.Height - height) / 2, (mWriter.Bounds.Height - height) / 2 - 4);
            }
            else if (connector.Direction == Direction.N)
            {
                // above the connector
                x = Rng.Int(connector.Position.X - width + 1, connector.Position.X + 1);
                y = connector.Position.Y - height;
            }
            else if (connector.Direction == Direction.E)
            {
                // to the right of the connector
                x = connector.Position.X + 1;
                y = Rng.Int(connector.Position.Y - height + 1, connector.Position.Y + 1);
            }
            else if (connector.Direction == Direction.S)
            {
                // below the connector
                x = Rng.Int(connector.Position.X - width + 1, connector.Position.X + 1);
                y = connector.Position.Y + 1;
            }
            else if (connector.Direction == Direction.W)
            {
                // to the left of the connector
                x = connector.Position.X - width;
                y = Rng.Int(connector.Position.Y - height + 1, connector.Position.Y + 1);
            }

            Rect bounds = new Rect(x, y, width, height);

            // check to see if the room can be positioned
            if (!mWriter.IsOpen(bounds.Inflate(1), (connector != null) ? (Vec?)connector.Position : (Vec?)null)) return Rect.Empty;

            return bounds;
        }

        private void AddRoomConnectors(Connector connector, Rect bounds)
        {
            AddRoomEdgeConnectors(connector, Rect.Row(bounds.TopLeft + Direction.N, bounds.Width), Direction.N);
            AddRoomEdgeConnectors(connector, Rect.Column(bounds.TopRight, bounds.Height), Direction.E);
            AddRoomEdgeConnectors(connector, Rect.Row(bounds.BottomLeft, bounds.Width), Direction.S);
            AddRoomEdgeConnectors(connector, Rect.Column(bounds.TopLeft + Direction.W, bounds.Height), Direction.W);
        }

        private void AddRoomEdgeConnectors(Connector connector, Rect edge, Direction dir)
        {
            bool skip = Rng.OneIn(2);

            foreach (Vec pos in edge)
            {
                // don't place connectors close to the incoming connector
                if (connector != null)
                {
                    if (Vec.IsDistanceWithin(connector.Position, pos, 1)) continue;
                }

                if (!skip && (Rng.Int(100) < mWriter.Options.ChanceOfRoomConnector))
                {
                    mWriter.AddRoomConnector(pos, dir);
                    skip = true;
                }
                else
                {
                    skip = false;
                }
            }
        }

        private void PlaceDoor(Vec pos)
        {
            int choice = Rng.Int(100);
            if (choice < mWriter.Options.ChanceOfOpenDoor)
            {
                mWriter.SetTile(pos, TileType.DoorOpen);
            }
            else if (choice - mWriter.Options.ChanceOfOpenDoor < mWriter.Options.ChanceOfClosedDoor)
            {
                mWriter.SetTile(pos, TileType.DoorClosed);
            }
            else
            {
                mWriter.SetTile(pos, TileType.Floor);
            }
            //### bob: add locked and secret doors
        }

        private void Populate(Rect bounds, int monsterDensity, int itemDensity, int depth)
        {
            // test every open tile
            foreach (Vec pos in bounds)
            {
                mWriter.Populate(pos, monsterDensity, itemDensity, depth);
            }
        }

        private TileType ChooseInnerWall()
        {
            return Rng.Item(new TileType[] { TileType.Wall, TileType.LowWall });
        }

        public class RoomDecorator : IRoomDecorator
        {
            public RoomDecorator(FeatureFactory factory, Action<Vec> insideRoom)
            {
                mInsideRoom = insideRoom;
                mFactory = factory;
                mDecoration = mFactory.ChooseInnerWall();
            }

            public void AddDecoration(Vec pos)
            {
                mFactory.mWriter.SetTile(pos, mDecoration);
            }

            public void AddInsideRoom(Vec pos)
            {
                mInsideRoom(pos);
            }

            public void AddDoor(Vec pos)
            {
                mFactory.PlaceDoor(pos);
            }

            private FeatureFactory mFactory;
            private Action<Vec> mInsideRoom;
            private TileType mDecoration;
        }

        private readonly IFeatureWriter mWriter;
        private readonly int mDepth;
    }
}
